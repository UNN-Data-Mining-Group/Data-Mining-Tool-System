using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using dms.tools;
using dms.models;
using dms.services.preprocessing;

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
            UpdateTaskTree();
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

                var pers = new List<TaskSolver>();
                var wards = new List<TaskSolver>();
                var convnets = new List<TaskSolver>();
                var dectrees = new List<TaskSolver>();
                var kohnets = new List<TaskSolver>();

                foreach (TaskSolver solver in solvers)
                {
                    if (solver.TypeName.Equals("Perceptron"))
                        pers.Add(solver);
                    else if (solver.TypeName.Equals("WardNN"))
                        wards.Add(solver);
                    else if (solver.TypeName.Equals("ConvNN"))
                        convnets.Add(solver);
                    else if (solver.TypeName.Equals("DecisionTree"))
                        dectrees.Add(solver);
                    else if (solver.TypeName.Equals("KohonenNet"))
                        kohnets.Add(solver);
                }

                Tasks.Add(new TaskTree
                    (task,
                    selections.ToArray(),
                    pers.ToArray(),
                    dectrees.ToArray(),
                    wards.ToArray(),
                    convnets.ToArray(),
                    kohnets.ToArray(),
                    new string[] { },
                    this));
            }
         //   new PCA().test();
        }
    }
}