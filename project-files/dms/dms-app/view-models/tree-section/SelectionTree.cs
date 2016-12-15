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
        private ActionHandler deleteSelection;

        public SelectionTree(string taskName, string[] selections, 
            TaskTreeViewModel vm) : base("Выборки")
        {
            this.createSelection = new ActionHandler(() => vm.ShowCreateSelectionDialog(taskName), e => true);
            this.deleteSelection = new ActionHandler(() => vm.DeleteSelection(taskName, Title), e => true);
            ParentTask = taskName;
            Content = new ObservableCollection<TreeSection>();
            for(int i = 0; i < selections.Length; i++)
            {
                Content.Add(new SelectionLeaf(taskName, selections[i], vm));
            }
        }

        public string ParentTask { get; set; }
        public ICommand ShowCreateSelectionDialogCommand { get { return createSelection; } }
    }
}
