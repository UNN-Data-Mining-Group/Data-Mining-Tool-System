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
        private string solverName;
        private ActionHandler showSolverInfoHandler;
        private ActionHandler showSolveDialogHandler;
        private ActionHandler showLearnDialogHandler;
        private ActionHandler deleteSolverHandler;

        public SolverLeaf(string taskName, string solverName, string solverType, 
            TaskTreeViewModel vm) : base(solverName)
        {
            parentTask = taskName;
            this.solverType = solverType;
            this.solverName = solverName;
            showSolverInfoHandler = new ActionHandler(() => vm.SendRequestCreateView(CreateSolverInfoViewModel()), e => true);
            showSolveDialogHandler = new ActionHandler(() => vm.SendRequestCreateView(new SolveViewModel(parentTask, solverName)), e => true);
            showLearnDialogHandler = new ActionHandler(() => vm.SendRequestCreateView(new LearnSolverViewModel(parentTask, solverName)), e => true);
        }

        public ViewmodelBase CreateSolverInfoViewModel()
        {
            if (solverType.Equals("Персептрон"))
                return new PerceptronInfoViewModel(parentTask, solverName);
            else if (solverType.Equals("Дерево решений"))
                return new DecisionTreeInfoViewModel(parentTask, solverName);
            else
                return null;
        }

        public ICommand ShowSolverInfoCommand { get { return showSolverInfoHandler; } }
        public ICommand ShowSolveDialogCommand { get { return showSolveDialogHandler; } }
        public ICommand ShowLearnDialogCommand { get { return showLearnDialogHandler; } }
        public ICommand DeleteSolverCommand { get { return deleteSolverHandler; } }
    }
}
