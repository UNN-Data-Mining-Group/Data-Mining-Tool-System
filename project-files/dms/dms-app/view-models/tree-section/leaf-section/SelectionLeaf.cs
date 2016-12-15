using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using dms.tools;

namespace dms.view_models
{
    public class SelectionLeaf : TreeSection
    {
        private string parentTask;
        private ActionHandler deleteHandler;
        private ActionHandler showSelectionInfoHandler;

        public SelectionLeaf(string taskName, string name, TaskTreeViewModel vm)
        {
            Title = name;
            parentTask = taskName;
            deleteHandler = new ActionHandler(() => vm.DeleteSelection(parentTask, Title), e => true);
            showSelectionInfoHandler = new ActionHandler(() => vm.ShowSelectionInfoDialog(parentTask, Title), e => true);
        }

        public ICommand DeleteCommand { get { return deleteHandler; } }
        public ICommand ShowSelectionInfoDialog { get { return showSelectionInfoHandler; } }
    }
}
