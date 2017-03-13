using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.decision_tree
{
    [Serializable()]
    public class Node
    {
        public Node left_child;
        public Node right_child;
        public Rule rule;
        public bool is_leaf;


        public Node(Node l_child = null, Node r_child = null, Rule new_rule = null, bool isleaf = false)
        {
            left_child = l_child;
            right_child = r_child;
            rule = new_rule;
            is_leaf = isleaf;
        }

        public bool IsLeaf()
        {
            return is_leaf;
        }
    }
}
