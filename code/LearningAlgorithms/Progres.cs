using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace LearningAlgorithms
{
    public partial class Progres : Form
    {
        Stopwatch stopWatch;
        TimeSpan ts;
        bool start_ = false;
        public Progres()
        {
            InitializeComponent();
        }
        public ProgressBar get_pg()
        {
            return progressBar1;
        }
        public Label get_lb()
        {
            return label2;
        }
        public void start(Stopwatch st, int step)
        {
            long i = st.Elapsed.Ticks;
            ts = new TimeSpan(i*step);
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                   ts.Hours, ts.Minutes, ts.Seconds);
            label2.Text = elapsedTime.ToString();
            //label2.Invoke(Refresh());
          //  start_ = true;
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (start_)
            {
                ts = new TimeSpan(ts.Ticks - 100);
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                    ts.Hours, ts.Minutes, ts.Seconds);
                label2.Text = elapsedTime.ToString();
                label2.Refresh();
            }
           
        }        
    }
}
