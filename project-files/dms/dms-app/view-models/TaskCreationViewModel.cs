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

        public ObservableCollection<SelectionCreationViewModel> Selections { get; set; }
        public ObservableCollection<ParameterCreationViewModel> Parameters { get; set; }

        public ICommand CancelCommand { get { return cancelHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }
        public ICommand AddSelectionCommand { get { return addSelectionHandler; } }

        public TaskCreationViewModel()
        { 
            cancelHandler = new ActionHandler(Cancel, (e) => true);
            createHandler = new ActionHandler(CreateTask, (e) => CanCreateTask());
            addSelectionHandler = new ActionHandler(AddSelection, e => true);

            Selections = new ObservableCollection<SelectionCreationViewModel>();
            Parameters = new ObservableCollection<ParameterCreationViewModel>();
        }

        public void UpdateParameterList(Tuple<string, bool> fileInfo)
        {
            string filePath = fileInfo.Item1;
            bool isAdd = fileInfo.Item2;

            int countFiles = 0;
            foreach(var vm in Selections)
            {
                if (!vm.FilePath.Equals(String.Empty))
                    countFiles++;
            }

            if ((countFiles == 1) && (isAdd))
            {
                Parameters.Add(new ParameterCreationViewModel(1));
                Parameters.Add(new ParameterCreationViewModel(2,"Parameter 2", "enum", true));
            }
            else if((countFiles > 1) && (isAdd))
            {

            }
            else if (countFiles == 0)
            {
                Parameters.Clear();
            }

            createHandler.RaiseCanExecuteChanged();
        }

        public void Cancel()
        {
        }

        public void CreateTask()
        {
        }

        public bool CanCreateTask()
        {
            return Parameters.Count > 0;
        }

        public void AddSelection()
        {
            var s = new SelectionCreationViewModel(TaskName);
            s.OnDelete += (o, e) => DeleteSelection((SelectionCreationViewModel)o);
            s.OnFileChanged += (o, e) => UpdateParameterList(e.Data);
            Selections.Add(s);
        }

        public void DeleteSelection(SelectionCreationViewModel s)
        {
            Selections.Remove(s);
        }
    }
}
