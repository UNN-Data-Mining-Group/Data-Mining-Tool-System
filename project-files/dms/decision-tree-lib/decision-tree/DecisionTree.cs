using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree
{
    [Serializable()]
    public class DecisionTree : ISolver
    {
        public Int64 inputsCount;
        public Int64 outputCount;
        public Node root;
        public int maxDepth;
        private TreeDescription treeDesc;

        public DecisionTree(TreeDescription treeDesc) : base(treeDesc)
        {
            inputsCount = treeDesc.GetInputsCount();
            outputCount = treeDesc.GetOutputsCount();
            maxDepth = treeDesc.MaxDepth;
            root = new Node();
        }

        public float treeSolve(float[] x, Node curNode)
        {
            while (curNode.is_leaf != true)
            {
                if (x[curNode.rule.index_of_param] <= curNode.rule.value)
                {
                    curNode = curNode.right_child;
                }
                else
                {
                    curNode = curNode.left_child;
                }
            }
            return curNode.rule.value;
        }


        public override float[] Solve(float[] x)
        {
            float[] res = new float[outputCount];
            res[0] = treeSolve(x, root);
            return res;
        }

        public override ISolver Copy()
        {
            TreeDescription dtDescr = new TreeDescription(this.GetInputsCount(), this.GetOutputsCount(), this.maxDepth);
            DecisionTree newDT = new DecisionTree(dtDescr);
            newDT.root = this.root.Copy();
            return newDT;
        }
    }
}