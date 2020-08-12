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
using System.Data.SQLite;
using System.IO;

namespace SimpleBackup
{
    public class ScriptRunner
    {
        private class DbScriptBlock
        {
            public List<string> SQLBlock = new List<string>();
            public int DbVer;
        }

        public static bool RunScript(string sqliteDatasource)
        {
            try
            {
                List<DbScriptBlock> sqlBlocks = new List<DbScriptBlock>();
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" +sqliteDatasource + ";Pooling=true;FailIfMissing=false"))
                {
                    conn.Open();


                    int dbVer = 0;
                    string line;

                    using (StreamReader sr = new StreamReader(Program.AppInstallDir + "script.sql_script"))
                    {
                        while (!sr.EndOfStream)
                        {
                            while (!(line = sr.ReadLine()).StartsWith("--VER " + dbVer))
                            {
                            }

                            DbScriptBlock scriptBlock = new DbScriptBlock();
                            scriptBlock.DbVer = Convert.ToInt32(line.Split(' ')[1]);

                            while (!(line = sr.ReadLine()).EndsWith("--ENDVER " + dbVer))
                            {
                                scriptBlock.SQLBlock.Add(line);
                            }

                            dbVer++;
                            sqlBlocks.Add(scriptBlock);
                        }
                    }

                    int dbVersion;
                    using (SQLiteCommand command = new SQLiteCommand(conn))
                    {
                        try 
                        {
                            command.CommandText = "SELECT MAX(DBVERSION) AS VER FROM DBVERSION; ";
                            using (SQLiteDataReader dr = command.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    dbVersion = dr.GetInt32(0) + 1;
                                }
                                else
                                {
                                    dbVersion = 0;
                                }
                            }
                        }
                        catch 
                        {
                            dbVersion = 0;
                        }
                    }

                    for (int i = dbVersion; i < sqlBlocks.Count; i++)
                    {
                        string exec = string.Empty;
                        foreach (string sqLine in sqlBlocks[i].SQLBlock)
                        {
                            exec += sqLine + Environment.NewLine;
                        }
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand(conn))
                            {
                                command.CommandText = exec;
                                command.ExecuteNonQuery();
                            }
                        }
                        catch
                        {
                            // ignored..
                        }

                        exec =  "INSERT INTO DBVERSION(DBVERSION) " + Environment.NewLine +
                                "SELECT " + sqlBlocks[i].DbVer + " " + Environment.NewLine +
                                "WHERE NOT EXISTS(SELECT 1 FROM DBVERSION WHERE DBVERSION = " + sqlBlocks[i].DbVer + "); " + Environment.NewLine;
                        using (SQLiteCommand command = new SQLiteCommand(conn))
                        {
                            command.CommandText = exec;
                            command.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
