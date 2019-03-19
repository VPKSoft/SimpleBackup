#region License
/*
A "timer" which based on the BackroundWorker class.
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
using System.ComponentModel;
using System.Threading;


namespace SimpleBackup
{
    public class BackgroundTimer
    {
        BackgroundWorker bgTimer = new BackgroundWorker();
        private int resolution = 20;
        private int interval = 1000;
        private double rounds = 50;
        private bool enabled = false;
        private bool stopped = false;

        private object _lock = new object();

        public bool Enabled
        {
            get
            {
                lock (_lock)
                {
                    return enabled;
                }
            }

            set
            {
                lock (_lock)
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
                lock (_lock)
                {
                    return stopped;
                }
            }

            set
            {
                lock (_lock)
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
                lock (_lock)
                {
                    return resolution;
                }
            }

            set
            {
                lock (_lock)
                {

                    if (value <= 0 || value > interval)
                    {
                        throw new BackgroundTimerException("Invalid resolution.");
                    }
                    else
                    {
                        rounds = (double)interval / (double)value;
                        resolution = value;
                    }
                }
            }
        }

        public int Interval
        {
            get
            {
                lock (_lock)
                {
                    return interval;
                }
            }

            set
            {
                lock (_lock)
                {
                    if (value <= 0 || value < interval)
                    {
                        throw new BackgroundTimerException("Invalid interval.");
                    }
                    else
                    {
                        rounds = (double)value / (double)resolution;
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
                if (TimerElapsed != null)
                {
                    TimerElapsed(this, EventArgs.Empty);
                }
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

        public event Elapsed TimerElapsed = null;
    }
}
