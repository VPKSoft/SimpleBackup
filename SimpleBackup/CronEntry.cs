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
using System.Linq;
using System.Data.SQLite;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;


namespace SimpleBackup
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class CronEntry
    {
        private List<int> weekDays = new List<int>();
        private List<int> months = new List<int>();
        private List<int> days = new List<int>();
        private List<int> hours = new List<int>();
        private List<int> minutes = new List<int>();
        private string cron = string.Empty;
        private const string CronEmpty = "* * * * *";
        private object id;
        private string name = string.Empty;
        private string backupDirFrom = string.Empty;
        private string backupDirTo = string.Empty;
        private string backupFile = string.Empty;
        private string lastBackup = string.Empty;
        public string LastHash = string.Empty;
        private string nextBackup = string.Empty;
        private string refBackup = string.Empty;
        private string lastFail = string.Empty;
        private int killProcessSeconds;
        private string closeProcess = string.Empty;

        public class LastBackupEntry
        {
            public string FileName;
            public DateTime Taken;
            public string Md5Hash;
            public LastBackupEntry(string fileName, DateTime taken, string md5Hash)
            {
                this.Md5Hash = md5Hash;
                this.Taken = taken;
                this.FileName = fileName;
            }
        }

        private List<LastBackupEntry> lastBackups = new List<LastBackupEntry>();



        private int numbersSave;

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
                    return CronEmpty;
                }
                else
                {
                    return cron;
                }
            }

            set => cron = value;
        }

        private BackgroundWorker backupThread;
        private volatile bool backupRunning;

        public bool BackupRunning => backupRunning;

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

        public event BackupCompleted OnBackupCompleted;    

        void backupThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnBackupCompleted?.Invoke(this);
        }

        private volatile ZipDir.ZipReturn lastBackupReturn = new ZipDir.ZipReturn();

        public ZipDir.ZipReturn LastBackupState => lastBackupReturn;

        private string lastBackupFile = string.Empty;

        public string LastBackupFile => lastBackupFile;

        private int retryHours = 1;

        public int RetryHours
        {
            get => retryHours;

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

        /// <summary>
        /// Gets or sets a value indicating whether the backup will skip locked files.
        /// </summary>
        /// <value><c>true</c> if locked files are skipped and the backup is allowed to continue; otherwise, <c>false</c>.</value>
        public bool AllowLockedFiles { get; set; }

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
            TakeProcessDown(out var processStopped);
            lastBackupFile = DirTo + "\\" + BackupFileFormatted;
            LastHash = IoHash.Md5HashDirSimple(DirFrom);

            if (BackupExists())
            {
                if (LastHash != string.Empty && LastBackups.Last().Md5Hash == LastHash)
                {
                    lastBackupReturn.Flags = ZipDir.ZipReturnFlags.SameHash;
                }
                else
                {
                    lastBackupReturn = ZipDir.Compress(DirFrom, lastBackupFile, AllowLockedFiles);
                }
            }
            else
            {
                lastBackupReturn = ZipDir.Compress(DirFrom, lastBackupFile, AllowLockedFiles);
            }

            if (processStopped)
            {
                BringProcessUp();
            }
            backupRunning = false;
        }

        public static List<CronEntry> GetEntries(ref SQLiteConnection conn)
        {
            List<CronEntry> result = new List<CronEntry>();
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
                result.Add(GetById(id, ref conn));
            }
            return result;
        }

        public static CronEntry GetById(int id, ref SQLiteConnection conn)
        {
            CronEntry result = new CronEntry();
            string sql = "SELECT ID, NAME, LEAVEFILESMAX, BACKUP_FROMDIR, BACKUP_TODIR, BACKUP_FILENAME, LASTBACKUP, " +
                         "NEXTBACKUP, REFTIME, FAILCOUNT, LASTFAIL, PROCESS, KILLSPAN_SECONDS, RETRYHOURS, ALLOWLOCKEDFILES " + 
                         "FROM BACKUP WHERE ID = " + id + " ";
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        result.ID = dr.GetInt32(0);
                        result.Name = dr.GetString(1);
                        result.SaveBackupsNo = dr.GetInt32(2);
                        result.DirFrom = dr.GetString(3);
                        result.DirTo = dr.GetString(4);
                        result.BackupFile = dr.GetString(5);
                        result.lastBackup = dr.IsDBNull(6) ? string.Empty : dr.GetString(6);
                        result.nextBackup = dr.IsDBNull(7) ? string.Empty : dr.GetString(7);
                        result.refBackup = dr.IsDBNull(8) ? string.Empty : dr.GetString(8);
                        result.FailCount = dr.IsDBNull(9) ? 0 : dr.GetInt32(9);
                        result.lastFail = dr.IsDBNull(10) ? string.Empty : dr.GetString(10);
                        result.closeProcess = dr.IsDBNull(11) ? string.Empty : dr.GetString(11);
                        result.killProcessSeconds = dr.IsDBNull(12) ? 0 : dr.GetInt32(12);
                        result.retryHours = dr.IsDBNull(13) ? 1 : dr.GetInt32(13);
                        result.AllowLockedFiles = dr.GetInt32(14) == 1;
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
                        result.WeekDays.Add(dr.GetInt32(0));
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
                        result.Months.Add(dr.GetInt32(0));
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
                        result.Days.Add(dr.GetInt32(0));
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
                        result.Hours.Add(dr.GetInt32(0));
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
                        result.Minutes.Add(dr.GetInt32(0));
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
                        result.lastBackups.Add(new LastBackupEntry(dr.GetString(0), FormulateDateTimeIso8601(dr.GetString(1)), dr.IsDBNull(2) ? string.Empty : dr.GetString(2)));
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
                        result.cron = dr.GetString(0);
                    }
                }
            }

            return result;
        }

        private bool ValidCron()
        {
            try
            {
                if (Cron != CronEmpty)
                {
                    TimeCalculations.ConstructDate(1, Cron);
                }
                else
                {
                    TimeCalculations.ConstructDate(1, WeekDays, Months, Days, Hours, Minutes);
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
                string sql = $"DELETE FROM BACKUP WHERE ID = {id}; " +
                             $"DELETE FROM WEEKDAYS WHERE ID = {id}; " +
                             $"DELETE FROM MONTHS WHERE ID = {id}; " +
                             $"DELETE FROM DAYS WHERE ID = {id}; " +
                             $"DELETE FROM HOURS WHERE ID = {id}; " +
                             $"DELETE FROM MINUTES WHERE ID = {id}; " +
                             $"DELETE FROM CRONS WHERE ID = {id}; " +
                             $"DELETE FROM LOGS WHERE ID_BACKUP = {id}; " +
                             $"DELETE FROM TAKENBACKUPS WHERE ID = {id}; ";

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
                string sql = string.Join(Environment.NewLine,
                    "UPDATE BACKUP SET " + $"NAME = '{name.Replace("'", "''")}', ",
                    $"LEAVEFILESMAX = {numbersSave}, ",
                    $"BACKUP_FROMDIR = '{backupDirFrom.Replace("'", "''")}', ",
                    $"BACKUP_TODIR = '{backupDirTo.Replace("'", "''")}', ",
                    $"BACKUP_FILENAME = '{backupFile.Replace("'", "''")}', ",
                    $"LASTBACKUP = {(LastBackup == DateTime.MinValue ? "NULL" : "'" + LastBackup.ToString("yyyy-MM-dd HH':'mm':'ss.fff") + "'")}, ",
                    $"NEXTBACKUP = '{(NextBackup == DateTime.MinValue ? RefDate.ToString("yyyy-MM-dd HH':'mm':'ss.fff") : NextBackup.ToString("yyyy-MM-dd HH':'mm':'ss.fff"))}', ",
                    $"REFTIME = '{(RefDate == DateTime.MinValue ? DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss.fff") : RefDate.ToString("yyyy-MM-dd HH':'mm':'ss.fff"))}', ",
                    $"FAILCOUNT = {FailCount}, ",
                    $"LASTFAIL = {(LastFail == DateTime.MinValue ? "NULL" : "'" + NextBackup.ToString("yyyy-MM-dd HH':'mm':'ss.fff") + "'")}, ",
                    $"PROCESS = {(closeProcess == string.Empty ? "NULL" : "'" + closeProcess.Replace("'", "''") + "'")}, ",
                    $"KILLSPAN_SECONDS = {(killProcessSeconds == 0 ? "NULL" : killProcessSeconds.ToString())}, ",
                    $"RETRYHOURS = {retryHours}, ",
                    $"ALLOWLOCKEDFILES = {(AllowLockedFiles ? "1" : "0")}",
                    $"WHERE ID = {id} ");
                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }

                sql = $"DELETE FROM WEEKDAYS WHERE ID = {id}; " +
                      $"DELETE FROM MONTHS WHERE ID = {id}; " +
                      $"DELETE FROM DAYS WHERE ID = {id}; " +
                      $"DELETE FROM HOURS WHERE ID = {id}; " +
                      $"DELETE FROM MINUTES WHERE ID = {id}; " +
                      $"DELETE FROM CRONS WHERE ID = {id}; " +
                      $"DELETE FROM TAKENBACKUPS WHERE ID = {id}; ";

                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }


                sql = string.Empty;
                foreach (int insert in weekDays)
                {
                    sql += $"INSERT INTO WEEKDAYS(ID, WEEKDAY) VALUES({id}, {insert});";
                }

                foreach (int insert in months)
                {
                    sql += $"INSERT INTO MONTHS(ID, MONTH) VALUES({id}, {insert}); ";
                }

                foreach (int insert in days)
                {
                    sql += $"INSERT INTO DAYS(ID, DAY) VALUES({id}, {insert}); ";
                }

                foreach (int insert in hours)
                {
                    sql += $"INSERT INTO HOURS(ID, HOUR) VALUES({id}, {insert}); ";
                }

                foreach (int insert in minutes)
                {
                    sql += $"INSERT INTO MINUTES(ID, MINUTE) VALUES({id}, {insert}); ";
                }

                foreach (LastBackupEntry insert in lastBackups)
                {
                    sql +=
                        $"INSERT INTO TAKENBACKUPS(ID, FILENAME, TAKEN, MD5HASH) VALUES({id}, '{insert.FileName}', '{insert.Taken:yyyy-MM-dd HH':'mm':'ss.fff}', '{insert.Md5Hash}'); ";
                }

                sql += $"INSERT INTO CRONS(ID, CRON) VALUES({id}, '{Cron}'); ";


                if (!(lastBackupReturn.Empty && LastBackupFile == string.Empty))
                {
                    sql +=
                        "INSERT INTO LOGS(ID_BACKUP, FILES_SUCCESS, FILES_FAILED, DIRCOUNT, EXCEPTION_TYPE, FLAGS, BACKUPFILE, TAKEN) " +
                        $"VALUES ({id}, {lastBackupReturn.FileCount}, {lastBackupReturn.FailedFileCount}, {lastBackupReturn.DirCount}, '{lastBackupReturn.ExceptionType}', {(int) lastBackupReturn.Flags}, '{lastBackupFile.Replace("'", "''")}', '{LastBackup:yyyy-MM-dd HH':'mm':'ss.fff}'); ";
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
                Process[] processlist = Process.GetProcesses();
                bool processFound = false;
                List<Process> processKillList = new List<Process>();
                foreach (Process process in processlist)
                {
                    try
                    {
                        string processFileName = process.Modules[0].FileName;
                        if (processFileName.ToUpper() == closeProcess.ToUpper())
                        {
                            processKillList.Add(process);
                        }
                    }
                    catch
                    {
                        // ignored..
                    }
                }


                foreach (Process process in processKillList)
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
                        closed = true;
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
                    Process.Start(ProcessFile);
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
                string sql = string.Join(Environment.NewLine,
                    "INSERT INTO BACKUP (NAME, LEAVEFILESMAX, BACKUP_FROMDIR, BACKUP_TODIR, ",
                    "BACKUP_FILENAME, REFTIME, PROCESS, KILLSPAN_SECONDS, RETRYHOURS, ALLOWLOCKEDFILES) ",
                    $"VALUES('{name.Replace("'", "''")}',",
                    $"{numbersSave},",
                    $"'{backupDirFrom.Replace("'", "''")}',",
                    $"'{backupDirTo.Replace("'", "''")}',",
                    $"'{backupFile.Replace("'", "''")}',",
                    $"'{DateTime.Now:yyyy-MM-dd HH':'mm':'ss.fff}',",
                    $"{(closeProcess == string.Empty ? "NULL" : "'" + closeProcess.Replace("'", "''") + "'")},",
                    $"{(killProcessSeconds == 0 ? "NULL" : killProcessSeconds.ToString())}, ",
                    $"{retryHours}, ",
                    $"{(AllowLockedFiles ? "1" : "0")}) ");

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
                    sql += $"INSERT INTO WEEKDAYS(ID, WEEKDAY) VALUES({id}, {insert});";
                }

                foreach (int insert in months)
                {
                    sql += $"INSERT INTO MONTHS(ID, MONTH) VALUES({id}, {insert}); ";
                }

                foreach (int insert in days)
                {
                    sql += $"INSERT INTO DAYS(ID, DAY) VALUES({id}, {insert}); ";
                }

                foreach (int insert in hours)
                {
                    sql += $"INSERT INTO HOURS(ID, HOUR) VALUES({id}, {insert}); ";
                }

                foreach (int insert in minutes)
                {
                    sql += $"INSERT INTO MINUTES(ID, MINUTE) VALUES({id}, {insert}); ";
                }

                sql += $"INSERT INTO CRONS(ID, CRON) VALUES({id}, '{Cron}'); ";

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
            while (lastBackups.Count > SaveBackupsNo)
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

        private static DateTime FormulateDateTimeIso8601(string value)
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
            get => FormulateDateTimeIso8601(lastBackup);

            set => lastBackup = value.ToString("yyyy-MM-dd HH':'mm':'ss.fff");
        }

        public void GenNextTime()
        {
            if (DateTime.Now < NextBackup)
            {
                return;
            }
            if (Cron != CronEmpty)
            {
                while (NextBackup < DateTime.Now)
                {
                    NextBackup = TimeCalculations.ConstructDate(1, Cron, NextBackup)[0];
                }
            }
            else
            {
                while (NextBackup < DateTime.Now)
                {
                    NextBackup = TimeCalculations.ConstructDate(1, weekDays, months, days, hours, minutes, NextBackup)[0];
                }
            }
        }


        public DateTime NextBackup
        {
            get
            {
                if (nextBackup != string.Empty)
                {
                    DateTime dt = FormulateDateTimeIso8601(nextBackup);
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

                    if (Cron != CronEmpty)
                    {
                        return TimeCalculations.ConstructDate(1, Cron, LastBackup)[0];
                    }
                    else
                    {
                        return TimeCalculations.ConstructDate(1, weekDays, months, days, hours, minutes, LastBackup)[0];
                    }
                }
                else if (lastBackup == string.Empty)
                {
                    DateTime dt = FormulateDateTimeIso8601(refBackup);
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
                    if (Cron != CronEmpty)
                    {
                        return TimeCalculations.ConstructDate(1, Cron)[0];
                    }
                    {
                        return TimeCalculations.ConstructDate(1, weekDays, months, days, hours, minutes)[0];
                    }
                }
                else
                {
                    DateTime dt = FormulateDateTimeIso8601(nextBackup);
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

            set => nextBackup = value.ToString("yyyy-MM-dd HH':'mm':'ss.fff");
        }

        public DateTime RefDate
        {
            get => FormulateDateTimeIso8601(refBackup);

            set => refBackup = value.ToString("yyyy-MM-dd HH':'mm':'ss.fff");
        }

        public int KillProcessSeconds
        {
            get => killProcessSeconds < 10 ? 10: killProcessSeconds;

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
            get => killProcessSeconds >= 10;

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
            get => closeProcess;

            set => closeProcess = value;
        }


        public int FailCount { get; set; }

        public DateTime LastFail
        {
            get => FormulateDateTimeIso8601(lastFail);

            set => lastFail = value.ToString("yyyy-MM-dd HH':'mm':'ss.fff");
        }

        public object ID
        {
            get => id;

            set => id = value;
        }

        public string Name
        {
            get => name;

            set => name = value;
        }

        public string BackupFile
        {
            get => backupFile;

            set => backupFile = value;
        }

        public string BackupFileFormatted => FormatBackupFilename(backupFile);

        public static string FormatBackupFilename(string fileName)
        {
            string result = fileName;
            if (!result.ToUpper().EndsWith(".zip".ToUpper()))
            {
                result = result + ".zip";
            }

            DateTime dt = DateTime.Now;
            result = result.Replace("%Y", dt.Year.ToString());
            result = result.Replace("%M", dt.Month.ToString().PadLeft(2, '0'));
            result = result.Replace("%D", dt.Day.ToString().PadLeft(2, '0'));
            result = result.Replace("%H", dt.Hour.ToString().PadLeft(2, '0'));
            result = result.Replace("%m", dt.Minute.ToString().PadLeft(2, '0'));
            result = result.Replace("%S", dt.Second.ToString().PadLeft(2, '0'));
            return result;
        }

        public string DirFrom
        {
            get => backupDirFrom.TrimEnd('\\');

            set => backupDirFrom = value;
        }

        public string DirTo
        {
            get => backupDirTo.TrimEnd('\\');

            set => backupDirTo = value;
        }

        public int SaveBackupsNo
        {
            get => numbersSave;

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
                numbersSave = value;
            }
        }

        public List<LastBackupEntry> LastBackups
        {
            get => lastBackups;

            set => lastBackups = value;
        }


        public List<int> WeekDays
        {
            get => weekDays;

            set => weekDays = value;
        }


        public List<int> Months
        {
            get => months;

            set => months = value;
        }

        public List<int> Days
        {
            get => days;

            set => days = value;
        }

        public List<int> Hours
        {
            get => hours;

            set => hours = value;
        }

        public List<int> Minutes
        {
            get => minutes;

            set => minutes = value;
        }

    }
}
