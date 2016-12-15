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
    /// Interaction logic for TaskInfoPage.xaml
    /// </summary>
    public partial class TaskInfoPage : UserControl
    {
        public TaskInfoPage(TaskInfoViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            PreprocessingTable.Columns.Clear();
            var p = vm.PreprocessingList[0].ParameterProcessing;

            PreprocessingTable.Columns.Add(new DataGridTextColumn
            {
                Header = "Название",
                Binding = new Binding("Name"),
                Width = new DataGridLength(2, DataGridLengthUnitType.Star)
            });

            int index = 0;
            foreach(var item in p)
            {
                PreprocessingTable.Columns.Add(new DataGridTextColumn
                {
                    Header = item.Item1.Name,
                    Binding = new Binding(string.Format("ParameterProcessing[{0}].Item2", index)),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
                index++;
            }
        }
    }
}
