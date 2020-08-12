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
using System.ComponentModel;
using System.Threading;


namespace SimpleBackup
{
    public class BackgroundTimer
    {
        readonly BackgroundWorker bgTimer = new BackgroundWorker();
        private int resolution = 20;
        private int interval = 1000;
        private double rounds = 50;
        private bool enabled;
        private bool stopped;

        private readonly object @lock = new object();

        public bool Enabled
        {
            get
            {
                lock (@lock)
                {
                    return enabled;
                }
            }

            set
            {
                lock (@lock)
                {
                    enabled = value;
                    if (value && !bgTimer.IsBusy)
                    {
                        bgTimer.RunWorkerAsync();
                    }
                }
            }
        }

        public bool Stopped
        {
            get
            {
                lock (@lock)
                {
                    return stopped;
                }
            }

            set
            {
                lock (@lock)
                {
                    if (value && !bgTimer.IsBusy && enabled)
                    {
                        bgTimer.RunWorkerAsync();
                    }
                    stopped = value;
                }
            }
        }

        public class BackgroundTimerException: Exception
        {
            public BackgroundTimerException(string message): base(message)
            {

            }
        }

        public int Resolution
        {
            get
            {
                lock (@lock)
                {
                    return resolution;
                }
            }

            set
            {
                lock (@lock)
                {

                    if (value <= 0 || value > interval)
                    {
                        throw new BackgroundTimerException("Invalid resolution.");
                    }
                    else
                    {
                        rounds = interval / (double)value;
                        resolution = value;
                    }
                }
            }
        }

        public int Interval
        {
            get
            {
                lock (@lock)
                {
                    return interval;
                }
            }

            set
            {
                lock (@lock)
                {
                    if (value <= 0 || value < interval)
                    {
                        throw new BackgroundTimerException("Invalid interval.");
                    }
                    else
                    {
                        rounds = value / (double)resolution;
                        interval = value;
                    }
                }
            }
        }
    
        public bool IsBusy()
        {
            return bgTimer.IsBusy;
        }

        ~BackgroundTimer()
        {
            Stopped = true;
            Enabled = false;
            while (IsBusy())
            {
                Thread.Sleep(1000);
            }
        }

        public BackgroundTimer()
        {
            bgTimer.DoWork += bgTimer_DoWork;
            bgTimer.RunWorkerCompleted += bgTimer_RunWorkerCompleted;
        }

        public BackgroundTimer(int interval)
        {
            Interval = interval;
            bgTimer.DoWork += bgTimer_DoWork;
            bgTimer.RunWorkerCompleted += bgTimer_RunWorkerCompleted;
        }

        public void Start()
        {
            Enabled = true;
        }

        void bgTimer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (enabled)
            {
                TimerElapsed?.Invoke(this, EventArgs.Empty);
                bgTimer.RunWorkerAsync();
            }
        }


        void bgTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (stopped || !enabled)
            {
                return;
            }
            for (double d = 0; d < rounds; d++)
            {
                if (stopped || !enabled)
                {
                    return;
                }
                Thread.Sleep(resolution);
            }
        }

        public delegate void Elapsed(object sender, EventArgs e);

        public event Elapsed TimerElapsed;
    }
}
