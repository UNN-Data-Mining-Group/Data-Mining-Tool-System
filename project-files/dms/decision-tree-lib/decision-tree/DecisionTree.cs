using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers;

namespace dms.solvers.decision_tree
{
    [Serializable()]
    public class DecisionTree : ISolver
    {
        
        int maxDepth;
        Int64 inputsCount;
        Int64 outputCount;

        public Node root;

        public DecisionTree(TreeDescription treeDesc) : base(treeDesc)
        {
            inputsCount = treeDesc.GetInputsCount();
            outputCount = treeDesc.GetOutputsCount();
            root = new Node();
        }

        public DecisionTree(TreeDescription description, long inputs, long outputs) : base(inputs, outputs)
        {
            root = new Node();
            maxDepth = description.MaxDepth;
        }

        public  float treeSolve(float[] x, Node curNode)
        {
            while (curNode.is_leaf != true)
            {
                if ( x[curNode.rule.index_of_param] <= curNode.rule.value)
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

        public int getInputsCount()
        {
            return 0;
        }
        public int getOutputsCount()
        {
            return 0;
        }

        public override float[] Solve(float[] x)
        {
            float[] res = new float[outputCount];
            res[0] = treeSolve(x, root);
            return res;
        }

        public override ISolver Copy()
        {
            throw new NotImplementedException();
        }
    }
}
