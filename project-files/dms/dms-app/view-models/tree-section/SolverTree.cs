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

        public SolverTree(models.Task task, models.TaskSolver[] per, models.TaskSolver[] des, TaskTreeViewModel vm) : base("Решатели")
        {
            ParentTask = task.Name;
            Content = new ObservableCollection<TreeSection>
            {
                new PerceptronTree(task, per, vm),
                new DecisionTreesTree(task, des, vm)
            };
            createSolverCommand = new ActionHandler(() => 
            {
                SolverCreationViewModel t = new SolverCreationViewModel(task.Name);
                vm.SendRequestCreateView(t);
            }, o => true);
        }

        public string ParentTask { get; set; }
        public ICommand ShowCreateSolverWindowCommand { get { return createSolverCommand; } }
    }
}
