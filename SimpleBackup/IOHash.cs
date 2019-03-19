#region License
/*
An utility for MD5 hashes.
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
using System.Security.Cryptography;

namespace VPKSoft.Hashes
{
    /// <summary>
    /// Utilities for MD5 hash calculations.
    /// </summary>
    public class IOHash
    {
        /// <summary>
        /// Appends (TransformBlock) a stream to a MD5 instance.
        /// </summary>
        /// <param name="s">The stream to append.</param>
        /// <param name="md5">A reference to a MD5 instance to append to.</param>
        public static void MD5AppenStream(Stream s, ref MD5 md5)
        {
            byte [] buffer = new byte[1000000]; // go with 1 MB
            s.Position = 0;
            int pos = 0, count;

            while ((count = s.Read(buffer, pos, 1000000)) > 0)
            {
                md5.TransformBlock(buffer, 0, count, buffer, 0);
            }
        }

        /// <summary>
        /// Appends (TransformBlock) a variable array of bytes to a MD5 instance.
        /// </summary>
        /// <param name="bytes">A variable array of bytes to append.</param>
        /// <param name="md5">A reference to a MD5 instance to append to.</param>
        public static void MD5AppendBytes(byte [] bytes, ref MD5 md5)
        {
            md5.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }

        /// <summary>
        /// Appends (TransformBlock) a file to an MD5 instance.
        /// </summary>
        /// <param name="fileName">The name of the file to append.</param>
        /// <param name="md5">A reference to a MD5 instance to append to.</param>
        public static void MD5AppendFile(string fileName, ref MD5 md5)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                MD5AppenStream(fs, ref md5);
            }
        }

        /// <summary>
        /// Appends (TransformBlock) a string in Unicode encoding to a MD5 instance.
        /// </summary>
        /// <param name="s">String to append.</param>
        /// <param name="md5">A reference to a MD5 instance to append to.</param>
        public static void MD5AppendString(string s, ref MD5 md5)
        {
            byte[] buffer = System.Text.Encoding.Unicode.GetBytes(s.ToCharArray());
            md5.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
        }

        /// <summary>
        /// Finalizes (TransformFinalBlock) a MD5 instance with a zero-sized buffer.
        /// </summary>
        /// <param name="md5">A reference to a MD5 instance to finalize.</param>
        public static void MD5FinalizeBlock(ref MD5 md5)
        {
            md5.TransformFinalBlock(new byte[0], 0, 0);
        }

        /// <summary>
        /// Finalizes (TransformFinalBlock) a MD5 instance with a zero-sized buffer
        /// and converts the hash into a hexadecimal string representation.
        /// </summary>
        /// <param name="md5">A reference to a MD5 instance to finalize.</param>
        /// <returns>A hexadecimal string representation of the MD5 hash.</returns>
        public static string MD5GetHashString(ref MD5 md5)
        {
            MD5FinalizeBlock(ref md5);
            return "0x" + BitConverter.ToString(md5.Hash, 0).Replace("-", "");
        }

        /// <summary>
        /// Enumerates an entire directory recursively and calculates a MD5
        /// hash of its files, file names and directory names.
        /// </summary>
        /// <param name="dir">A directory to enumerate.</param>
        /// <param name="ignoreUnreadableFiles">If true causes the method to ignore locked files.</param>
        /// <returns>
        /// A hexadecimal string representation of the MD5 hash
        /// or an empty string if the operation failed.
        /// </returns>
        public static string MD5HashDirSimple(string dir, bool ignoreUnreadableFiles = false)
        {
            try
            {
                MD5 md5 = MD5.Create();
                string[] fileArray = Directory.GetFileSystemEntries(dir + "\\", "*.*", SearchOption.AllDirectories).ToArray();
                List<string> files = new List<string>();
                files.AddRange(fileArray);
                files.Sort();
                List<string> dirs = new List<string>();
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
                        if (!dirs.Contains(directory))
                        {
                            MD5AppendString(directory, ref md5);
                            dirs.Add(directory);
                        }
                    }
                    else
                    {
                        if (!dirs.Contains(directory))
                        {
                            MD5AppendString(directory, ref md5);
                            dirs.Add(directory);
                        }
                        MD5AppendString(fname, ref md5);
                        if (ignoreUnreadableFiles)
                        {
                            try
                            {
                                MD5AppendFile(fname, ref md5);
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            MD5AppendFile(fname, ref md5);
                        }
                    }
                }
                return MD5GetHashString(ref md5);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
