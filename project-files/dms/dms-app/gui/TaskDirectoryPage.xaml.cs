using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using dms.view_models;

namespace dms.gui
{
    /// <summary>
    /// Interaction logic for TaskDirectoryPage.xaml
    /// </summary>
    public partial class TaskDirectoryPage : UserControl
    {
        TaskTreeViewModel taskVM = new TaskTreeViewModel();
        public TaskDirectoryPage()
        {
            InitializeComponent();

            this.DataContext = taskVM;
            taskVM.requestTaskCreation += TaskVM_requestTaskCreation;
        }

        private void TaskVM_requestTaskCreation(object sender, EventArgs<TaskCreationViewModel> e)
        {
            TaskCreationWindow t = new TaskCreationWindow(e.Data);
            t.ShowDialog();
        }
    }
}
