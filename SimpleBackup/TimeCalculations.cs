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
using NCrontab;

namespace SimpleBackup
{
    class TimeCalculations
    {
        public static List<DateTime> ConstructDate(int occurenceCnt, int weekDay, int month, int day, int hour, int minute)
        {
            List<DateTime> result = new List<DateTime>();
            if (weekDay == -1 &&
                month == -1 &&
                day == -1 &&
                hour == -1 &&
                minute == -1)
            {
                throw new Exception("ConstructDate: All selected!");
            }
            //            * * * * *
            //            - - - - -
            //            | | | | |
            //            | | | | +----- day of week (0 - 6) (Sunday=0)
            //            | | | +------- month (1 - 12)
            //            | | +--------- day of month (1 - 31)
            //            | +----------- hour (0 - 23)
            //            +------------- min (0 - 59)


            string parseString = (minute == -1 ? "*" : minute.ToString()) + " " +
                                  (hour == -1 ? "*" : hour.ToString()) + " " +
                                  (day == -1 ? "*" : day.ToString()) + " " +
                                  (month == -1 ? "*" : month.ToString()) + " " +
                                  (weekDay == -1 ? "*" : weekDay.ToString());
            CrontabSchedule s = CrontabSchedule.Parse(parseString);

            DateTime dt = DateTime.Now;
            for (int i = 0; i < occurenceCnt; i++)
            {
                dt = s.GetNextOccurrence(dt);
                result.Add(dt);
            }

            return result;
        }

        public static DateTime ConstructDate(int weekDay, int month, int day, int hour, int minute)
        {
            return ConstructDate(1, weekDay, month, day, hour, minute)[0];
        }

        public static List<DateTime> ConstructDate(int occurenceCnt, List<int> weekDays, List<int> months, List<int> days, List<int> hours, List<int> minutes)
        {
            return ConstructDate(occurenceCnt, weekDays, months, days, hours, minutes, DateTime.Now);
        }

        public static List<DateTime> ConstructDate(int occurenceCnt, List<int> weekDays, List<int> months, List<int> days, List<int> hours, List<int> minutes, DateTime dt)
        {
            List<DateTime> result = new List<DateTime>();
            weekDays.Sort();
            months.Sort();
            days.Sort();
            hours.Sort();
            minutes.Sort();
            string cron = string.Empty;
            if (minutes.Count == 0 || minutes.IndexOf(-1) >= 0)
            {
                cron += "* ";
            }
            else
            {
                for (int i = 0; i < minutes.Count; i++)
                {
                    cron += minutes[i].ToString() + ",";
                }
            }
            cron = cron.TrimEnd(',') + " ";

            if (hours.Count == 0 || hours.IndexOf(-1) >= 0)
            {
                cron += "* ";
            }
            else
            {
                for (int i = 0; i < hours.Count; i++)
                {
                    cron += hours[i].ToString() + ",";
                }
            }
            cron = cron.TrimEnd(',') + " ";

            if (days.Count == 0 || days.IndexOf(-1) >= 0)
            {
                cron += "* ";
            }
            else
            {
                for (int i = 0; i < days.Count; i++)
                {
                    cron += days[i].ToString() + ",";
                }
            }
            cron = cron.TrimEnd(',') + " ";

            if (months.Count == 0 || months.IndexOf(-1) >= 0)
            {
                cron += "* ";
            }
            else
            {
                for (int i = 0; i < months.Count; i++)
                {
                    cron += months[i].ToString() + ",";
                }
            }
            cron = cron.TrimEnd(',') + " ";

            if (weekDays.Count == 0 || weekDays.IndexOf(-1) >= 0)
            {
                cron += "* ";
            }
            else
            {
                for (int i = 0; i < weekDays.Count; i++)
                {
                    cron += weekDays[i].ToString() + ",";
                }
            }
            cron = cron.TrimEnd(',', ' ');

            cron = cron.Replace("  ", " ");

            if (cron == "* * * * *")
            {
                throw new Exception("ConstructDate: All selected!");
            }

            CrontabSchedule s = CrontabSchedule.Parse(cron);
            for (int i = 0; i < occurenceCnt; i++)
            {
                dt = s.GetNextOccurrence(dt);
                result.Add(dt);
            }
            return result;
        }

        public static List<DateTime> ConstructDate(int occurenceCnt, string cron, DateTime dt)
        {
            List<DateTime> result = new List<DateTime>();
            if (cron == "* * * * *")
            {
                throw new Exception("ConstructDate: All selected!");
            }

            CrontabSchedule s = CrontabSchedule.Parse(cron);
            for (int i = 0; i < occurenceCnt; i++)
            {
                dt = s.GetNextOccurrence(dt);
                result.Add(dt);
            }
            return result;
        }

        public static List<DateTime> ConstructDate(int occurenceCnt, string cron)
        {
            return ConstructDate(occurenceCnt, cron, DateTime.Now);
        }

        public static DateTime ConstructDate(List<int> weekDays, List<int> months, List<int> days, List<int> hours, List<int> minutes, DateTime dt)
        {
            return ConstructDate(1, weekDays, months, days, hours, minutes, dt)[0];
        }

        public static DateTime ConstructDate(List<int> weekDays, List<int> months, List<int> days, List<int> hours, List<int> minutes)
        {
            return ConstructDate(1, weekDays, months, days, hours, minutes, DateTime.Now)[0];
        }

    }
}
