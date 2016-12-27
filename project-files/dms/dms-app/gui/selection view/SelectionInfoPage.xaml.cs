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

using dms.view_models;
using dms.tools;

namespace dms.gui
{
    /// <summary>
    /// Interaction logic for SelectionInfoPage.xaml
    /// </summary>
    public partial class SelectionInfoPage : UserControl, IDocumentContent
    {
        private SelectionInfoViewModel vm;
        public SelectionInfoPage(SelectionInfoViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            this.vm = vm;

            vm.PropertyChanged += Vm_PropertyChanged;

            calculateColumns();
        }

        public LayoutDocument ParentDocument { get; set; }

        private void calculateColumns()
        {
            dataTable.Columns.Clear();
            var p = vm.DataColumns;

            int index = 0;
            foreach (var item in p)
            {
                dataTable.Columns.Add(new DataGridTextColumn
                {
                    Header = item,
                    Binding = new Binding(string.Format("[{0}]", index)),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
                index++;
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("DataColumns"))
            {
                calculateColumns();
            }
        }
    }
}
