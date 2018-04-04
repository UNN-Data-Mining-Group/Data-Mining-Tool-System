using dms.solvers.decision.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree.algo
{
    public class NodeLst
    {
        public List<NodeLst> childs;
        public Rule rule;
        public bool is_leaf;

        public NodeLst(List<NodeLst> new_childs = null, Rule new_rule = null, bool isleaf = false)
        {
            childs = new_childs;
            rule = new_rule;
            is_leaf = isleaf;
        }

        public NodeLst Copy()
        {
            NodeLst newNODE = new NodeLst();
            
            if (!this.is_leaf)
            {
                newNODE.childs = new List<NodeLst>();
                foreach (var child in childs)
                {
                    newNODE.childs.Add(child.Copy());
                } 
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
