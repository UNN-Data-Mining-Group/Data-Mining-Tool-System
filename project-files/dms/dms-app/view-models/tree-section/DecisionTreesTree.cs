using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class DecisionTreesTree : TreeSection
    {
        public DecisionTreesTree(string taskName, string[] trees,
            TaskTreeViewModel vm) : base("Деревья решений", trees)
        {
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < trees.Length; i++)
            {
                Content.Add(new SolverLeaf(taskName, trees[i], "Дерево решений", vm));
            }
        }
    }
}
