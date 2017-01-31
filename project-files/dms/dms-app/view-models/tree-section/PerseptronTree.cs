using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class PerceptronTree : TreeSection
    {
        public PerceptronTree(models.Task task, models.TaskSolver[] perceptrons, 
            TaskTreeViewModel vm) : base("Персептроны", perceptrons.Select(x => x.Name).ToArray())
        {
            Content = new ObservableCollection<TreeSection>();
            for(int i = 0; i < perceptrons.Length; i++)
            {
                Content.Add(new SolverLeaf(task, perceptrons[i], "Персептрон", vm));
            }
        }
    }
}
