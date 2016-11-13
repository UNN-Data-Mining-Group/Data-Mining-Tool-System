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

namespace dms.gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LayoutDocument[] l = new LayoutDocument[] { new LayoutDocument(), new LayoutDocument() };
            l[0].Title = "Doc 1";
            l[1].Title = "Doc 2";
            documentPane.Children.Add(l[0]);
            documentPane.Children.Add(l[1]);

            TaskCreationWindow wnd = new TaskCreationWindow();
            wnd.Show();
        }
    }
}
