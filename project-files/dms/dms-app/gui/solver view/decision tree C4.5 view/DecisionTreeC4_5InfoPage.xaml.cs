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
    /// Interaction logic for DecisionTreeInfoPage.xaml
    /// </summary>
    public partial class DecisionTreeC4_5InfoPage : UserControl
    {
        public DecisionTreeC4_5InfoPage(DecisionTreeC4_5InfoViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
