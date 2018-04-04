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
    public partial class TaskDirectoryPage : UserControl, IDocumentContent
    {
        private TaskTreeViewModel taskVM = new TaskTreeViewModel();

        public LayoutDocument ParentDocument { get; set; }

        public event Action<string, UserControl> OnShowPage;

        public TaskDirectoryPage()
        {
            InitializeComponent();

            DataContext = taskVM;
            taskVM.requestViewCreation += CreateView;
        }

        public void updateTasks()
        {
            taskVM.UpdateTaskTree();
        }

        private void CreateView(ViewmodelBase vm)
        {
            if (vm is SolverCreationViewModel)
                CreateSolverCreationPage(vm as SolverCreationViewModel);
            else if (vm is SelectionInfoViewModel)
                CreateSelectionInfoPage(vm as SelectionInfoViewModel);
            else if (vm is TaskInfoViewModel)
                CreateTaskInfoPage(vm as TaskInfoViewModel);
            else if (vm is SelectionCreationViewModel)
                CreateSelectionCreationPage(vm as SelectionCreationViewModel);
            else if (vm is PerceptronInfoViewModel)
                CreatePerceptronInfoPage(vm as PerceptronInfoViewModel);
            else if (vm is DecisionTreeInfoViewModel)
                CreateDecisionTreeInfoPage(vm as DecisionTreeInfoViewModel);
            else if (vm is WardNetInfoViewModel)
                CreateWardNNInfoPage(vm as WardNetInfoViewModel);
            else if (vm is ConvNNInfoViewModel)
                CreateConvNNInfoPage(vm as ConvNNInfoViewModel);
            else if (vm is SolveViewModel)
                CreateSolvePage(vm as SolveViewModel);
            else if (vm is LearnSolverViewModel)
                CreateLearnSolverPage(vm as LearnSolverViewModel);
            else if (vm is SelectionLearnStatisticViewModel)
                CreateSelectionLearnStatisticPage(vm as SelectionLearnStatisticViewModel);
            else if (vm is PreprocessingViewModel)
                CreatePreprocessingCreationPage(vm as PreprocessingViewModel);
            else if (vm is CreateSolutionViewModel)
                CreateSolutionCreationPage(vm as CreateSolutionViewModel);
            else if (vm is SolveStatisticViewModel)
                CreateSolveStatisticPage(vm as SolveStatisticViewModel);
            else if (vm is KohonenInfoViewModel)
                CreateKohonenInfoPage(vm as KohonenInfoViewModel);
        }

        private void CreateSolverCreationPage(SolverCreationViewModel obj)
        {
            SolverCreationPage t = new SolverCreationPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/Создание решателя", t);
        }

        private void CreateSelectionInfoPage(SelectionInfoViewModel obj)
        {
            SelectionInfoPage t = new SelectionInfoPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.SelectionName + "/Информация", t);
        }

        private void CreateTaskInfoPage(TaskInfoViewModel obj)
        {
            TaskInfoPage t = new TaskInfoPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/Информация", t);
            obj.OnShowPreprocessingDetails += (p) => OnShowPage?.Invoke(obj.TaskName + "/" + p.PreprocessingName, new PreprocessingInfoPage(p));
        }

        private void CreateSelectionCreationPage(SelectionCreationViewModel obj)
        {
            CreateSelectionPage t = new CreateSelectionPage(obj);
            OnShowPage?.Invoke(obj.ParentTask + "/Создание выборки", t);
        }

        private void CreatePerceptronInfoPage(PerceptronInfoViewModel obj)
        {
            PerceptronInfoPage t = new PerceptronInfoPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.Name + "/Информация", t);
        }

        private void CreateKohonenInfoPage(KohonenInfoViewModel obj)
        {
            KohonenInfoPage t = new KohonenInfoPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.Name + "/Информация", t);
        }

        private void CreateWardNNInfoPage(WardNetInfoViewModel obj)
        {
            WardInfoPage t = new WardInfoPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.Name + "/Информация", t);
        }

        private void CreateConvNNInfoPage(ConvNNInfoViewModel obj)
        {
            ConvNNInfoPage t = new ConvNNInfoPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.Name + "/Информация", t);
        }
        
        private void CreateDecisionTreeInfoPage(DecisionTreeInfoViewModel obj)
        {
            DecisionTreeInfoPage t = new DecisionTreeInfoPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.Name + "/Информация", t);
        }

        private void CreateSolvePage(SolveViewModel obj)
        {
            SolvePage t = new SolvePage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.SolverName + "/Решение", t);
        }

        private void CreateLearnSolverPage(LearnSolverViewModel obj)
        {
            LearnSolverPage t = new LearnSolverPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.SolverName + "/Обучение", t);
        }

        private void CreateSelectionLearnStatisticPage(SelectionLearnStatisticViewModel obj)
        {
            SelectionLearnStatisticsPage t = new SelectionLearnStatisticsPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.SelectionName + "/Качество обучения", t);
        }

        private void CreatePreprocessingCreationPage(PreprocessingViewModel obj)
        {
            PreprocessingCreationPage t = new PreprocessingCreationPage(obj);
            OnShowPage?.Invoke(obj.Task.Name + "/Создание преобразования", t);
        }

        public void CreateSolutionCreationPage(CreateSolutionViewModel obj)
        {
            CreateSolutionPage t = new CreateSolutionPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/Создание решения", t);
        }

        public void CreateSolveStatisticPage(SolveStatisticViewModel obj)
        {
            SolveStatisticPage t = new SolveStatisticPage(obj);
            OnShowPage?.Invoke(obj.TaskName + "/" + obj.Name, t);
        }
    }
}
