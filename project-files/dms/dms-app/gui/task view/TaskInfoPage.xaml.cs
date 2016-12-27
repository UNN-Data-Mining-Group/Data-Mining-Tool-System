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

using dms.tools;
using dms.view_models;
using Xceed.Wpf.AvalonDock.Layout;

namespace dms.gui
{
    /// <summary>
    /// Interaction logic for TaskInfoPage.xaml
    /// </summary>
    public partial class TaskInfoPage : UserControl
    {
        public TaskInfoPage(TaskInfoViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
