using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class DecisionTreesTree : TreeSection
    {
        public DecisionTreesTree(string[] trees, TaskTreeViewModel vm) : base("Деревья решений", trees) { }
    }
}
