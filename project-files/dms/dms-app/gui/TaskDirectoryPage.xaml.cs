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
    /// Interaction logic for TaskDirectoryPage.xaml
    /// </summary>
    public partial class TaskDirectoryPage : UserControl
    {
        private TaskTreeViewModel taskVM = new TaskTreeViewModel();
        private LayoutDocumentPane parentArea;

        public TaskDirectoryPage(LayoutDocumentPane parentArea)
        {
            InitializeComponent();

            this.parentArea = parentArea;
            DataContext = taskVM;

            taskVM.requestTaskCreation += TaskVM_requestTaskCreation;
            taskVM.requestSelectionCreation += TaskVM_requestSelectionCreation;
            taskVM.requestTaskInfoShow += TaskVM_requestTaskInfoShow;
            taskVM.requestSelectionInfoShow += TaskVM_requestSelectionInfoShow;
        }

        private void TaskVM_requestSelectionInfoShow(object sender, EventArgs<SelectionInfoViewModel> e)
        {
            SelectionInfoPage t = new SelectionInfoPage(e.Data);

            LayoutDocument d = new LayoutDocument();
            d.Title = e.Data.TaskName + "/" + e.Data.SelectionName + "/Информация";
            d.Content = t;

            parentArea.Children.Add(d);
            d.IsActive = true;
        }

        private void TaskVM_requestTaskInfoShow(object sender, EventArgs<TaskInfoViewModel> e)
        {
            TaskInfoPage t = new TaskInfoPage(e.Data);

            LayoutDocument d = new LayoutDocument();
            d.Title = e.Data.TaskName + "/Информация";
            d.Content = t;

            parentArea.Children.Add(d);
            d.IsActive = true;
        }

        private void TaskVM_requestSelectionCreation(object sender, EventArgs<SelectionCreationViewModel> e)
        {
            CreateSelectionPage t = new CreateSelectionPage(e.Data);

            LayoutDocument d = new LayoutDocument();
            d.Title = e.Data.ParentTask + "/Создание выборки";
            d.Content = t;

            parentArea.Children.Add(d);
            d.IsActive = true;
        }

        private void TaskVM_requestTaskCreation(object sender, EventArgs<TaskCreationViewModel> e)
        {
            TaskCreationPage t = new TaskCreationPage(e.Data);

            LayoutDocument d = new LayoutDocument();
            d.Title = "Создание задачи";
            d.Content = t;

            parentArea.Children.Add(d);
            d.IsActive = true;
        }
    }
}
