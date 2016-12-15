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

        private Action<string> onDelete;
        private Action<string> showInfoDialog;

        public TaskTree(string Name,
            string[] sel, string[] per, string[] des, string[] solv, 
            TaskTreeViewModel vm)
        {
            Title = Name;
            Content = new ObservableCollection<TreeSection>
            {
                new SelectionTree(Title, sel, vm),
                new SolverTree(per, des, vm),
                new SolutionsTree(solv, vm)
            };
            onDelete = vm.DeleteTask;
            showInfoDialog = vm.ShowTaskInfoDialog;
        }

        public ICommand ShowTaskInfoDialogCommand
        {
            get
            {
                return showInfoDialogCommand ?? (showInfoDialogCommand = new ActionHandler(() => showInfoDialog(Title), e => true));
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new ActionHandler(() => onDelete(Title), (e) => true));
            }
        }
    }
}
