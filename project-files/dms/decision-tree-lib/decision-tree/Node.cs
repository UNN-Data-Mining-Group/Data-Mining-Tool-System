using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree
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

        public Node Copy()
        {
            Node newNODE = new Node();
            if (!this.is_leaf)
            {
                newNODE.left_child = this.left_child.Copy();
                newNODE.right_child = this.right_child.Copy();
            }
            newNODE.rule = new Rule(this.rule.index_of_param, this.rule.value);
            newNODE.is_leaf = this.is_leaf;
            return newNODE;
        }

        public bool IsLeaf()
        {
            return is_leaf;
        }
    }
}
