#region License
/*
A simple backup software to backup directories with a schedule.
Copyright (C) 2020 VPKSoft

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
using System.Windows.Forms;
using System.Data.SQLite;
using Microsoft.Win32;
using System.Threading;
using VPKSoft.LangLib;
using System.Globalization;
using System.Reflection;

namespace SimpleBackup
{
    public partial class FormMain : DBLangEngineWinforms
    {
        bool quitFromMenu;


        SQLiteConnection conn;

        readonly BackgroundTimer tmCheckBackups = new BackgroundTimer(60000);
        readonly BackgroundTimer tmWatchProcesses = new BackgroundTimer(1000);

        public FormMain()
        {
            InitializeComponent();

            try
            {
                // ReSharper disable twice StringLiteralTypo
                DBLangEngine.DBName = "simplebackup_lang.sqlite";

                DBLangEngine.InitializeLanguage("SimpleBackup.Messages");

                if (VPKSoft.LangLib.Utils.ShouldLocalize() != null)
                {
                    DBLangEngine.InitializeLanguage("SimpleBackup.Messages", VPKSoft.LangLib.Utils.ShouldLocalize(), false);
                    return; // After localization don't do anything more.
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            MainInit();
        }

        void MainInit()
        {
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            pbProgress.Image = null;
            Program.MakeDataDir();
            try
            {
                // ReSharper disable twice StringLiteralTypo
                conn = new SQLiteConnection("Data Source=" + Program.AppDataDir + "simplebackup.sqlite;Pooling=true;FailIfMissing=false");
                conn.Open();
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            try
            {
                if (!ScriptRunner.RunScript(Program.AppDataDir + "simplebackup.sqlite"))
                {
                    MessageBox.Show(DBLangEngine.GetMessage("msgErrorInScript", "Error in script: '{0}'!|The SQL script has an error in it", "?"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            ListBackups(false);
            tmCheckBackups.TimerElapsed += tmCheckBackups_TimerElapsed;
            tmWatchProcesses.TimerElapsed += tmWatchProcesses_TimerElapsed;
            tmWatchProcesses.Resolution = 2;
            tmWatchProcesses.Start();
            tmCheckBackups.Start();
            noIco.BalloonTipText = DBLangEngine.GetMessage("msgWorkBackground", "Running on the background...|As in running on the system tray..");

            // copy the localization from the menu items..
            tsbAddBackup.Text = mnuAddBackup.Text;
            tsbAddBackup.ToolTipText = mnuAddBackup.Text;
            tsbEditBackup.Text = mnuEditBackup.Text;
            tsbEditBackup.ToolTipText = mnuEditBackup.Text;
            tsbDeleteBackup.Text = mnuDeleteBackup.Text;
            tsbDeleteBackup.ToolTipText = mnuDeleteBackup.Text;
            tsbAbout.Text = mnuAbout.Text;
            tsbAbout.ToolTipText = mnuAbout.Text;
        }

        void SetQuiEnabled(bool enabled)
        {
            mnuHelp.Enabled = enabled;
            mnuFile.Enabled = enabled;
            btQuit.Enabled = enabled;
            btRunNow.Enabled = enabled;
            ControlBox = enabled;
            ShowInTaskbar = enabled;
        }

        void WaitForBackups()
        {
            bool backupsRunning = false;
            foreach (ListViewObjectItem lvi in lvBackups.Items)
            {
                if (((CronEntry) lvi.ItemObject).BackupRunning)
                {
                    backupsRunning = true;
                }
            }

            if (backupsRunning)
            {
                var frmWait = new FormWaitBackups();
                frmWait.Show();
                SetQuiEnabled(false);
                while (backupsRunning)
                {
                    backupsRunning = false;
                    foreach (ListViewObjectItem lvi in lvBackups.Items)
                    {
                        if (((CronEntry) lvi.ItemObject).BackupRunning)
                        {
                            backupsRunning = true;
                        }
                    }
                    Application.DoEvents();
                }
                frmWait.Close();
                SetQuiEnabled(true);
            }
        }

        void tmWatchProcesses_TimerElapsed(object sender, EventArgs e)
        {
            bool running = false;
            foreach (ListViewObjectItem lvi in lvBackups.Items)
            {
                if (((CronEntry) lvi.ItemObject).BackupRunning)
                {
                    lvi.ImageIndex = 1;
                    running = true;
                }
            }
            if (!running)
            {
                pbProgress.Image = null;
            }
        }

        void tmCheckBackups_TimerElapsed(object sender, EventArgs e)
        {
            foreach (ListViewObjectItem lvi in lvBackups.Items)
            {
                if (!((CronEntry) lvi.ItemObject).BackupRunning)
                {
                    if (DateTime.Now >= ((CronEntry) lvi.ItemObject).NextBackup)
                    {
                        ((CronEntry)lvi.ItemObject)?.DoBackup();
                        pbProgress.Image = Properties.Resources.AnimatedBar;
                    }
                }
            }
        }

        void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            quitFromMenu = true;
            Close();
        }

        [Serializable]
        public class ListViewObjectItem: ListViewItem
        {
            public ListViewObjectItem(string text): base(text) {}
            public ListViewObjectItem(object item) : base(item.ToString()) 
            {
                ItemObject = item;
                ToolTipText = "OK";
            }
            public object ItemObject = new object();
            public void UpdateItemObject()
            {

                CronEntry en = (CronEntry)ItemObject;
                this.Text = en.ToString();
                this.ToolTipText = en.LastBackupState.StatusString;
                this.SubItems[1].Text = en.LastBackup == DateTime.MinValue ? "-" : en.LastBackup.ToString(CultureInfo.CurrentCulture);
                this.SubItems[2].Text = en.NextBackup.ToString(CultureInfo.CurrentCulture);
            }
        }

        private void ListBackups(bool updateList)
        {
            if (updateList)
            {
                List<CronEntry> entries = CronEntry.GetEntries(ref conn);
                bool found;



                for (int i = lvBackups.Items.Count - 1; i >= 0; i--)
                {
                    ListViewObjectItem itemTmp = (ListViewObjectItem)lvBackups.Items[i];
                    found = false;
                    for (int j = 0; j < entries.Count; j++)
                    {
                        if (entries[j].ID == ((CronEntry)itemTmp.ItemObject).ID)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        lvBackups.Items.RemoveAt(i);
                    }
                }

                for (int i = 0; i < entries.Count; i++)
                {
                    found = false;
                    for (int j = 0; j < lvBackups.Items.Count; j++)
                    {
                        ListViewObjectItem itemTmp = (ListViewObjectItem)lvBackups.Items[j];
                        if (entries[i].ID == ((CronEntry)itemTmp.ItemObject).ID)
                        {
                            found = true;
                            break;
                        }                        
                    }

                    if (!found)
                    {
                        CronEntry en = entries[i];
                        ListViewObjectItem item = new ListViewObjectItem(en);
                        en.OnBackupCompleted += FormMain_OnBackupCompleted;
                        item.ImageIndex = 0;
                        item.StateImageIndex = -1;
                        item.SubItems.Add(en.LastBackup == DateTime.MinValue ? "-" : en.LastBackup.ToString(CultureInfo.CurrentCulture));
                        item.SubItems.Add(en.NextBackup.ToString(CultureInfo.CurrentCulture));
                        lvBackups.Items.Add(item);
                    }
                }
                
            }
            else
            {
                foreach (CronEntry en in CronEntry.GetEntries(ref conn))
                {
                    ListViewObjectItem item = new ListViewObjectItem(en);
                    en.OnBackupCompleted += FormMain_OnBackupCompleted;
                    item.ImageIndex = 0;
                    item.StateImageIndex = -1;
                    item.SubItems.Add(en.LastBackup == DateTime.MinValue ? "-" : en.LastBackup.ToString(CultureInfo.CurrentCulture));
                    item.SubItems.Add(en.NextBackup.ToString(CultureInfo.CurrentCulture));
                    lvBackups.Items.Add(item);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!quitFromMenu)
            {
                e.Cancel = true;
                Hide();
                ShowInTaskbar = false;
                noIco.ShowBalloonTip(1000);
            }
            else
            {
                WaitForBackups();
                tmCheckBackups.Enabled = false;
                tmWatchProcesses.Enabled = false;
                while (tmCheckBackups.IsBusy() || tmWatchProcesses.IsBusy())
                {
                    Thread.Sleep(1000);
                    Application.DoEvents();
                }

                using (conn)
                {
                    // Close through disposing..
                }
            }
            quitFromMenu = false;
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            quitFromMenu = true; 
            Close();
        }

        private void noIco_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            Show();
        }

        private void btRunNow_Click(object sender, EventArgs e)
        {
            foreach (ListViewObjectItem lvi in lvBackups.SelectedItems)
            {
                if (!((CronEntry) lvi.ItemObject).BackupRunning)
                {
                    ((CronEntry) lvi.ItemObject)?.DoBackup();
                    pbProgress.Image = Properties.Resources.AnimatedBar;
                }
            }
        }

        void FormMain_OnBackupCompleted(CronEntry entry)
        {
            DateTime dt = DateTime.Now;

            if (entry.LastBackupState.Flags.HasFlag(ZipDir.ZipReturnFlags.SameHash))
            {
                entry.GenNextTime();
                entry.Update(ref conn);
            }
            else if (entry.LastBackupState.Flags != ZipDir.ZipReturnFlags.Success)
            {
                entry.NextBackup = entry.NextBackup.AddHours(entry.RetryHours);
                while (entry.NextBackup <= DateTime.Now) // 12.10.17, Try to get up to date...
                {
                    entry.NextBackup = entry.NextBackup.AddHours(entry.RetryHours);
                }
                entry.Update(ref conn);
            }
            else
            {
                CronEntry.LastBackupEntry e = new CronEntry.LastBackupEntry(entry.LastBackupFile, dt, entry.LastHash);
                entry.LastBackups.Add(e);
                entry.LastBackup = dt;
                entry.GenNextTime();
                entry.HandleTaken();
                entry.Update(ref conn);
            }

            foreach (ListViewObjectItem lvi in lvBackups.Items)
            {
                lvi.UpdateItemObject();
                CronEntry en = (CronEntry)lvi.ItemObject;
                ZipDir.ZipReturn r = en.LastBackupState;
                if (r.Flags == ZipDir.ZipReturnFlags.Success)
                {
                    lvi.ImageIndex = 0;
                }
                else if (r.Flags.HasFlag(ZipDir.ZipReturnFlags.SameHash))
                {
                    lvi.ImageIndex = 3;
                }
                else if ((r.Flags & ZipDir.ZipReturnFlags.LockedFile) == ZipDir.ZipReturnFlags.LockedFile)
                {
                    lvi.ImageIndex = 2;
                }
            }
        }

        private void mnuAddBackup_Click(object sender, EventArgs e)
        {
            FormEditBackup.ExecuteAdd(ref conn);
            ListBackups(true);
        }

        private void mnuEditBackup_Click(object sender, EventArgs e)
        {
            if (lvBackups.SelectedItems.Count == 0)
            {
                return;
            }
            CronEntry entry = (lvBackups.SelectedItems[0] as ListViewObjectItem)?.ItemObject as CronEntry;
            FormEditBackup.ExecuteEdit(ref entry , ref conn);
        }

        private void mnuDeleteBackup_Click(object sender, EventArgs e)
        {
            if (lvBackups.SelectedItems.Count == 0)
            {
                return;
            }
            CronEntry entry = (lvBackups.SelectedItems[0] as ListViewObjectItem)?.ItemObject as CronEntry;
            if (MessageBox.Show(DBLangEngine.GetMessage("msgDeleteBackup", "Delete backup '{0}'?|Query to confirm a backup deletion", entry), DBLangEngine.GetMessage("msgConfirm", "Confirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                entry?.Delete(ref conn);
                ListBackups(true);
            }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new VPKSoft.VersionCheck.Forms.FormAbout(this, Assembly.GetEntryAssembly(), "GPL",
                "http://www.gnu.org/licenses/gpl-3.0.txt", "https://www.vpksoft.net/versions/version.php");
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            bool hidden = false, noBalloonTip = false;
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg == "--hidden")
                {
                    hidden = true;
                }
                if (arg == "--nob")
                {
                    noBalloonTip = true;
                }
            }
            if (hidden)
            {
                Hide();
                ShowInTaskbar = false;
                if (!noBalloonTip)
                {
                    noIco.ShowBalloonTip(1000);
                }
            }
        }

        private void EnableDisableGui()
        {
            mnuEditBackup.Enabled = lvBackups.SelectedItems.Count > 0;
            mnuDeleteBackup.Enabled = lvBackups.SelectedItems.Count > 0;
            mnuRunNow.Enabled = lvBackups.SelectedItems.Count > 0;
            btRunNow.Enabled = lvBackups.SelectedItems.Count > 0;
            tsbDeleteBackup.Enabled = lvBackups.SelectedItems.Count > 0;
            tsbEditBackup.Enabled = lvBackups.SelectedItems.Count > 0;
        }


        private void lvBackups_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisableGui();
        }

        private void FormMain_VisibleChanged(object sender, EventArgs e)
        {
            EnableDisableGui();
        }
    }
}
