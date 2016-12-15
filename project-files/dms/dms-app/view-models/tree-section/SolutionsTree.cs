using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class SolutionsTree : TreeSection
    {
        public SolutionsTree(string[] solutions, TaskTreeViewModel vm) : base("Решения", solutions) { }
    }
}
