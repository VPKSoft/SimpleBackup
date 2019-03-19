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
using System.Diagnostics;
using VPKSoft.LangLib;

namespace SimpleBackup
{
    public partial class FormSelectProcess : DBLangEngineWinforms
    {
        public FormSelectProcess()
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
            Process[] processlist = Process.GetProcesses();


            foreach (Process process in processlist)
            {
                try
                {
                    string processName = process.ProcessName;
                    string processFileName = process.Modules[0].FileName;
                    int id = process.Id;
                    ListViewItem lvi = new ListViewItem(process.ProcessName);
                    lvi.SubItems.Add(process.Modules[0].FileName);
                    lvi.Tag = id;
                    lvProcesses.Items.Add(lvi);
                }
                catch
                {
                    // Process is not accessible, so we don't list it
                }
            }
        }

        public static string Execute()
        {
            FormSelectProcess frm = new FormSelectProcess();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem lvi in frm.lvProcesses.SelectedItems)
                {
                    return lvi.SubItems[1].Text;
                }
            }
            return string.Empty;
        }

        private void lvProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {
            btOK.Enabled = lvProcesses.SelectedIndices.Count > 0;
        }
    }
}
