using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using desision_tree_lib.Common;
using dms.models;

namespace desision_tree_lib.decision_tree
{
    class DecisionTree : ISolver
    {
        public float[] Solve(float[] x)
        {
            return null;
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
            LearningClassInfo[] thisClassInfo = ClassInfoInit();//ClassInfoInit2(education_table);
            for (int i = 0; i < education_table.Rows.Count; i++)
            {
                foreach (LearningClassInfo clinf in thisClassInfo)
                {
                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                    {
                        clinf.number_of_checked++;
                    }
                }
            }
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
                tree_node.is_leaf = false;
                int index_of_parametr = 0;
                string best_value_for_split = "";
                Parameter param = new Parameter();
                LearningTable.FindBetterParameter(education_table, ref index_of_parametr, ref best_value_for_split, ref param);
                tree_node.rule = new Rule();
                tree_node.rule.index_of_param = index_of_parametr;
                tree_node.rule.value = best_value_for_split;
                tree_node.left_child = new Node();
                tree_node.right_child = new Node();
                LearningTable left_table = new LearningTable();
                LearningTable right_table = new LearningTable();
                SplitLearningTable(education_table, tree_node.rule, param, ref left_table, ref right_table);
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

    }
}
