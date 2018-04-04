using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;

namespace dms.view_models
{
    public class SolverTree : TreeSection
    {
        private ActionHandler createSolverCommand;

        public SolverTree(models.Task task, 
            models.TaskSolver[] per, 
            models.TaskSolver[] des,
            models.TaskSolver[] wards, 
            models.TaskSolver[] convnets,
            models.TaskSolver[] kohnets,
            TaskTreeViewModel vm) : base("Решатели")
        {
            ParentTask = task.Name;
            Content = new ObservableCollection<TreeSection>
            {
                new PerceptronTree(task, per, vm),
                new DecisionTree(task, des, vm),
                new WardTree(task, wards, vm),
                new ConvNNTree(task, convnets, vm),
                new KohonenTree(task, kohnets, vm)
            };
            createSolverCommand = new ActionHandler(() => 
            {
                SolverCreationViewModel t = new SolverCreationViewModel(task);
                t.OnClose += (s,p) => vm.UpdateTaskTree();
                vm.SendRequestCreateView(t);
            }, o => true);
        }

        public string ParentTask { get; set; }
        public ICommand ShowCreateSolverWindowCommand { get { return createSolverCommand; } }
    }
}
