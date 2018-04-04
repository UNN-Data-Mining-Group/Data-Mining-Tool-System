using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using dms.tools;

namespace dms.view_models
{
    public class TaskTree : TreeSection
    {
        private ActionHandler deleteCommand;
        private ActionHandler showInfoDialogCommand;
        private ActionHandler showPreprocessingCreationHandler;
        
        public TaskTree(models.Task task,
            models.Selection[] sel, 
            models.TaskSolver[] per, 
            models.TaskSolver[] des,
            models.TaskSolver[] wards, 
            models.TaskSolver[] convnets, 
            models.TaskSolver[] kohnets,
            string[] solv, 
            TaskTreeViewModel vm)
        {
            Title = task.Name;
            Content = new ObservableCollection<TreeSection>
            {
                new SelectionTree(task, sel, vm),
                new SolverTree(task, per, des, wards, convnets, kohnets, vm),
                new SolutionsTree(Title, solv, vm)
            };
            deleteCommand = new ActionHandler(
                () =>
                {
                    new dms.services.preprocessing.DataHelper().deleteTask(task);
                    vm.UpdateTaskTree();
                }, e => true);
            showInfoDialogCommand = new ActionHandler(() => 
            {
                TaskInfoViewModel t = new TaskInfoViewModel(task);
                vm.SendRequestCreateView(t);
            }, e => true);
            showPreprocessingCreationHandler = new ActionHandler(() =>
            {
                PreprocessingViewModel t = new PreprocessingViewModel(task, -1);
                vm.SendRequestCreateView(t);
            }, e => true);
        }

        public ICommand ShowTaskInfoDialogCommand { get { return showInfoDialogCommand; } }
        public ICommand ShowPreprocessingCreationCommand { get { return showPreprocessingCreationHandler; } }
        public ICommand DeleteCommand { get { return deleteCommand; } }
    }
}
