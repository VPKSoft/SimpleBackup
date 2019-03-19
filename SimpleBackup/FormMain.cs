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
using System.Data.SQLite;
using System.IO;
using Microsoft.Win32;
using System.Threading;
using VPKSoft.LangLib;
using VPKSoft.About;
using System.Globalization;

namespace SimpleBackup
{
    public partial class FormMain : DBLangEngineWinforms
    {
        bool quitFromMenu = false;
        bool BackupsRunning
        {
            get
            {
                return false;
            }
        }


        SQLiteConnection conn = null;

        BackgroundTimer tmCheckBackups = new BackgroundTimer(60000);
        BackgroundTimer tmWatchProcesses = new BackgroundTimer(1000);

        public FormMain()
        {
            InitializeComponent();

            try
            {
                DBLangEngine.DBName = "simplebackup_lang.sqlite";

                DBLangEngine.InitalizeLanguage("SimpleBackup.Messages");

                if (VPKSoft.LangLib.Utils.ShouldLocalize() != null)
                {
                    DBLangEngine.InitalizeLanguage("SimpleBackup.Messages", VPKSoft.LangLib.Utils.ShouldLocalize(), false);
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
            FormWaitBackups frmWait;
            foreach (ListViewObjectItem lvi in lvBackups.Items)
            {
                if ((lvi.ItemObject as CronEntry).BackupRunning)
                {
                    backupsRunning = true;
                }
            }

            if (backupsRunning)
            {
                frmWait = new FormWaitBackups();
                frmWait.Show();
                SetQuiEnabled(false);
                while (backupsRunning)
                {
                    backupsRunning = false;
                    foreach (ListViewObjectItem lvi in lvBackups.Items)
                    {
                        if ((lvi.ItemObject as CronEntry).BackupRunning)
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
                if ((lvi.ItemObject as CronEntry).BackupRunning)
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
                if (!(lvi.ItemObject as CronEntry).BackupRunning)
                {
                    if (DateTime.Now >= (lvi.ItemObject as CronEntry).NextBackup)
                    {
                        (lvi.ItemObject as CronEntry).DoBackup();
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
                this.SubItems[1].Text = en.LastBackup == DateTime.MinValue ? "-" : en.LastBackup.ToString();
                this.SubItems[2].Text = en.NextBackup.ToString();
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
                        en.OnbackupCompleted += FormMain_OnbackupCompleted;
                        item.ImageIndex = 0;
                        item.StateImageIndex = -1;
                        item.SubItems.Add(en.LastBackup == DateTime.MinValue ? "-" : en.LastBackup.ToString());
                        item.SubItems.Add(en.NextBackup.ToString());
                        lvBackups.Items.Add(item);
                    }
                }
                
            }
            else
            {
                foreach (CronEntry en in CronEntry.GetEntries(ref conn))
                {
                    ListViewObjectItem item = new ListViewObjectItem(en);
                    en.OnbackupCompleted += FormMain_OnbackupCompleted;
                    item.ImageIndex = 0;
                    item.StateImageIndex = -1;
                    item.SubItems.Add(en.LastBackup == DateTime.MinValue ? "-" : en.LastBackup.ToString());
                    item.SubItems.Add(en.NextBackup.ToString());
                    lvBackups.Items.Add(item);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!quitFromMenu && !BackupsRunning)
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



        private void btAdd_Click(object sender, EventArgs e)
        {
            FormEditBackup.ExecuteAdd(ref conn);
        }

        

        private void btRunNow_Click(object sender, EventArgs e)
        {
            foreach (ListViewObjectItem lvi in lvBackups.SelectedItems)
            {
                if (!(lvi.ItemObject as CronEntry).BackupRunning)
                {
                    (lvi.ItemObject as CronEntry).DoBackup();
                    pbProgress.Image = Properties.Resources.AnimatedBar;
                }
            }
        }

        void FormMain_OnbackupCompleted(CronEntry entry)
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
                CronEntry.LastBackupEntry e = new CronEntry.LastBackupEntry(entry.LastBackupFile, dt, entry.lastHash);
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
                Type t = en.LastBackupExceptionType;
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
//            ListBackups();
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
            CronEntry entry = (lvBackups.SelectedItems[0] as ListViewObjectItem).ItemObject as CronEntry;
            FormEditBackup.ExecuteEdit(ref entry , ref conn);
        }

        private void mnuDeleteBackup_Click(object sender, EventArgs e)
        {
            if (lvBackups.SelectedItems.Count == 0)
            {
                return;
            }
            CronEntry entry = (lvBackups.SelectedItems[0] as ListViewObjectItem).ItemObject as CronEntry;
            if (MessageBox.Show(DBLangEngine.GetMessage("msgDeleteBackup", "Delete backup '{0}'?|Query to confirm a backup deletion", entry), DBLangEngine.GetMessage("msgConfirm", "Confirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                entry.Delete(ref conn);
                ListBackups(true);
            }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            new VPKSoft.About.FormAbout(this, System.Reflection.Assembly.GetExecutingAssembly(), "GPL", "http://www.gnu.org/licenses/gpl-3.0.txt"); // 12.10.17, Added my "universal" about dialog..
//            new FormAbout().ShowDialog(); // 12.10.17, Disregraded this
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
    }
}
