using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using dms.tools;

namespace dms.view_models
{
    public class SolutionLeaf : TreeSection
    {
        private ActionHandler showSolutionInfoHandler;

        public SolutionLeaf(string taskName, string name, TaskTreeViewModel vm)
        {
            Title = name;
            showSolutionInfoHandler = new ActionHandler(() => 
            {
                var t = new SolveStatisticViewModel(taskName, name);
                vm.SendRequestCreateView(t);
            }, e => true);
        }

        public ICommand ShowSolutionInfoCommand { get { return showSolutionInfoHandler; } }
    }
}
