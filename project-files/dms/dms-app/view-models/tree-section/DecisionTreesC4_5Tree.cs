using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class DecisionTreesC4_5Tree : TreeSection
    {
        public DecisionTreesC4_5Tree(models.Task task, models.TaskSolver[] trees,
            TaskTreeViewModel vm) : base("Деревья решений C4.5", trees.Select(x => x.Name).ToArray())
        {
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < trees.Length; i++)
            {
                Content.Add(new SolverLeaf(task, trees[i], "DecisionTreeC4_5", vm));
            }
        }
    }
}
