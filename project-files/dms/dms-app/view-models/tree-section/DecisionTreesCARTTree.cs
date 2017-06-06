using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class DecisionTreesCARTTree : TreeSection
    {
        public DecisionTreesCARTTree(models.Task task, models.TaskSolver[] trees,
            TaskTreeViewModel vm) : base("Деревья решений CART", trees.Select(x => x.Name).ToArray())
        {
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < trees.Length; i++)
            {
                Content.Add(new SolverLeaf(task, trees[i], "DecisionTreeCART", vm));
            }
        }
    }
}
