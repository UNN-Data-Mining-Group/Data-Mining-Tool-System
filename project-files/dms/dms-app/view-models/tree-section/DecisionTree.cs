using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class DecisionTree : TreeSection
    {
        public DecisionTree(models.Task task, models.TaskSolver[] trees,
            TaskTreeViewModel vm) : base("Деревья решений", trees.Select(x => x.Name).ToArray())
        {
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < trees.Length; i++)
            {
                Content.Add(new SolverLeaf(task, trees[i], "DecisionTree", vm));
            }
        }
    }
}
