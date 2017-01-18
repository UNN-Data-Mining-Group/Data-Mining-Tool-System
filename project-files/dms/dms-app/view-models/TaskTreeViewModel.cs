using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using dms.tools;
using dms.models;

namespace dms.view_models
{
    public class TaskTreeViewModel : ViewmodelBase
    {
        private ObservableCollection<TaskTree> tasks;

        public TaskTreeViewModel()
        {            
            Tasks = new ObservableCollection<TaskTree>();
            UpdateTaskTree();
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
            for (int i = Tasks.Count - 1; i >= 0; i--)
            {
                Tasks.RemoveAt(i);
            }
            List<Entity> tasks = models.Task.all(typeof(models.Task));
            foreach (models.Task task in tasks)
            {
                List<TaskSolver> solvers = TaskSolver.solversOfTaskId(task.ID);
                List<Selection> selections = Selection.selectionsOfDefaultTemplateWithTaskId(task.ID);

                Tasks.Add(new TaskTree(task.Name,
                    selections.Select(x => x.Name).ToArray(),
                    solvers.Select(x => x.Name).ToArray(),
                    new string[] { },
                    new string[] { },
                    this));
            }
        }
    }
}
