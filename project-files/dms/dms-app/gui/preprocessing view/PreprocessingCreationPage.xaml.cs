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
    /// Interaction logic for PreprocessingCreationPage.xaml
    /// </summary>
    public partial class PreprocessingCreationPage : UserControl, IDocumentContent
    {
        public PreprocessingCreationPage(PreprocessingViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.OnClose += OnClose;
        }

        public LayoutDocument ParentDocument { get; set; }

        private void OnClose(object sender, EventArgs e)
        {
            if (ParentDocument != null)
            {
                ParentDocument.Close();
                ParentDocument = null;
            }
        }

        private void CreateTemplateForPreprocessing_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CreateTemplateForPreprocessing.Text != null && CreateTemplateForPreprocessing.Text != "" || IsUsingExitingTemp.IsChecked.Value)
            {
                CreatePreprocessing.IsEnabled = true;
            }
            else
            {
                CreatePreprocessing.IsEnabled = false;
            }
        }

        private void IsUsingExitingTemp_Checked(object sender, RoutedEventArgs e)
        {
            if (IsUsingExitingTemp.IsChecked.Value)
            {
                CreatePreprocessing.IsEnabled = true;
            }
            else
            {
                CreatePreprocessing.IsEnabled = false;
            }
        }

        private void PreprocessingName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PreprocessingName.Text != null && PreprocessingName.Text != "")
            {
                if (CreateTemplateForPreprocessing.Text != null && CreateTemplateForPreprocessing.Text != "" || IsUsingExitingTemp.IsChecked.Value)
                {
                    CreatePreprocessing.IsEnabled = true;
                }
                else
                {
                    CreatePreprocessing.IsEnabled = false;
                }
            }
            else
            {
                CreatePreprocessing.IsEnabled = false;
            }
        }
    }
}
