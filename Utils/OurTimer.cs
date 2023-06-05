using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Form1.Utils
{
    class OurTimer
    {
        private int startAt, s;
        private Label referenceLabel;
        private DispatcherTimer timer;
        private Func<string, DataTable> getdtfunc;
        private DataGrid referenceGrid;
        private string rows;

        public OurTimer(int startAt, Label referenceLabel, DataGrid referenceGrid, Func<string, DataTable> getdtfunc, string rows)
        {
            this.startAt = startAt;
            this.referenceLabel = referenceLabel;
            this.referenceGrid = referenceGrid;
            this.getdtfunc = getdtfunc;
            this.rows = rows;

            s = startAt;
            timer = new DispatcherTimer
            {
                //  Ticking occurs every 1 second
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (s > 0)
            {
                s--;
                referenceLabel.Content = s;
            }
            else
            {
                RefreshTimer();
                UpdateDataGrid();
            }
        }
        
        public void StartTimer() { timer.Start(); }

        public void StopTimer() { timer.Stop(); }

        public void RefreshTimer() { s = startAt; }

        public void UpdateLines(string rows) { this.rows = rows; }

        public void UpdateDataGrid() { referenceGrid.ItemsSource = getdtfunc(rows).DefaultView; }
    }
}
