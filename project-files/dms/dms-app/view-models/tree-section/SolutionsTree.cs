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
    public class SolutionsTree : TreeSection
    {
        private ActionHandler createSolutionHandler;

        public SolutionsTree(string taskName, string[] solutions, TaskTreeViewModel vm) : base("Решения", solutions)
        {
            createSolutionHandler = new ActionHandler(() =>
            {
                var t = new CreateSolutionViewModel(taskName);
                vm.SendRequestCreateView(t);
            }, e => true);
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < solutions.Length; i++)
            {
                Content.Add(new SolutionLeaf(taskName, solutions[i], vm));
            }
        }

        public ICommand ShowCreateSolutionCommand { get { return createSolutionHandler; } }
    }
}
