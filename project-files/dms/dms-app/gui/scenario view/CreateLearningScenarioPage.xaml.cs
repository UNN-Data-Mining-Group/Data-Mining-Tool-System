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
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;


using dms.view_models;


namespace dms.gui
{
    /// <summary>
    /// Interaction logic for CreateLearningScenarioPage.xaml
    /// </summary>
    public partial class CreateLearningScenarioPage : UserControl
    {
        public CreateLearningScenarioPage()
        {
            InitializeComponent();
            DataContext = new LearningScenarioViewModel();
        }
    }
}
