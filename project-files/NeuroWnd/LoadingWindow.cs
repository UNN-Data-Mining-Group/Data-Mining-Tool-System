using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace NeuroWnd
{
    public partial class LoadingWindow : Form
    {
        public LoadingWindow()
        {
            InitializeComponent();
        }
        public ProgressBar LoadingBar { get { return progressBar; } }
        public void MakeLoading(Action loadingFunction, string loadingCaption)
        {
            Thread loadingThread = new Thread(new ThreadStart(loadingFunction));
            loadingThread.Start();
            Text = loadingCaption;
            ShowDialog();
        }
    }
}
