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

        public TaskTree(string Name,
            string[] sel, string[] per, string[] des, string[] solv, 
            TaskTreeViewModel vm)
        {
            Title = Name;
            Content = new ObservableCollection<TreeSection>
            {
                new SelectionTree(Title, sel, vm),
                new SolverTree(Title, per, des, vm),
                new SolutionsTree(Title, solv, vm)
            };
            deleteCommand = new ActionHandler(() => vm.UpdateTaskTree(), e => true);
            showInfoDialogCommand = new ActionHandler(() => 
            {
                TaskInfoViewModel t = new TaskInfoViewModel(Title);
                vm.SendRequestCreateView(t);
            }, e => true);
            showPreprocessingCreationHandler = new ActionHandler(() =>
            {
                PreprocessingViewModel t = new PreprocessingViewModel(Title);
                vm.SendRequestCreateView(t);
            }, e => true);
        }

        public ICommand ShowTaskInfoDialogCommand { get { return showInfoDialogCommand; } }
        public ICommand ShowPreprocessingCreationCommand { get { return showPreprocessingCreationHandler; } }
        public ICommand DeleteCommand { get { return deleteCommand; } }
    }
}
