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
using dms.tools;

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

            var vm = new MainWindowViewModel();
            DataContext = vm;
            vm.requestTaskCreation += (s, e) => { var p = new TaskCreationPage(e.Data); ShowPage("Создание задачи", p); };
            vm.requestTaskTreeShow += (visible) =>
            {
                if (visible && taskPanel.Parent == null)
                {
                    windowPanel.Children.Insert(0, taskPanelPane);
                    taskPanelPane.Children.Add(taskPanel);
                    taskPanel.Show();
                }
                else if (!visible && taskPanel.Parent != null)
                {
                    taskPanel.Close();
                }
            };
            vm.requestLSShow += (e) => { var p = new LearningScenarioManagerPage(e); p.OnShowPage += ShowPage; ShowPage("Сценарии обучения", p); };

            TaskDirectoryPage t = new TaskDirectoryPage();
            t.OnShowPage += ShowPage;
            taskPanel.Content = t;
        }

        private void ShowPage(string title, UserControl control)
        {
            int documentPaneIndex = -1;
            int index = 0;
            foreach(ILayoutPanelElement item in windowPanel.Children)
            {
                if ((item is LayoutDocumentPane) && (item != documentPane))
                {
                    documentPaneIndex = index;
                    break;
                }
                index++;
            }
            if (documentPaneIndex < 0)
            {
                windowPanel.Children.Remove(documentPane);

                documentPaneIndex = windowPanel.ChildrenCount;
                windowPanel.Children.Add(new LayoutDocumentPane());
            }

            LayoutDocument d = new LayoutDocument();
            d.Title = title;
            d.Content = control;
            d.IsActive = true;
            if (control is IDocumentContent)
            {
                (control as IDocumentContent).ParentDocument = d;
            }

            var p = windowPanel.Children[documentPaneIndex] as LayoutDocumentPane;
            p.Children.Add(d);
        }

        private void CloseDocument(LayoutDocument document)
        {
            document.Close();
        }

        private void CleanDocumentArea(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            bool hasDocument = false;
            var elements = windowPanel.Children;

            int i = 0;
            while(i < windowPanel.ChildrenCount)
            {
                if (elements[i] is LayoutDocumentPaneGroup)
                {
                    var group = elements[i] as LayoutDocumentPaneGroup;
                    if (group.ChildrenCount < 2)
                    {
                        elements.RemoveAt(i);
                        if (group.ChildrenCount == 1)
                        {
                            elements.Add(group.Children[0]);
                            hasDocument = true;
                        }
                    }
                    else
                    {
                        hasDocument = true;
                        i++;
                    }
                }
                else if ((elements[i] is LayoutDocumentPane))
                {
                    var docPane = elements[i] as LayoutDocumentPane;
                    if (docPane.ChildrenCount > 0)
                    {
                        hasDocument = true;
                        i++;
                    }
                    else
                        elements.RemoveAt(i);
                }
                else i++;
            }

            if (!hasDocument)
                windowPanel.Children.Add(documentPane);
        }
    }
}
