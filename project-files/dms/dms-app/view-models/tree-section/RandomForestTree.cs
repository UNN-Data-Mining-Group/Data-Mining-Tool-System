using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    class RandomForestTree : TreeSection
    {
        public RandomForestTree(models.Task task, models.TaskSolver[] trees,
            TaskTreeViewModel vm) : base("Случайный лес", trees.Select(x => x.Name).ToArray())
        {
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < trees.Length; i++)
            {
                Content.Add(new SolverLeaf(task, trees[i], "RandomForest", vm));
            }
        }
    }
}
