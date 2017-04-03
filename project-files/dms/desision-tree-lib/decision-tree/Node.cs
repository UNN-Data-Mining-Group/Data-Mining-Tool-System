using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desision_tree_lib.decision_tree
{
    class Node
    {
        public Node left_child;
        public Node right_child;
        public Rule rule;
        public bool is_leaf;
        public int index_of_param;
        public String rule_value;
        public LearningClassInfo[] checked_classes;


        public Node(Node l_child = null, Node r_child = null, Rule new_rule = null, bool isleaf = false) //VeryfiedClassInfo[] ch_classes=null)
        {
            left_child = l_child;
            right_child = r_child;
            rule = new_rule;
            is_leaf = isleaf;   // вернуть Rule
            index_of_param = -1;
            rule_value = "";
            checked_classes = null;
        }

        public void SetRule(int ind_param, String rule_val)
        {
            index_of_param = ind_param;
            rule_value = rule_val;
        }

        public void SetLearningClassInfo(LearningClassInfo[] check_classes)
        {
            checked_classes = check_classes;
        }

        public bool IsLeaf()
        {
            return is_leaf;
        }
    }
}
