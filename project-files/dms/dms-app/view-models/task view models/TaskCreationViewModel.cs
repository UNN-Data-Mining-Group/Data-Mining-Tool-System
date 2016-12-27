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

        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; NotifyPropertyChanged(); }
        }

        public event EventHandler OnClose;
        public ICommand CancelCommand { get { return cancelHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }

        public TaskCreationViewModel()
        { 
            cancelHandler = new ActionHandler(Cancel, (e) => true);
            createHandler = new ActionHandler(CreateTask, (e) => true);
        }

        public void Cancel()
        {
            OnClose?.Invoke(this, null);
        }

        public void CreateTask()
        {
            OnClose?.Invoke(this, null);
        }
    }
}
