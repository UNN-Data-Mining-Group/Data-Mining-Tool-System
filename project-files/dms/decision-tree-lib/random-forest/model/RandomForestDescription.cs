using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree.random_forest.model
{
    [Serializable()]
    public class RandomForestDescription : ISolverDescription
    {
        public long inputs, outputs;
        private int numberTrees;

        public RandomForestDescription(long inputs, long outputs, int numberTrees)
        {
            this.inputs = inputs;
            this.outputs = outputs;
            this.numberTrees = numberTrees;
        }

        public long GetInputsCount()
        {
            return inputs;
        }

        public long GetOutputsCount()
        {
            return outputs;
        }
        public int GetNumberTrees()
        {
            return numberTrees;
        }
    }
}
