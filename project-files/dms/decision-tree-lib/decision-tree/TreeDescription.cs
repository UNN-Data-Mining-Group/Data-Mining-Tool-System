using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers;

namespace dms.solvers.decision.tree
{
    [Serializable()]
    public class TreeDescription : ISolverDescription
    {
        public int MaxDepth;
        public long inputs, outputs;

        public TreeDescription(long inputs, long outputs, int max_depth)
        {
            MaxDepth = max_depth;
            this.inputs = inputs;
            this.outputs = outputs;
        }

        public long GetInputsCount()
        {
            return inputs;
        }

        public long GetOutputsCount()
        {
            return outputs;
        }
    }
}
