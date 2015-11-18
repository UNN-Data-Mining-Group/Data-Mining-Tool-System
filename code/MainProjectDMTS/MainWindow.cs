using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainProjectDMTS
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnDataBase_Click(object sender, EventArgs e)
        {
            SII.TaskForm taskForm = new SII.TaskForm();
            taskForm.Show();
        }

        private void btnNeuroWnd_Click(object sender, EventArgs e)
        {
            NeuroWnd.NeuroNetsMainWindow neuroNetsWindow = new NeuroWnd.NeuroNetsMainWindow();
            neuroNetsWindow.Show();
        }

        private void btnDesisionTrees_Click(object sender, EventArgs e)
        {
            DesisionTrees.DesisionTreeMainWindow treesWindow = new DesisionTrees.DesisionTreeMainWindow();
            treesWindow.Show();
        }
    }
}
