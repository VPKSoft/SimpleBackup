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
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SQLite;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;


namespace SimpleBackup
{
    public class CronEntry
    {
        private List<int> weekDays = new List<int>();
        private List<int> months = new List<int>();
        private List<int> days = new List<int>();
        private List<int> hours = new List<int>();
        private List<int> minutes = new List<int>();
        private string cron = string.Empty;
        private const string cronEmpty = "* * * * *";
        private object id = null;
        private string name = string.Empty;
        private string backupDirFrom = string.Empty;
        private string backupDirTo = string.Empty;
        private string backupFile = string.Empty;
        private string lastBackup = string.Empty;
        public string lastHash = string.Empty;
        private string nextBackup = string.Empty;
        private string refBackup = string.Empty;
        private string lastFail = string.Empty;
        private int killProcessSeconds = 0;
        private string closeProcess = string.Empty;

        private int failCount = 0;

        public class LastBackupEntry
        {
            public string FileName = string.Empty;
            public DateTime Taken = DateTime.Now;
            public string MD5Hash = string.Empty;
            public LastBackupEntry(string FileName, DateTime Taken, string MD5Hash)
            {
                this.MD5Hash = MD5Hash;
                this.Taken = Taken;
                this.FileName = FileName;
            }
        }

        private List<LastBackupEntry> lastBackups = new List<LastBackupEntry>();



        private int numsSave = 0;

        public override string ToString()
        {
            if (name == string.Empty)
            {
                return base.ToString();
            }
            else
            {
                return name;
            }
        }

        public string Cron
        {
            get
            {
                if (cron == string.Empty)
                {
                    return cronEmpty;
                }
                else
                {
                    return cron;
                }
            }

            set
            {
                cron = value;
            }
        }

        private BackgroundWorker backupThread = null;
        private volatile bool backupRunning;

        public bool BackupRunning
        {
            get
            {
                return backupRunning;
            }
        }

        public void DoBackup()
        {
            backupThread = new BackgroundWorker();
            backupThread.DoWork += (sender, args) => {
                Backup();
            };

            backupThread.RunWorkerCompleted += backupThread_RunWorkerCompleted;
            backupThread.RunWorkerAsync();
        }

        public delegate void BackupCompleted(CronEntry entry);

        public event BackupCompleted OnbackupCompleted = null;    

        void backupThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnbackupCompleted != null)
            {
                OnbackupCompleted(this);
            }
        }

        private volatile ZipDir.ZipReturn lastBackupReturn = new ZipDir.ZipReturn();

        public ZipDir.ZipReturn LastBackupState
        {
            get
            {
                return lastBackupReturn;
            }
        }

        public Type LastBackupExceptionType
        {
            get
            {
                return lastBackupReturn.ExceptionType;
            }
        }

        private string lastBackupFile = string.Empty;

        public string LastBackupFile
        {
            get
            {
                return lastBackupFile;
            }
        }

        private int retryHours = 1;

        public int RetryHours
        {
            get 
            {
                return retryHours;
            }

            set 
            {
                if (value > 0 && value <= 24)
                {
                    retryHours = value;
                }
                else
                {
                    throw new Exception("Invalid value RetryHours (1-24): " + value);
                }
            }
        }


        private bool BackupExists()
        {
            if (lastBackups.Count == 0)
            {
                return false;
            }
            return File.Exists(LastBackups.Last().FileName);
        }

        private void Backup()
        {
            backupRunning = true;
            bool processStopped;
            TakeProcessDown(out processStopped);
            lastBackupFile = DirTo + "\\" + BackuFileFormatted;
            lastHash = VPKSoft.Hashes.IOHash.MD5HashDirSimple(DirFrom);

            if (BackupExists())
            {
                if (lastHash != string.Empty && LastBackups.Last().MD5Hash == lastHash)
                {
                    lastBackupReturn.Flags = ZipDir.ZipReturnFlags.SameHash;
                }
                else
                {
                    lastBackupReturn = ZipDir.Compress(DirFrom, lastBackupFile);
                }
            }
            else
            {
                lastBackupReturn = ZipDir.Compress(DirFrom, lastBackupFile);
            }

            if (processStopped)
            {
                BringProcessUp();
            }
            backupRunning = false;
        }

        public static List<CronEntry> GetEntries(ref SQLiteConnection conn)
        {
            List<CronEntry> retval = new List<CronEntry>();
            List<int> ids = new List<int>();
            string sql = "SELECT ID FROM BACKUP ORDER BY ID ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ids.Add(dr.GetInt32(0));
                    }
                }
            }
            foreach (int id in ids)
            {
                retval.Add(GetByID(id, ref conn));
            }
            return retval;
        }

        public static CronEntry GetByID(int id, ref SQLiteConnection conn)
        {
            CronEntry retval = new CronEntry();
            string sql = "SELECT ID, NAME, LEAVEFILESMAX, BACKUP_FROMDIR, BACKUP_TODIR, BACKUP_FILENAME, LASTBACKUP, " +
                         "NEXTBACKUP, REFTIME, FAILCOUNT, LASTFAIL, PROCESS, KILLSPAN_SECONDS, RETRYHOURS " + 
                         "FROM BACKUP WHERE ID = " + id + " ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        retval.ID = dr.GetInt32(0);
                        retval.Name = dr.GetString(1);
                        retval.SaveBackupsNO = dr.GetInt32(2);
                        retval.DirFrom = dr.GetString(3);
                        retval.DirTo = dr.GetString(4);
                        retval.BackupFile = dr.GetString(5);
                        retval.lastBackup = dr.IsDBNull(6) ? string.Empty : dr.GetString(6);
                        retval.nextBackup = dr.IsDBNull(7) ? string.Empty : dr.GetString(7);
                        retval.refBackup = dr.IsDBNull(8) ? string.Empty : dr.GetString(8);
                        retval.failCount = dr.IsDBNull(9) ? 0 : dr.GetInt32(9);
                        retval.lastFail = dr.IsDBNull(10) ? string.Empty : dr.GetString(10);
                        retval.closeProcess = dr.IsDBNull(11) ? string.Empty : dr.GetString(11);
                        retval.killProcessSeconds = dr.IsDBNull(12) ? 0 : dr.GetInt32(12);
                        retval.retryHours = dr.IsDBNull(13) ? 1 : dr.GetInt32(13);
                    }
                }
            }

            sql = "SELECT WEEKDAY FROM WEEKDAYS WHERE ID = " + id + " ORDER BY WEEKDAY ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retval.WeekDays.Add(dr.GetInt32(0));
                    }
                }
            }

            sql = "SELECT MONTH FROM MONTHS WHERE ID = " + id + " ORDER BY MONTH ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retval.Months.Add(dr.GetInt32(0));
                    }
                }
            }

            sql = "SELECT DAY FROM DAYS WHERE ID = " + id + " ORDER BY DAY ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retval.Days.Add(dr.GetInt32(0));
                    }
                }
            }

            sql = "SELECT HOUR FROM HOURS WHERE ID = " + id + " ORDER BY HOUR ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retval.Hours.Add(dr.GetInt32(0));
                    }
                }
            }

            sql = "SELECT MINUTE FROM MINUTES WHERE ID = " + id + " ORDER BY MINUTE ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retval.Minutes.Add(dr.GetInt32(0));
                    }
                }
            }


            sql = "SELECT FILENAME, TAKEN, MD5HASH FROM TAKENBACKUPS WHERE ID = " + id + " ORDER BY TAKEN ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {                        
                        retval.lastBackups.Add(new LastBackupEntry(dr.GetString(0), FormulateDateTimeISO8601(dr.GetString(1)), dr.IsDBNull(2) ? string.Empty : dr.GetString(2)));
                    }
                }
            }

            sql = "SELECT CRON FROM CRONS WHERE ID = " + id + " ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retval.cron = dr.GetString(0);
                    }
                }
            }

            return retval;
        }

        private bool ValidCron()
        {
            try
            {
                if (Cron != cronEmpty)
                {
                    TimeCalcs.ConstructDate(1, Cron);
                }
                else
                {
                    TimeCalcs.ConstructDate(1, WeekDays, Months, Days, Hours, Minutes);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(ref SQLiteConnection conn)
        {
            if (id == null)
            {
                return false;
            }
            try
            {
                string sql = string.Format("DELETE FROM BACKUP WHERE ID = {0}; ", id) +
                             string.Format("DELETE FROM WEEKDAYS WHERE ID = {0}; ", id) +
                             string.Format("DELETE FROM MONTHS WHERE ID = {0}; ", id) +
                             string.Format("DELETE FROM DAYS WHERE ID = {0}; ", id) +
                             string.Format("DELETE FROM HOURS WHERE ID = {0}; ", id) +
                             string.Format("DELETE FROM MINUTES WHERE ID = {0}; ", id) +
                             string.Format("DELETE FROM CRONS WHERE ID = {0}; ", id) +
                             string.Format("DELETE FROM LOGS WHERE ID_BACKUP = {0}; ", id) +
                             string.Format("DELETE FROM TAKENBACKUPS WHERE ID = {0}; ", id);

                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(ref SQLiteConnection conn)
        {
            if (id == null ||
                name == string.Empty ||
                backupDirFrom == string.Empty ||
                backupDirTo == string.Empty ||
                name == string.Empty ||
                backupFile == string.Empty ||
                !ValidCron())
            {
                return false;
            }
            try
            {
                string sql = string.Format("UPDATE BACKUP SET " +
                                           "NAME = '{0}', " +
                                           "LEAVEFILESMAX = {1}, " +
                                           "BACKUP_FROMDIR = '{2}', " +
                                           "BACKUP_TODIR = '{3}', " +
                                           "BACKUP_FILENAME = '{4}', " +
                                           "LASTBACKUP = {5}, " +
                                           "NEXTBACKUP = '{6}', " +
                                           "REFTIME = '{7}', " +
                                           "FAILCOUNT = {8}, " +
                                           "LASTFAIL = {9}, " +
                                           "PROCESS = {10}, " +
                                           "KILLSPAN_SECONDS = {11}, " +
                                           "RETRYHOURS = {12} " +                                           
                                           "WHERE ID = {13} ",
                                           name.Replace("'", "''"),
                                           numsSave,
                                           backupDirFrom.Replace("'", "''"),
                                           backupDirTo.Replace("'", "''"),
                                           backupFile.Replace("'", "''"),
                                           LastBackup == DateTime.MinValue ? "NULL" : "'" + LastBackup.ToString("yyyy-MM-dd HH':'mm':'ss.fff") + "'",
                                           NextBackup == DateTime.MinValue ? RefDate.ToString("yyyy-MM-dd HH':'mm':'ss.fff") : NextBackup.ToString("yyyy-MM-dd HH':'mm':'ss.fff"),
                                           RefDate == DateTime.MinValue ? DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss.fff") : RefDate.ToString("yyyy-MM-dd HH':'mm':'ss.fff"),
                                           failCount,
                                           LastFail == DateTime.MinValue ? "NULL" : "'" + NextBackup.ToString("yyyy-MM-dd HH':'mm':'ss.fff") + "'",
                                           closeProcess == string.Empty ? "NULL" : "'" + closeProcess.Replace("'", "''") + "'",
                                           killProcessSeconds == 0 ? "NULL" : killProcessSeconds.ToString(), 
                                           retryHours,
                                           id);
                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }

                sql = string.Format("DELETE FROM WEEKDAYS WHERE ID = {0}; ", id) +
                        string.Format("DELETE FROM MONTHS WHERE ID = {0}; ", id) +
                        string.Format("DELETE FROM DAYS WHERE ID = {0}; ", id) +
                        string.Format("DELETE FROM HOURS WHERE ID = {0}; ", id) +
                        string.Format("DELETE FROM MINUTES WHERE ID = {0}; ", id) +
                        string.Format("DELETE FROM CRONS WHERE ID = {0}; ", id) +
                        string.Format("DELETE FROM TAKENBACKUPS WHERE ID = {0}; ", id);

                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }


                sql = string.Empty;
                foreach (int insert in weekDays)
                {
                    sql += string.Format("INSERT INTO WEEKDAYS(ID, WEEKDAY) VALUES({0}, {1});", id, insert);
                }

                foreach (int insert in months)
                {
                    sql += string.Format("INSERT INTO MONTHS(ID, MONTH) VALUES({0}, {1}); ", id, insert);
                }

                foreach (int insert in days)
                {
                    sql += string.Format("INSERT INTO DAYS(ID, DAY) VALUES({0}, {1}); ", id, insert);
                }

                foreach (int insert in hours)
                {
                    sql += string.Format("INSERT INTO HOURS(ID, HOUR) VALUES({0}, {1}); ", id, insert);
                }

                foreach (int insert in minutes)
                {
                    sql += string.Format("INSERT INTO MINUTES(ID, MINUTE) VALUES({0}, {1}); ", id, insert);
                }

                foreach (LastBackupEntry insert in lastBackups)
                {
                    sql += string.Format("INSERT INTO TAKENBACKUPS(ID, FILENAME, TAKEN, MD5HASH) VALUES({0}, '{1}', '{2}', '{3}'); ", id, insert.FileName, insert.Taken.ToString("yyyy-MM-dd HH':'mm':'ss.fff"), insert.MD5Hash);
                }

                sql += string.Format("INSERT INTO CRONS(ID, CRON) VALUES({0}, '{1}'); ", id, Cron);


                if (!(lastBackupReturn.Empty && LastBackupFile == string.Empty))
                {
                    sql += string.Format("INSERT INTO LOGS(ID_BACKUP, FILES_SUCCESS, FILES_FAILED, DIRCOUNT, EXCEPTION_TYPE, FLAGS, BACKUPFILE, TAKEN) " +
                                         "VALUES ({0}, {1}, {2}, {3}, '{4}', {5}, '{6}', '{7}'); ",
                                         id,
                                         lastBackupReturn.FileCount,
                                         lastBackupReturn.FailedFileCount,
                                         lastBackupReturn.DirCount,
                                         lastBackupReturn.ExceptionType.ToString(),
                                         (int)lastBackupReturn.Flags,
                                         lastBackupFile.Replace("'", "''"),
                                         LastBackup.ToString("yyyy-MM-dd HH':'mm':'ss.fff"));
                }

                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }

                HandleLog(ref conn);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TakeProcessDown(out bool closed)
        {
            closed = false;
            if (closeProcess != string.Empty)
            {
                Process[] processlist = System.Diagnostics.Process.GetProcesses();
                bool processFound = false;
                List<Process> processKilllist = new List<Process>();
                foreach (Process process in processlist)
                {
                    try
                    {
                        string processName = process.ProcessName;
                        string processFileName = process.Modules[0].FileName;
                        int id = process.Id;
                        if (processFileName.ToUpper() == closeProcess.ToUpper())
                        {
                            processKilllist.Add(process);
                        }
                    }
                    catch
                    {

                    }
                }


                foreach (Process process in processKilllist)
                {
                    try
                    {
                        processFound = true;
                        if (!process.CloseMainWindow())
                        {
                            if (!process.WaitForExit(KillProcessSeconds * 1000))
                            {
                                if (KillProcessWait)
                                {
                                    process.Kill();
                                    process.WaitForExit();
                                }
                            }
                        }
                        else
                        {
                            if (!process.WaitForExit(KillProcessSeconds * 1000))
                            {
                                if (KillProcessWait)
                                {
                                    process.Kill();
                                    process.WaitForExit();
                                }
                            }
                        }
                        closed = processFound;
                    }
                    catch
                    {
                        if (processFound)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool BringProcessUp()
        {
            try
            {
                if (closeProcess != string.Empty)
                {
                    System.Diagnostics.Process.Start(ProcessFile);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void HandleLog(ref SQLiteConnection conn)
        {
            int rCount = 0;
            string sql = "SELECT COUNT(*) FROM LOGS; ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        rCount = dr.GetInt32(0);
                    }
                }
            }

            if (rCount >= 10000)
            {
                sql = "INSERT INTO TMP_LOGS (ID, ID_BACKUP, FILES_SUCCESS, FILES_FAILED, DIRCOUNT, EXCEPTION_TYPE, FLAGS, BACKUPFILE, TAKEN) " +
                      "SELECT ID, ID_BACKUP, FILES_SUCCESS, FILES_FAILED, DIRCOUNT, EXCEPTION_TYPE, FLAGS, BACKUPFILE, TAKEN FROM LOGS ORDER BY TAKEN DESC LIMIT 5000; ";

                sql +=
                      "DELETE FROM LOGS; ";

                sql += 
                      "INSERT INTO LOGS (ID_BACKUP, FILES_SUCCESS, FILES_FAILED, DIRCOUNT, EXCEPTION_TYPE, FLAGS, BACKUPFILE, TAKEN) " +
                      "SELECT ID_BACKUP, FILES_SUCCESS, FILES_FAILED, DIRCOUNT, EXCEPTION_TYPE, FLAGS, BACKUPFILE, TAKEN FROM TMP_LOGS; ";

                sql +=
                      "DELETE FROM TMP_LOGS; ";
                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool Save(ref SQLiteConnection conn)
        {
            if (id != null ||
                name == string.Empty ||
                backupDirFrom == string.Empty ||
                backupDirTo == string.Empty ||
                name == string.Empty ||
                backupFile == string.Empty ||
                !ValidCron())
            {
                return false;
            }
            try
            {
                string sql = string.Format("INSERT INTO BACKUP (NAME, LEAVEFILESMAX, BACKUP_FROMDIR, BACKUP_TODIR, " +
                                           "BACKUP_FILENAME, REFTIME, PROCESS, KILLSPAN_SECONDS, RETRYHOURS) " +
                                           "VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', {6}, {7}, {8}) ",
                                           name.Replace("'", "''"),
                                           numsSave,
                                           backupDirFrom.Replace("'", "''"),
                                           backupDirTo.Replace("'", "''"),
                                           backupFile.Replace("'", "''"),
                                           DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss.fff"),
                                           closeProcess == string.Empty ? "NULL" : "'" + closeProcess.Replace("'", "''") + "'",
                                           killProcessSeconds == 0 ? "NULL" : killProcessSeconds.ToString(),
                                           retryHours);
                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }

                sql = "SELECT MAX(ID) FROM BACKUP ";

                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    using (SQLiteDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            id = dr.GetInt32(0);
                        }
                    }
                }

                sql = string.Empty;
                foreach (int insert in weekDays)
                {
                    sql += string.Format("INSERT INTO WEEKDAYS(ID, WEEKDAY) VALUES({0}, {1});", id, insert);
                }

                foreach (int insert in months)
                {
                    sql += string.Format("INSERT INTO MONTHS(ID, MONTH) VALUES({0}, {1}); ", id, insert);
                }

                foreach (int insert in days)
                {
                    sql += string.Format("INSERT INTO DAYS(ID, DAY) VALUES({0}, {1}); ", id, insert);
                }

                foreach (int insert in hours)
                {
                    sql += string.Format("INSERT INTO HOURS(ID, HOUR) VALUES({0}, {1}); ", id, insert);
                }

                foreach (int insert in minutes)
                {
                    sql += string.Format("INSERT INTO MINUTES(ID, MINUTE) VALUES({0}, {1}); ", id, insert);
                }

                sql += string.Format("INSERT INTO CRONS(ID, CRON) VALUES({0}, '{1}'); ", id, Cron);

                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool HandleTaken()
        {
            int dCount = 0;
            while (lastBackups.Count > SaveBackupsNO)
            {
                if (File.Exists(lastBackups.First().FileName))
                {
                    try
                    {
                        File.Delete(lastBackups.First().FileName);
                    }
                    catch
                    {
                        return false;
                    }
                }
                lastBackups.RemoveAt(0);
                dCount++;
            }
            return dCount > 0;
        }

        private static DateTime FormulateDateTimeISO8601(string value)
        {
            if (value == string.Empty)
            {
                return DateTime.MinValue;
            }
            else
            {
                try
                {
                    return DateTime.ParseExact(value, "yyyy-MM-dd HH':'mm':'ss.fff", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }

        public DateTime LastBackup
        {
            get
            {
                return FormulateDateTimeISO8601(lastBackup);
            }

            set
            {
                lastBackup = value.ToString("yyyy-MM-dd HH':'mm':'ss.fff");
            }
        }

        public void GenNextTime()
        {
            if (DateTime.Now < NextBackup)
            {
                return;
            }
            if (Cron != cronEmpty)
            {
                while (NextBackup < DateTime.Now)
                {
                    NextBackup = TimeCalcs.ConstructDate(1, Cron, NextBackup)[0];
                }
            }
            else
            {
                while (NextBackup < DateTime.Now)
                {
                    NextBackup = TimeCalcs.ConstructDate(1, weekDays, months, days, hours, minutes, NextBackup)[0];
                }
            }
        }


        public DateTime NextBackup
        {
            get
            {
                if (nextBackup != string.Empty)
                {
                    DateTime dt = FormulateDateTimeISO8601(nextBackup);
                    if (dt == DateTime.MinValue)
                    {
                        return DateTime.Now;
                    }
                    else
                    {
                        return dt;
                    }
                }
                else if (lastBackup != string.Empty)
                {
                    if (LastBackup == DateTime.MinValue)
                    {
                        return DateTime.Now;
                    }

                    if (Cron != cronEmpty)
                    {
                        return TimeCalcs.ConstructDate(1, Cron, LastBackup)[0];
                    }
                    else
                    {
                        return TimeCalcs.ConstructDate(1, weekDays, months, days, hours, minutes, LastBackup)[0];
                    }
                }
                else if (lastBackup == string.Empty)
                {
                    DateTime dt = FormulateDateTimeISO8601(refBackup);
                    if (dt == DateTime.MinValue)
                    {
                        return DateTime.Now;
                    }
                    else
                    {
                        return dt;
                    }
                }
                if (nextBackup == string.Empty)
                {
                    if (Cron != cronEmpty)
                    {
                        return TimeCalcs.ConstructDate(1, Cron)[0];
                    }
                    {
                        return TimeCalcs.ConstructDate(1, weekDays, months, days, hours, minutes)[0];
                    }
                }
                else
                {
                    DateTime dt = FormulateDateTimeISO8601(nextBackup);
                    if (dt == DateTime.MinValue)
                    {
                        return DateTime.Now;
                    }
                    else
                    {
                        return dt;
                    }
                }
            }

            set
            {
                nextBackup = value.ToString("yyyy-MM-dd HH':'mm':'ss.fff");
            }
        }

        public DateTime RefDate
        {
            get
            {
                return FormulateDateTimeISO8601(refBackup);
            }

            set
            {
                refBackup = value.ToString("yyyy-MM-dd HH':'mm':'ss.fff");
            }
        }

        public int KillProcessSeconds
        {
            get
            {
                return killProcessSeconds < 10 ? 10: killProcessSeconds;
            }

            set
            {
                if (value >= 10)
                {
                    killProcessSeconds = value;
                }
                else
                {
                    throw new Exception("Invalid process kill wait.");
                }
            }
        }

        public bool KillProcessWait
        {
            get
            {
                return killProcessSeconds >= 10;
            }

            set
            {
                if (!value)
                {
                    killProcessSeconds = 0;
                }
            }
        }

        public string ProcessFile
        {
            get
            {
                return closeProcess;
            }

            set
            {
                closeProcess = value;
            }
        }


        public int FailCount
        {
            get
            {
                
                return failCount;
            }

            set
            {
                failCount = value;
            }
        }

        public DateTime LastFail
        {
            get
            {
                return FormulateDateTimeISO8601(lastFail);
            }

            set
            {
                lastFail = value.ToString("yyyy-MM-dd HH':'mm':'ss.fff");
            }

        }

        public object ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string BackupFile
        {
            get
            {
                return backupFile;
            }

            set
            {
                backupFile = value;
            }
        }

        public string BackuFileFormatted
        {
            get
            {
                return FormatBackupFilename(backupFile);
            }
        }

        public static string FormatBackupFilename(string fileName)
        {
            string retval = fileName;
            if (!retval.ToUpper().EndsWith(".zip".ToUpper()))
            {
                retval = retval + ".zip";
            }

            DateTime dt = DateTime.Now;
            retval = retval.Replace("%Y", dt.Year.ToString());
            retval = retval.Replace("%M", dt.Month.ToString().PadLeft(2, '0'));
            retval = retval.Replace("%D", dt.Day.ToString().PadLeft(2, '0'));
            retval = retval.Replace("%H", dt.Hour.ToString().PadLeft(2, '0'));
            retval = retval.Replace("%m", dt.Minute.ToString().PadLeft(2, '0'));
            retval = retval.Replace("%S", dt.Second.ToString().PadLeft(2, '0'));
            return retval;
        }

        public string DirFrom
        {
            get
            {
                return backupDirFrom.TrimEnd('\\');
            }

            set
            {
                backupDirFrom = value;
            }
        }

        public string DirTo
        {
            get
            {
                return backupDirTo.TrimEnd('\\');
            }

            set
            {
                backupDirTo = value;
            }
        }

        public int SaveBackupsNO
        {
            get
            {
                return numsSave;
            }

            set
            {
                if (value < 1)
                {
                    throw new Exception("Number of saves must be at least 1.");
                }
                else if (value > 100)
                {
                    throw new Exception("Number of saves is too large.");
                }
                numsSave = value;
            }
        }

        public List<LastBackupEntry> LastBackups
        {
            get
            {
                return lastBackups;
            }

            set
            {
                lastBackups = value;
            }
        }


        public List<int> WeekDays
        {
            get
            {
                return weekDays;
            }

            set
            {
                weekDays = value;
            }
        }


        public List<int> Months
        {
            get
            {
                return months;
            }

            set
            {
                months = value;
            }
        }

        public List<int> Days
        {
            get
            {
                return days;
            }

            set
            {
                days = value;
            }
        }

        public List<int> Hours
        {
            get
            {
                return hours;
            }

            set
            {
                hours = value;
            }
        }

        public List<int> Minutes
        {
            get
            {
                return minutes;
            }

            set
            {
                minutes = value;
            }
        }

    }
}
