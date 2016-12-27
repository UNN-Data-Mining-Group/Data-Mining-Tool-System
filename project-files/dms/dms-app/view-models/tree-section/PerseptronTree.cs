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
        public PerceptronTree(string taskName, string[] perceptrons, 
            TaskTreeViewModel vm) : base("Персептроны", perceptrons)
        {
            Content = new ObservableCollection<TreeSection>();
            for(int i = 0; i < perceptrons.Length; i++)
            {
                Content.Add(new SolverLeaf(taskName, perceptrons[i], "Персептрон", vm));
            }
        }
    }
}
