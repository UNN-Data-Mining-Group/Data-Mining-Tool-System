using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers;

namespace dms.decision_tree
{
    [Serializable()]
    public class TreeDescription : ISolverDescription
    {
        public int MaxDepth;

        public TreeDescription(int max_depth)
        {
            MaxDepth = max_depth;
        }
    }
}
