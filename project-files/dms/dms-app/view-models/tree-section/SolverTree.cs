using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class SolverTree : TreeSection
    {
        public SolverTree(string[] per, string[] des, TaskTreeViewModel vm) : base("Решатели")
        {
            Content = new ObservableCollection<TreeSection>
            {
                new PerceptronTree(per, vm),
                new DecisionTreesTree(des, vm)
            };
        }
    }
}
