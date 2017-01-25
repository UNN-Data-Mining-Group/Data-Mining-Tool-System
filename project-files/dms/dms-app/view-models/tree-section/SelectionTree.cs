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

        public SelectionTree(int taskId, string[] selections, 
            TaskTreeViewModel vm) : base("Выборки")
        {
            createSelection = new ActionHandler(() => 
            {
                SelectionCreationViewModel t = new SelectionCreationViewModel(taskId);
                vm.SendRequestCreateView(t);
            }, e => true);

            ParentTask = ((dms.models.Task)dms.services.DatabaseManager.SharedManager.entityById(taskId, typeof(dms.models.Task))).Name;
            Content = new ObservableCollection<TreeSection>();
            for(int i = 0; i < selections.Length; i++)
            {
                Content.Add(new SelectionLeaf(ParentTask, selections[i], vm));
            }
        }

        public string ParentTask { get; set; }
        public ICommand ShowCreateSelectionDialogCommand { get { return createSelection; } }
    }
}
