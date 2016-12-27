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
    /// Interaction logic for SolveStatisticPage.xaml
    /// </summary>
    public partial class SolveStatisticPage : UserControl
    {
        public SolveStatisticPage(SolveStatisticViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            for (int i = 0; i < vm.XNames.Length; i++)
                xGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = vm.XNames[i],
                    Binding = new Binding("X[" + i + "]"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
            for (int i = 0; i < vm.YNames.Length; i++)
                yGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = vm.YNames[i],
                    Binding = new Binding("Y[" + i + "]")
                });
        }
    }
}
