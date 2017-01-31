using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using dms.tools;

namespace dms.view_models
{
    public class SolverLeaf : TreeSection
    {
        private string parentTask;
        private string solverType;
        private models.TaskSolver solver;
        private ActionHandler showSolverInfoHandler;
        private ActionHandler showSolveDialogHandler;
        private ActionHandler showLearnDialogHandler;
        private ActionHandler deleteSolverHandler;

        public SolverLeaf(models.Task task, models.TaskSolver solver, string solverType, 
            TaskTreeViewModel vm) : base(solver.Name)
        {
            parentTask = task.Name;
            this.solverType = solverType;
            this.solver = solver;
            showSolverInfoHandler = new ActionHandler(() => vm.SendRequestCreateView(CreateSolverInfoViewModel()), e => true);
            showSolveDialogHandler = new ActionHandler(() => vm.SendRequestCreateView(new SolveViewModel(parentTask, solver.Name)), e => true);
            showLearnDialogHandler = new ActionHandler(() => vm.SendRequestCreateView(new LearnSolverViewModel(parentTask, solver.Name)), e => true);
        }

        public ViewmodelBase CreateSolverInfoViewModel()
        {
            if (solverType.Equals("Персептрон"))
                return new PerceptronInfoViewModel(parentTask, solver.Name);
            else if (solverType.Equals("Дерево решений"))
                return new DecisionTreeInfoViewModel(parentTask, solver.Name);
            else
                return null;
        }

        public ICommand ShowSolverInfoCommand { get { return showSolverInfoHandler; } }
        public ICommand ShowSolveDialogCommand { get { return showSolveDialogHandler; } }
        public ICommand ShowLearnDialogCommand { get { return showLearnDialogHandler; } }
        public ICommand DeleteSolverCommand { get { return deleteSolverHandler; } }
    }
}
