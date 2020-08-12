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

using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace SimpleBackup
{
    public class Utils
    {
        // ReSharper disable once CollectionNeverQueried.Local
        private static readonly List<Mutex> Mutexes = new List<Mutex>();
        public static bool CheckIfRunning(string uniqueId)
        {
            try
            {
                Mutex.OpenExisting(uniqueId);
                return true;
            }
            catch
            {
                var mutex = new Mutex(true, uniqueId);
                Mutexes.Add(mutex);
                return false;
            }
        }

        public static bool ValidFileName(string fileName)
        {
            try 
            {
                // ReSharper disable once ObjectCreationAsStatement
                new FileInfo(fileName);
            }
            catch
            {
                return false;
            }

            if (fileName.Contains('\\') || 
                fileName.Contains('/') || 
                fileName.Contains(':') ||
                fileName.Contains('*') ||
                fileName.Contains('?') ||
                fileName.Contains('"') ||
                fileName.Contains('<') ||
                fileName.Contains('>') ||
                fileName.Contains('|'))
            {
                return false;
            }
            return true;
        }
    }
}
