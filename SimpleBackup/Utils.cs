#region License
/*
Just utilities.
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
using System.Threading;

namespace SimpleBackup
{
    public class Utils
    {
        private static List<Mutex> mutexes = new List<Mutex>();
        public static bool CheckIfRunning(string uniqueID)
        {
            Mutex mutex;

            try
            {
                mutex = Mutex.OpenExisting(uniqueID);
                if (mutex != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                mutex = new Mutex(true, uniqueID);
                mutexes.Add(mutex);
                return false;
            }
        }

        public static bool ValidFileName(string fileName)
        {
            try 
            {
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
