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
using System.IO;
using Microsoft.Win32;
using dms.view_models;

namespace dms.gui
{
    /// <summary>
    /// Interaction logic for SelectionAddPane.xaml
    /// </summary>
    public partial class SelectionAddPane : UserControl
    {
        public SelectionAddPane()
        {
            InitializeComponent();
        }

        public Button CreateSelection_button { get; set; }

        private void SelectionName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CreateSelection_button != null && SelectionName.Text != null && SelectionName.Text != "")
            {
                CreateSelection_button.IsEnabled = true;
            }
            else if (CreateSelection_button != null)
            {
                CreateSelection_button.IsEnabled = false;
            }
        }
    }
}
