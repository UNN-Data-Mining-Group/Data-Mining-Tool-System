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

        public SolverTree(string taskName, string[] per, string[] des, TaskTreeViewModel vm) : base("Решатели")
        {
            ParentTask = taskName;
            Content = new ObservableCollection<TreeSection>
            {
                new PerceptronTree(taskName, per, vm),
                new DecisionTreesTree(taskName, des, vm)
            };
            createSolverCommand = new ActionHandler(() => 
            {
                SolverCreationViewModel t = new SolverCreationViewModel(taskName);
                vm.SendRequestCreateView(t);
            }, o => true);
        }

        public string ParentTask { get; set; }
        public ICommand ShowCreateSolverWindowCommand { get { return createSolverCommand; } }
    }
}
