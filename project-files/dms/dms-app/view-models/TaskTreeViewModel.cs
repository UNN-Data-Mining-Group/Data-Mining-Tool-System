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
    public class TaskTreeViewModel : ViewmodelBase
    {
        private ObservableCollection<TaskTree> tasks;

        public TaskTreeViewModel()
        {
            Tasks = new ObservableCollection<TaskTree>
            {
                new TaskTree("Ирис", 
                    new string[] { "Выборка 1", "Выборка 2" },
                    new string[] { "Персептрон 1", "Персептрон 2" },
                    new string[] { "Дерево 1", },
                    new string[] { }, 
                    this), 
                new TaskTree("Морское ушко", 
                    new string[] {"Выборка 1"}, 
                    new string[] { "П_му1"}, 
                    new string[] { }, 
                    new string[] { "Решение 1"},
                    this)
                };
        }

        public event Action<ViewmodelBase> requestViewCreation;

        public ObservableCollection<TaskTree> Tasks
        {
            get { return tasks; }
            set { tasks = value; NotifyPropertyChanged(); }
        }

        public void SendRequestCreateView(ViewmodelBase vm)
        {
            requestViewCreation?.Invoke(vm);
        }

        public void UpdateTaskTree()
        {

        }
    }
}
