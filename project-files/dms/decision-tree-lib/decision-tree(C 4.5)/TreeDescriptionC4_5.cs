using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers;

namespace dms.solvers.decision_tree.C4_5
{
    [Serializable()]
    public class TreeDescriptionC4_5 : ISolverDescription
    {
        public int MaxDepth;
        public long inputs, outputs;

        public TreeDescriptionC4_5(long inputs, long outputs, int max_depth)
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
