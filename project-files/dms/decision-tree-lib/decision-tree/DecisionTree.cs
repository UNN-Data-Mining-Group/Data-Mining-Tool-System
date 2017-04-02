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
        Int64 inputsCount;
        Int64 outputCount;
        public Node root;

        public DecisionTree(TreeDescription treeDesc) : base(treeDesc)
        {
            inputsCount = treeDesc.GetInputsCount();
            outputCount = treeDesc.GetOutputsCount();
            root = new Node();
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

        public void treeBuilding(LearningTable education_table, Node tree_node)
        {
            LearningClassInfo[] thisClassInfo = education_table.ClassInfoInit(education_table, 0, education_table.LearningClasses.Length);
            
            int k = 0;
            foreach (LearningClassInfo clinf in thisClassInfo)
            {
                if (clinf.number_of_checked >= 1)
                {
                    k++;
                }
            }

            if (k >= 2)
            {
                LearningTable left_table = new LearningTable();
                LearningTable right_table = new LearningTable();
                tree_node.is_leaf = false;
                int index_of_parametr = 0;
                string best_value_for_split = "";
                left_table.FindBetterParameter(education_table, ref index_of_parametr, ref best_value_for_split);
                tree_node.rule = new Rule();
                tree_node.rule.index_of_param = index_of_parametr;
                tree_node.rule.value = (float)Convert.ToDouble(best_value_for_split);
                tree_node.left_child = new Node();
                tree_node.right_child = new Node();
                
                left_table.SplitLearningTable(education_table, tree_node.rule, ref left_table, ref right_table);
                treeBuilding(left_table, tree_node.left_child);
                treeBuilding(right_table, tree_node.right_child);


            }
            else
            {
                tree_node.is_leaf = true;
                tree_node.rule = new Rule();
                foreach (LearningClassInfo clinf in thisClassInfo)
                {
                    if (clinf.number_of_checked > 0)
                    {
                        tree_node.rule.value = clinf.class_name;
                    }
                }

            }
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
