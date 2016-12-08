using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class SelectionCreationViewModel : ViewmodelBase
    {
        private ActionHandler deleteSelectionHandler;

        public event EventHandler OnDelete;
        public ICommand DeleteSelectionCommand { get { return deleteSelectionHandler; } }

        public void DeleteSelection()
        {
            if (OnDelete != null)
                OnDelete(this, null);
        }
    }

    public class Parameter
    {

    }

    public class TaskCreationViewModel : ViewmodelBase
    {
        private string taskName = "Новая задача";
        private ActionHandler cancelHandler;
        private ActionHandler createHandler;
        private ActionHandler addSelectionHandler;

        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; NotifyPropertyChanged(); }
        }

        public event EventHandler<EventArgs<SelectionCreationViewModel>> OnCreatingSelection;

        public ObservableCollection<SelectionCreationViewModel> Selections { get; set; }
        public ObservableCollection<Parameter> Parameters { get; set; }

        public ICommand CancelCommand { get { return cancelHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }
        public ICommand AddSelectionCommand { get { return addSelectionHandler; } }

        public TaskCreationViewModel()
        {
            cancelHandler = new ActionHandler(Cancel, (e) => true);
            createHandler = new ActionHandler(CreateTask, (e) => CanCreateTask());
            addSelectionHandler = new ActionHandler(AddSelection, e => true);

            Selections = new ObservableCollection<SelectionCreationViewModel>();
        }

        public void Cancel()
        {

        }

        public void CreateTask()
        {

        }

        public bool CanCreateTask()
        {
            return false;
        }

        public void AddSelection()
        {
            var s = new SelectionCreationViewModel();
            s.OnDelete += (o, e) => DeleteSelection((SelectionCreationViewModel)o);
            Selections.Add(s);
        }

        public void DeleteSelection(SelectionCreationViewModel s)
        {
            Selections.Remove(s);
        }
    }
}
