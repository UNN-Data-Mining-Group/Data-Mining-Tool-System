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
    public class SelectionTree : TreeSection
    {
        private ActionHandler createSelection;
        
        public SelectionTree(models.Task task, models.Selection[] selections,
            TaskTreeViewModel vm) : base("Выборки")
        {
            createSelection = new ActionHandler(() => 
            {
                SelectionCreationViewModel t = new SelectionCreationViewModel(task.ID);
                vm.SendRequestCreateView(t);
            }, e => true);

            ParentTask = task.Name;
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < selections.Length; i++)
            {
                Content.Add(new SelectionLeaf(task, selections[i], vm));
            }
        }

        public string ParentTask { get; set; }
        public ICommand ShowCreateSelectionDialogCommand { get { return createSelection; } }

    }
}
