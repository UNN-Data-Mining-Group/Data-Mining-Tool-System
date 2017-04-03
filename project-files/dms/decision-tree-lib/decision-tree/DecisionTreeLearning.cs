using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision_tree
{
    class DecisionTreeLearning
    {

        public float startLearn(ISolver solver, float[][] train_x, float[] train_y)
        {
            if (solver.GetType() == typeof(DecisionTree))
            {
                DecisionTree dc_solver = (DecisionTree)solver;
                treeBuilding(new LearningTable(train_x, train_y), dc_solver.root);
                solver = dc_solver;
            }
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
    }
}
