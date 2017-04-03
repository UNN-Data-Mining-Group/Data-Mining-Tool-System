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
        private ActionHandler showSelectionLearnHandler;
        
        public SelectionLeaf(models.Task task, models.Selection selection, TaskTreeViewModel vm)
        {
            Title = selection.Name;
            parentTask = task.Name;
            deleteHandler = new ActionHandler(
                () =>
                {
                    new dms.services.preprocessing.DataHelper().deleteSelection(selection);
                    vm.UpdateTaskTree();
                }, e => true);
            showSelectionInfoHandler = new ActionHandler(
                () => 
                {
                    SelectionInfoViewModel t = new SelectionInfoViewModel(task.ID, selection.ID);
                    vm.SendRequestCreateView(t);
                }, e => true);
            showSelectionLearnHandler = new ActionHandler(
                () =>
                {
                    var t = new SelectionLearnStatisticViewModel(selection, task.Name);
                    vm.SendRequestCreateView(t);
                }, e => true);
        }

        public ICommand DeleteCommand { get { return deleteHandler; } }
        public ICommand ShowSelectionInfoDialog { get { return showSelectionInfoHandler; } }
        public ICommand ShowSelectionLearnDialog { get { return showSelectionLearnHandler; } }
    }
}

