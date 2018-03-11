using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision_tree.C4_5
{
    [Serializable()]
    public class DecisionTreeC4_5 : DecisionTree
    {
        public DecisionTreeC4_5(TreeDescriptionC4_5 treeDesc) : base(treeDesc)
        {
            inputsCount = treeDesc.GetInputsCount();
            outputCount = treeDesc.GetOutputsCount();
            maxDepth = treeDesc.MaxDepth;
            root = new Node();
        }

        public override ISolver Copy()
        {
            TreeDescriptionC4_5 dtDescr = new TreeDescriptionC4_5(this.GetInputsCount(), this.GetOutputsCount(), this.maxDepth);
            DecisionTreeC4_5 newDT = new DecisionTreeC4_5(dtDescr);
            newDT.root = this.root.Copy();
            return newDT;
        }
    }
}
