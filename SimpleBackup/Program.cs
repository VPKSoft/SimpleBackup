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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SimpleBackup
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                VPKSoft.LangLib.DBLangEngine.DBName = "simplebackup_lang.sqlite";
                if (VPKSoft.LangLib.Utils.ShouldLocalize() != null)
                {
                    new FormEditBackup();
                    new FormMain();
                    new FormSelectProcess();
                    new FormWaitBackups();
                }
                else
                {
                    if (!Utils.CheckIfRunning("VPKSoft.SimpleBackup.c#"))
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new FormMain());
                    }
                    else
                    {
                        MessageBox.Show("SimpleBackup is already running..");
                    }
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void MakeDataDir()
        {
            string dir = Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"\" + Application.ProductName + @"\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        static public string AppDataDir
        {
            get
            {
                MakeDataDir();
                return Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"\" + Application.ProductName + @"\";
            }
        }

        public static string AppInstallDir
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

    }
}
