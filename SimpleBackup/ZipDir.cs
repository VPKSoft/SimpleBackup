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
using System.IO;
using Ionic.Zip;
using VPKSoft.LangLib;

namespace SimpleBackup
{
    public static class ZipDir
    {
        [Flags] public enum ZipReturnFlags
        {
            Success = 0,                                
            LockedFile = 1,                                
            Other = 2,                                
            UnknownException = 4, 
            PreviousFileCantBeDeleted = 8,
            InvalidFileName = 16,
            SameHash = 32
        };


        public class ZipReturn
        {
            public int FileCount;
            public int FailedFileCount;
            public int DirCount;
            public Type ExceptionType = new object().GetType();
            public ZipReturnFlags Flags = ZipReturnFlags.Success;

            public string StatusString
            {
                get
                {
                    string result = string.Empty;
                    
                    result += FailedFileCount > 0 ? DBLangEngine.GetStatMessage("msgFilesFailed", "Files failed: {0}{1}|How many files failed during backup operation", FailedFileCount, Environment.NewLine) : string.Empty;
                    result += (FailedFileCount + FileCount) > 0 ? DBLangEngine.GetStatMessage("msgFilesSucceeded", "Files succeeded: {0}{1}|How many files succeeded during backup operation", FileCount, Environment.NewLine) : string.Empty;
                    result += (FailedFileCount + DirCount) > 0 ? DBLangEngine.GetStatMessage("msgDirectoriesSucceeded", "Directories succeeded: {0}{1}|How many directories succeeded during backup operation", DirCount, Environment.NewLine) : string.Empty;
                    result += (FailedFileCount + FileCount) > 0 ? DBLangEngine.GetStatMessage("msgFileFailRatio", "File fail ratio: {0} %|How many files in percentage failed compared to the total file amount in a backup operation",
                        (FileCount > 0 ? (FailedFileCount / (double)FileCount * 100.0).ToString("F2") : (100.0).ToString("F2")) + 
                        Environment.NewLine) : string.Empty;
                    result += Flags.HasFlag(ZipReturnFlags.SameHash) ? DBLangEngine.GetStatMessage("msgDirNotChanged", "Backup directory hasn't changed.|As in the hash of the directory is the same.") : string.Empty;

                    result = result == string.Empty ? "OK" : result;
                    return result;
                }
            }

            public bool Empty =>
                FileCount == 0 &&
                FailedFileCount == 0 &&
                DirCount == 0 &&
                ExceptionType == new object().GetType() &&
                Flags == ZipReturnFlags.Success;
        }

        public static bool FileLockedRead(string fileName)
        {
            try
            {
                using (new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {

                }
                return false;
            } 
            catch
            {
                return true;
            }
        }

        public static ZipReturn Compress(string dir, string file, bool allowFileLocks)
        {
            ZipReturn result = new ZipReturn();
            if (!Utils.ValidFileName(Path.GetFileName(file)))
            {
                result.Flags = ZipReturnFlags.InvalidFileName;
                return result;
            }

            string tmpFile = file;
            file = file + ".tmp_$$$";

            if (!Utils.ValidFileName(Path.GetFileName(file)))
            {
                result.Flags = ZipReturnFlags.InvalidFileName;
                return result;
            }

            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {
                result.Flags = ZipReturnFlags.PreviousFileCantBeDeleted;
                return result;
            }

            try
            {
                string[] files = Directory.GetFileSystemEntries(dir + "\\", "*.*", SearchOption.AllDirectories).ToArray();
                List<string> dirs = new List<string>();
                using (ZipFile zip = new ZipFile())
                {
                    foreach (string fileName in files)
                    {
                        var directory = Directory.Exists(fileName)
                            ? fileName.Replace(dir, string.Empty).TrimStart('\\')
                            : Path.GetDirectoryName(fileName)?.Replace(dir, string.Empty).TrimStart('\\');

                        if (Directory.Exists(fileName) && directory != string.Empty)
                        {
                            zip.AddDirectoryByName(directory);
                            if (!dirs.Contains(directory))
                            {
                                dirs.Add(directory);
                                result.DirCount++;
                            }
                        }
                        else
                        {
                            if (FileLockedRead(fileName))
                            {
                                if (allowFileLocks)
                                {
                                    continue;
                                }

                                result.Flags |= ZipReturnFlags.LockedFile;
                                result.FailedFileCount++;
                            }
                            else
                            {
                                if (!dirs.Contains(directory))
                                {
                                    dirs.Add(directory);
                                    result.DirCount++;
                                }
                                zip.AddFile(fileName, directory);
                                result.FileCount++;
                            }
                        }
                    }
                    // DateTime dt1 = DateTime.Now;
                    zip.Save(file);
                    /*
                    if ((DateTime.Now - dt1).TotalSeconds > 2)
                    {
                        DebugCall();
                    }
                    */
                }
                result.Flags |= ZipReturnFlags.Success;
            }
            catch (Exception ex)
            {
                result.ExceptionType = ex.GetType();
                result.Flags |= ZipReturnFlags.UnknownException;
            }

            if (result.Flags == ZipReturnFlags.Success)
            {
                try
                {
                    if (File.Exists(tmpFile))
                    {
                        File.Delete(tmpFile);
                    }
                    File.Move(file, tmpFile);
                }
                catch
                {
                    result.Flags |= ZipReturnFlags.PreviousFileCantBeDeleted;
                }
            }
            else
            {
                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                    // ignored..
                }
            }

            return result;
        }
    }
}
