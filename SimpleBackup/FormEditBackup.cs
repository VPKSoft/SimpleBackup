#region License
/*
A simple backup software to backup directories with a schedule.
Copyright (C) 2015  VPKSoft

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SQLite;
using System.IO;
using FolderSelect;
using VPKSoft.LangLib;

namespace SimpleBackup
{
    public partial class FormEditBackup : DBLangEngineWinforms
    {

        public static CronEntry ExecuteAdd(ref SQLiteConnection conn)
        {
            FormEditBackup frm = new FormEditBackup();
            frm.cbWeekDaysAll.Checked = true;
            frm.SetChecked(false, frm.cbWeekDaysAll);
            frm.cbMonthsAll.Checked = true;
            frm.SetChecked(false, frm.cbMonthsAll);
            frm.cbDaysAll.Checked = true;
            frm.SetChecked(false, frm.cbDaysAll);
            frm.cbHoursAll.Checked = true;
            frm.SetChecked(false, frm.cbHoursAll);
            frm.cbMinutesAll.Checked = true;
            frm.SetChecked(false, frm.cbMinutesAll);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                CronEntry entry = frm.GetEntry();
                entry.Save(ref conn);
                return entry;
            }
            else
            {
                return null;
            }
        }

        public static bool ExecuteEdit(ref CronEntry entry, ref SQLiteConnection conn)
        {
            FormEditBackup frm = new FormEditBackup();
            frm.Text = frm.DBLangEngine.GetMessage("msgEditBackup", "Edit backup|Edit backup");
            frm.tbBackupName.Text = entry.Name;
            frm.tbBackupDirFrom.Text = entry.DirFrom;
            frm.tbBackupDirTo.Text = entry.DirTo;
            frm.tbBackupFile.Text = entry.BackupFile;
            frm.nudAmout.Value = entry.SaveBackupsNO;
            frm.tbProcess.Text = entry.ProcessFile;
            frm.nudProcessKill.Value = entry.KillProcessSeconds;
            frm.cbKillProcess.Checked = entry.KillProcessWait;
            frm.nudRetryHours.Value = entry.RetryHours;


            if (entry.WeekDays.IndexOf(-1) >= 0)
            {
                frm.cbWeekDaysAll.Checked = true;
                frm.SetChecked(false, frm.cbWeekDaysAll);                
            }
            else if (entry.WeekDays.Count > 0)
            {
                if (DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek == DayOfWeek.Monday)
// DEBUG               if (DayOfWeek.Sunday == DayOfWeek.Monday)
                {
                    for (int i = 0; i < entry.WeekDays.Count; i++)
                    {
                        frm.clbWeekDays.SetItemChecked(entry.WeekDays[i] == 0 ? 6 : entry.WeekDays[i] - 1, true); // 12.10.17: entry.WeekDays[i] + 1 --> entry.WeekDays[i] - 1
                    }
                }
                else
                {
                    for (int i = 0; i < entry.WeekDays.Count; i++)
                    {
                        frm.clbWeekDays.SetItemChecked(entry.WeekDays[i], true);
                    }
                }
                frm.SetChecked(true, frm.clbWeekDays);
            }

            if (entry.Months.IndexOf(-1) >= 0)
            {
                frm.cbMonthsAll.Checked = true;
                frm.SetChecked(false, frm.cbMonthsAll);
            }
            else if (entry.Months.Count > 0)
            {
                for (int i = 0; i < entry.Months.Count; i++)
                {
                    frm.clbMonths.SetItemChecked(entry.Months[i], true);
                }
                frm.SetChecked(true, frm.clbMonths);
            }

            if (entry.Days.IndexOf(-1) >= 0)
            {
                frm.cbDaysAll.Checked = true;
                frm.SetChecked(false, frm.cbDaysAll);
            }
            else if (entry.Days.Count > 0)
            {
                for (int i = 0; i < entry.Days.Count; i++)
                {
                    frm.clbDays.SetItemChecked(entry.Days[i], true);
                }
                frm.SetChecked(true, frm.clbDays);
            }

            if (entry.Hours.IndexOf(-1) >= 0)
            {
                frm.cbHoursAll.Checked = true;
                frm.SetChecked(false, frm.cbHoursAll);
            }
            else if (entry.Hours.Count > 0)
            {
                for (int i = 0; i < entry.Hours.Count; i++)
                {
                    frm.clbHours.SetItemChecked(entry.Hours[i], true);
                }
                frm.SetChecked(true, frm.clbHours);
            }

            if (entry.Minutes.IndexOf(-1) >= 0)
            {
                frm.cbMinutesAll.Checked = true;
                frm.SetChecked(false, frm.cbMinutesAll);
            }
            else if (entry.Minutes.Count > 0)
            {
                for (int i = 0; i < entry.Minutes.Count; i++)
                {
                    frm.clbMinutes.SetItemChecked(entry.Minutes[i], true);
                }
                frm.SetChecked(true, frm.clbMinutes);
            }


            if (frm.ShowDialog() == DialogResult.OK)
            {
                frm.UpdateEntryParams(ref entry);
                entry.Update(ref conn);
                return true;
            }
            else
            {
                return false;
            }
        }

        FolderSelectDialog fbBrowse = new FolderSelectDialog();

        public FormEditBackup()
        {
            InitializeComponent();
            DBLangEngine.InitalizeLanguage("SimpleBackup.Messages");

            if (VPKSoft.LangLib.Utils.ShouldLocalize() != null)
            {
                DBLangEngine.InitalizeLanguage("SimpleBackup.Messages", VPKSoft.LangLib.Utils.ShouldLocalize(), false);
                return; // After localization don't do anything more.
            }
            MainInit();
        }

        void MainInit()
        {
            List<string> days = new List<string>();
// DEBUG            if (DayOfWeek.Sunday == DayOfWeek.Monday)
            if (DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek == DayOfWeek.Monday)
            {
                days.Add(DateTimeFormatInfo.CurrentInfo.DayNames[1]);
                days.Add(DateTimeFormatInfo.CurrentInfo.DayNames[2]);
                days.Add(DateTimeFormatInfo.CurrentInfo.DayNames[3]);
                days.Add(DateTimeFormatInfo.CurrentInfo.DayNames[4]);
                days.Add(DateTimeFormatInfo.CurrentInfo.DayNames[5]);
                days.Add(DateTimeFormatInfo.CurrentInfo.DayNames[6]);
                days.Add(DateTimeFormatInfo.CurrentInfo.DayNames[0]);
            }
            else
            {
                days.AddRange(DateTimeFormatInfo.CurrentInfo.DayNames.ToArray());
            }

            btOK.Enabled = false;

            clbWeekDays.Items.AddRange(days.ToArray());

            List<string> months = new List<string>();
            months.AddRange(DateTimeFormatInfo.CurrentInfo.MonthNames);
            for (int i = 0; i < months.Count; i++)
            {
                months[i] = (i + 1).ToString().PadLeft(2, '0') + " - " + months[i];
            }

            if (months.Count > 12)
                months.RemoveAt(12);

            clbMonths.Items.AddRange(months.ToArray());

            for (int i = 1; i <= 31; i++)
            {
                clbDays.Items.Add(i.ToString());
            }

            for (int i = 0; i < 24; i++)
            {
                clbHours.Items.Add(i.ToString().PadLeft(2, '0'));
            }

            for (int i = 0; i < 60; i++)
            {
                clbMinutes.Items.Add(i.ToString().PadLeft(2, '0'));
            }
        }


        private void UpdateEntryParams(ref CronEntry entry)
        {
            List<int> weekDays = new List<int>();
            List<int> months = new List<int>();
            List<int> days = new List<int>();
            List<int> hours = new List<int>();
            List<int> minutes = new List<int>();
            if (cbWeekDaysAll.Checked || clbWeekDays.CheckedIndices.Count == 0)
            {
                weekDays.Add(-1);
            }
            else
            {
                for (int i = 0; i < clbWeekDays.Items.Count; i++)
                {
                    if (clbWeekDays.GetItemChecked(i))
                    {
                        if (DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek == DayOfWeek.Monday)
                        {
                            switch (i)
                            {
                                case 0: weekDays.Add(1); break;
                                case 1: weekDays.Add(2); break;
                                case 2: weekDays.Add(3); break;
                                case 3: weekDays.Add(4); break;
                                case 4: weekDays.Add(5); break;
                                case 5: weekDays.Add(6); break;
                                case 6: weekDays.Add(0); break;
                                default: weekDays.Add(-1); break;
                            }
                        }
                        else
                        {
                            if (clbWeekDays.GetItemChecked(i))
                            {
                                weekDays.Add(i);
                            }
                        }
                    }
                }
            }

            if (cbHoursAll.Checked || clbHours.CheckedIndices.Count == 0)
            {
                hours.Add(-1);
            }
            else
            {
                for (int i = 0; i < clbHours.Items.Count; i++)
                {
                    if (clbHours.GetItemChecked(i))
                    {
                        hours.Add(i);
                    }
                }
            }

            if (cbMinutesAll.Checked || clbMinutes.CheckedIndices.Count == 0)
            {
                minutes.Add(-1);
            }
            else
            {
                for (int i = 0; i < clbMinutes.Items.Count; i++)
                {
                    if (clbMinutes.GetItemChecked(i))
                    {
                        minutes.Add(i);
                    }
                }
            }

            if (cbMonthsAll.Checked || clbMonths.CheckedIndices.Count == 0)
            {
                months.Add(-1);
            }
            else
            {
                for (int i = 0; i < clbMonths.Items.Count; i++)
                {
                    if (clbMonths.GetItemChecked(i))
                    {
                        months.Add(i + 1);
                    }
                }
            }

            if (cbDaysAll.Checked || clbDays.CheckedIndices.Count == 0)
            {
                days.Add(-1);
            }
            else
            {
                for (int i = 0; i < clbDays.Items.Count; i++)
                {
                    if (clbDays.GetItemChecked(i))
                    {
                        days.Add(i + 1);
                    }
                }
            }
            entry.WeekDays = weekDays;
            entry.Months = months;
            entry.Days = days;
            entry.Hours = hours;
            entry.Minutes = minutes;

            if (cbUseCronEntry.Checked)
            {
                entry.Cron = tbCronEntry.Text;
            }
            else
            {
                entry.Cron = "* * * * *";
            }

            entry.Name = tbBackupName.Text;
            entry.SaveBackupsNO = Convert.ToInt32(nudAmout.Value);
            entry.BackupFile = tbBackupFile.Text;
            entry.DirTo = tbBackupDirTo.Text;
            entry.DirFrom = tbBackupDirFrom.Text;

            entry.RetryHours = Convert.ToInt32(nudRetryHours.Value);

            if (cbKillProcess.Checked)
            {
                entry.KillProcessSeconds = (int)nudProcessKill.Value;
            }
            else
            {
                entry.KillProcessWait = false;
            }
            entry.ProcessFile = tbProcess.Text;
        }

        public CronEntry GetEntry()
        {
            CronEntry retEntry = new CronEntry();
            UpdateEntryParams(ref retEntry);
            return retEntry;
        }

        private TimeSpan tsMin = TimeSpan.MaxValue, tsMax = TimeSpan.MinValue;
        private string occurenceError = string.Empty;

        void GetNextOccurenceList()
        {
            CronEntry entry = GetEntry();


            lvDates.Items.Clear();
            tsMin = TimeSpan.MaxValue;
            tsMax = TimeSpan.MinValue;
            occurenceError = string.Empty;
            try
            {
                List<DateTime> dates;
                if (cbUseCronEntry.Checked)
                {
                    dates = TimeCalcs.ConstructDate(20, tbCronEntry.Text);
                }
                else
                {
                     dates = TimeCalcs.ConstructDate(20, entry.WeekDays, entry.Months, entry.Days, entry.Hours, entry.Minutes);
                }

                for (int i = 0; i < dates.Count - 1; i++)
                {
                    if (dates[i + 1] - dates [i] < tsMin)
                    {
                        tsMin = dates[i + 1] - dates[i];
                    }

                    if (dates[i + 1] - dates[i] > tsMax)
                    {
                        tsMax = dates[i + 1] - dates[i];
                    }

                }

                foreach (DateTime dt in dates)
                {
                    DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                    ListViewItem lvi = new ListViewItem(dt.ToString("dddd"));
                    lvi.SubItems.Add(dt.ToString("yyyy"));
                    lvi.SubItems.Add(Convert.ToInt32(dt.ToString("MM")).ToString().TrimStart('0'));
                    lvi.SubItems.Add(dfi.MonthNames[Convert.ToInt32(dt.ToString("MM")) - 1]);
                    lvi.SubItems.Add(dfi.Calendar.GetWeekOfYear(dt, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString());
                    lvi.SubItems.Add(dt.ToString("dd").TrimStart('0'));
                    lvi.SubItems.Add(dt.ToString("HH:mm"));
                    lvi.SubItems.Add(dt.ToLongDateString() + " " + dt.ToLongTimeString());
                    lvDates.Items.Add(lvi);
                    btOK.Enabled = true;
                }
                SetOKEnable();
            } 
            catch
            {
                ListViewItem lvi = new ListViewItem(DBLangEngine.GetMessage("msgInvalidDatetime", "INVALID DATE/TIME|As it is"));

                ttTimeWarning.SetToolTip(pbTimeWarning, DBLangEngine.GetMessage("msgInvalidDatetime", "INVALID DATE/TIME|As it is"));
                occurenceError = DBLangEngine.GetMessage("msgInvalidDatetime", "INVALID DATE/TIME|As it is");
                btOK.Enabled = false;
                SetOKEnable();
                lvDates.Items.Add(lvi);
            }
        }

        private void SetOKEnable()
        {
            string infoText = string.Empty;
            pbTimeWarning.Image = Bitmap.FromHicon(SystemIcons.Information.Handle);
            if (occurenceError != string.Empty)
            {
                infoText += occurenceError + Environment.NewLine;
                pbTimeWarning.Image = Bitmap.FromHicon(SystemIcons.Exclamation.Handle);
            }
            if (tsMin.TotalMinutes < 5)
            {

                infoText += DBLangEngine.GetMessage("msgOccurencesSmall", "Some time occurences would be larger than a year.|Some time occurences in generated time values are too large") + Environment.NewLine;
                pbTimeWarning.Image = Bitmap.FromHicon(SystemIcons.Exclamation.Handle);
            }
            if (tsMax.TotalDays > 366)
            {
                infoText += DBLangEngine.GetMessage("msgOccurencesLarge", "Some time occurences would be larger than a year.|Some time occurences in generated time values are too large") + Environment.NewLine;
                pbTimeWarning.Image = Bitmap.FromHicon(SystemIcons.Exclamation.Handle);
            }

            if (!Directory.Exists(tbBackupDirFrom.Text))
            {

                infoText += DBLangEngine.GetMessage("msdDirNotExists", "Directory '{0}' does not exist.{1}|Directory does not exist.", tbBackupDirFrom.Text, Environment.NewLine);
                pbTimeWarning.Image = Bitmap.FromHicon(SystemIcons.Exclamation.Handle);
            }

            if (!Directory.Exists(tbBackupDirTo.Text))
            {
                infoText += DBLangEngine.GetMessage("msdDirNotExists", "Directory '{0}' does not exist.{1}|Directory does not exist.", tbBackupDirTo.Text, Environment.NewLine);
                pbTimeWarning.Image = Bitmap.FromHicon(SystemIcons.Exclamation.Handle);
            }

            if (!Utils.ValidFileName(CronEntry.FormatBackupFilename(tbBackupFile.Text)))
            {
                infoText += DBLangEngine.GetMessage("msgFileNameInvalidChars", "Backup file name ('{0}') contains invalid characters.{1}|File name contains invalid characters", tbBackupFile.Text, Environment.NewLine);
                pbTimeWarning.Image = Bitmap.FromHicon(SystemIcons.Exclamation.Handle);
            }

            if (tbBackupName.Text == string.Empty)
            {
                infoText += DBLangEngine.GetMessage("msgBackupMustHaveName", "Backup must have a name.{0}|As said", Environment.NewLine);
                pbTimeWarning.Image = Bitmap.FromHicon(SystemIcons.Exclamation.Handle);
            }

            ttTimeWarning.SetToolTip(pbTimeWarning, infoText == string.Empty ? "OK" : infoText);
            btOK.Enabled = occurenceError == string.Empty &&
                           Directory.Exists(tbBackupDirFrom.Text) &&
                           Directory.Exists(tbBackupDirTo.Text) &&
                           tbBackupName.Text != string.Empty &&
                           Utils.ValidFileName(CronEntry.FormatBackupFilename(tbBackupFile.Text));
        }

        private void btNextTime_Click(object sender, EventArgs e)
        {
            this.GetNextOccurenceList();
        }

        private void clbClick(object sender, EventArgs e)
        {
            SetChecked(true, sender);
        }

        private void cbClick(object sender, EventArgs e)
        {
            if (sender is CheckBox)
                SetChecked(false, sender);
        }

        private void ToggleCheckLcb(CheckedListBox clb, bool check)
        {
            for (int i = 0; i < clb.Items.Count; i++)
            {
                clb.SetItemCheckState(i, check ? CheckState.Checked : CheckState.Unchecked);
            }
            GetNextOccurenceList();
        }

        private void SetChecked(bool clb, object sender)
        {
            if (clb)
            {
                cbWeekDaysAll.Checked = (clbWeekDays.CheckedIndices.Count == clbWeekDays.Items.Count);
                cbDaysAll.Checked = (clbDays.CheckedIndices.Count == clbDays.Items.Count);
                cbHoursAll.Checked = (clbHours.CheckedIndices.Count == clbHours.Items.Count);
                cbMonthsAll.Checked = (clbMonths.CheckedIndices.Count == clbMonths.Items.Count);
                cbMinutesAll.Checked = (clbMinutes.CheckedIndices.Count == clbMinutes.Items.Count);
            }
            else
            {
                if (sender is CheckBox)
                {
                    if ((sender as CheckBox) == cbWeekDaysAll)
                        ToggleCheckLcb(clbWeekDays, cbWeekDaysAll.Checked);

                    if ((sender as CheckBox) == cbDaysAll)
                        ToggleCheckLcb(clbDays, cbDaysAll.Checked);

                    if ((sender as CheckBox) == cbHoursAll)
                        ToggleCheckLcb(clbHours, cbHoursAll.Checked);

                    if ((sender as CheckBox) == cbMonthsAll)
                        ToggleCheckLcb(clbMonths, cbMonthsAll.Checked);

                    if ((sender as CheckBox) == cbMinutesAll)
                        ToggleCheckLcb(clbMinutes, cbMinutesAll.Checked);

                    if ((sender as CheckBox) == cbUseCronEntry)
                    {
                        ToggleCheckLcb(clbMinutes, cbUseCronEntry.Checked);
                        lbCronHelp.Visible = cbUseCronEntry.Checked;
                    }
                }
            }
            GetNextOccurenceList();
        }

        private void lvDates_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = lvDates.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        private void btSelectBackupFrom_Click(object sender, EventArgs e)
        {
            if (fbBrowse.ShowDialog(this.Handle))
            {
                tbBackupDirFrom.Text = fbBrowse.FileName;
                if (tbBackupName.Text == string.Empty)
                {
                    tbBackupName.Text = tbBackupDirFrom.Text.Split(Path.DirectorySeparatorChar).Last();
                }
            }
        }

        private void btSelectBackupTo_Click(object sender, EventArgs e)
        {
            if (fbBrowse.ShowDialog(this.Handle))
            {
                tbBackupDirTo.Text = fbBrowse.FileName;
            }
        }

        private void cbUseCronEntry_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tbCronEntry_TextChanged(object sender, EventArgs e)
        {
            GetNextOccurenceList();
        }

        private void tbTextChange(object sender, EventArgs e)
        {
            SetOKEnable();
        }

        private void btSelectProcess_Click(object sender, EventArgs e)
        {
            tbProcess.Text = FormSelectProcess.Execute();
        }

        private void btOK_Click(object sender, EventArgs e)
        {

        }
    }
}
