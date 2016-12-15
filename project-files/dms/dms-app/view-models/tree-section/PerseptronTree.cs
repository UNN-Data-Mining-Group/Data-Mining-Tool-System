using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class PerceptronTree : TreeSection
    {
        public PerceptronTree(string[] perceptrons, TaskTreeViewModel vm) : base("Персептроны", perceptrons) { }
    }
}
