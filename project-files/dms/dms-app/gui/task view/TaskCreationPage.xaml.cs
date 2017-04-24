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
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using System.Runtime.Serialization.Formatters.Binary;

using dms.tools;
using dms.view_models;
using dms.models;
using System.IO;
using System.IO.Compression;

namespace dms.gui
{

    /// <summary>
    /// Interaction logic for TaskCreation.xaml
    /// </summary>
    public partial class TaskCreationPage : UserControl, IDocumentContent
    {
        private TaskCreationViewModel viewModel;
        public TaskCreationPage(TaskCreationViewModel vm)
        {
            InitializeComponent();

            viewModel = vm;
            DataContext = viewModel;
            viewModel.OnClose += OnClose;
        }

        public LayoutDocument ParentDocument { get; set; }

        private void OnClose(object sender, EventArgs e)
        {
            if(ParentDocument != null)
            {
                ParentDocument.Close();
                ParentDocument = null;
            }
        }
    }
}
