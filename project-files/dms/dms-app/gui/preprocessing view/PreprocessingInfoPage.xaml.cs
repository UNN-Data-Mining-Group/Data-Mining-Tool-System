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
using Xceed.Wpf.AvalonDock.Layout;

using dms.tools;
using dms.view_models;

namespace dms.gui
{
    /// <summary>
    /// Interaction logic for PreprocessingInfoPage.xaml
    /// </summary>
    public partial class PreprocessingInfoPage : UserControl
    {
        public PreprocessingInfoPage(PreprocessingViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
