#region License
/*
An utility to zip a directory using DotNetZip Library 
(http://dotnetzip.codeplex.com).

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
            public int FileCount = 0;
            public int FailedFileCount = 0;
            public int DirCount = 0;
            public Type ExceptionType = new object().GetType();
            public ZipReturnFlags Flags = ZipReturnFlags.Success;
            public bool Success
            {
                get
                {
                    return  Flags == ZipReturnFlags.Success && 
                                     ExceptionType == (new object()).GetType() &&
                                     FailedFileCount == 0 &&
                                     FileCount > 0;
                }
            }

            public string StatusString
            {
                get
                {
                    string retval = string.Empty;
                    
                    retval += FailedFileCount > 0 ? DBLangEngine.GetStatMessage("msgFilesFailed", "Files failed: {0}{1}|How many files failed during backup operation", FailedFileCount, Environment.NewLine) : string.Empty;
                    retval += (FailedFileCount + FileCount) > 0 ? DBLangEngine.GetStatMessage("msgFilesSucceeded", "Files succeeded: {0}{1}|How many files succeeded during backup operation", FileCount, Environment.NewLine) : string.Empty;
                    retval += (FailedFileCount + DirCount) > 0 ? DBLangEngine.GetStatMessage("msgDirectoriesSucceeded", "Directories succeeded: {0}{1}|How many directories succeeded during backup operation", DirCount, Environment.NewLine) : string.Empty;
                    retval += (FailedFileCount + FileCount) > 0 ? DBLangEngine.GetStatMessage("msgFileFailRatio", "File fail ratio: {0} %|How many files in percentage failed compared to the total file amount in a backup operation",
                        (FileCount > 0 ? ((double)FailedFileCount / (double)FileCount * 100.0).ToString("F2") : (100.0).ToString("F2")) + 
                        Environment.NewLine) : string.Empty;
                    retval += Flags.HasFlag(ZipReturnFlags.SameHash) ? DBLangEngine.GetStatMessage("msgDirNotChanged", "Backup directory hasn't changed.|As in the hash of the directory is the same.") : string.Empty;

                    retval = retval == string.Empty ? "OK" : retval;
                    return retval;
                }
            }

            public bool Empty
            {
                get
                {
                    return FileCount == 0 &&
                           FailedFileCount == 0 &&
                           DirCount == 0 &&
                           ExceptionType == new object().GetType() &&
                           Flags == ZipReturnFlags.Success;
                }
            }
        }

        public static bool FileLockedRead(string fileName)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) { }
//                using (File.OpenRead(fileName)) { } this will cause an exception..
                return false;
            } 
            catch
            {
                return true;
            }
        }

        private static void DebugCall()
        {

        }

        public static ZipReturn Compress(string dir, string file)
        {
            ZipReturn retval = new ZipReturn();
            if (!Utils.ValidFileName(Path.GetFileName(file)))
            {
                retval.Flags = ZipReturnFlags.InvalidFileName;
                return retval;
            }

            string tmpFile = file;
            file = file + ".tmp_$$$";

            if (!Utils.ValidFileName(Path.GetFileName(file)))
            {
                retval.Flags = ZipReturnFlags.InvalidFileName;
                return retval;
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
                retval.Flags = ZipReturnFlags.PreviousFileCantBeDeleted;
                return retval;
            }

            try
            {
                string[] files = Directory.GetFileSystemEntries(dir + "\\", "*.*", SearchOption.AllDirectories).ToArray();
                List<string> dirs = new List<string>();
                using (ZipFile zip = new ZipFile())
                {
                    string directory;
                    foreach (string fname in files)
                    {
                        if (Directory.Exists(fname))
                        {
                            directory = fname.Replace(dir, string.Empty).TrimStart('\\');
                        }
                        else
                        {
                            directory = Path.GetDirectoryName(fname).Replace(dir, string.Empty).TrimStart('\\');
                        }

                        if (Directory.Exists(fname) && directory != string.Empty)
                        {
                            zip.AddDirectoryByName(directory);
                            if (!dirs.Contains(directory))
                            {
                                dirs.Add(directory);
                                retval.DirCount++;
                            }
                        }
                        else
                        {
                            if (FileLockedRead(fname))
                            {
                                retval.Flags |= ZipReturnFlags.LockedFile;
                                retval.FailedFileCount++;
                            }
                            else
                            {
                                if (!dirs.Contains(directory))
                                {
                                    dirs.Add(directory);
                                    retval.DirCount++;
                                }
                                zip.AddFile(fname, directory);
                                retval.FileCount++;
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
                retval.Flags |= ZipReturnFlags.Success;
            }
            catch (Exception ex)
            {
                retval.ExceptionType = ex.GetType();
                retval.Flags |= ZipReturnFlags.UnknownException;
            }

            if (retval.Flags == ZipReturnFlags.Success)
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
                    retval.Flags |= ZipReturnFlags.PreviousFileCantBeDeleted;
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

                }
            }

            return retval;
        }
    }
}
