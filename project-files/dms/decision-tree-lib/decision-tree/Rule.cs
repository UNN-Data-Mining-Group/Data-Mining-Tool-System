using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree
{
    [Serializable()]
    public class Rule
    {
        public int index_of_param;
        public float value;

        public Rule(int _param = 0, float _value = 0)
        {
            index_of_param = _param;
            value = _value;
        }

        public void SetRule(int ind_param, float rule_val)
        {
            index_of_param = ind_param;
            value = rule_val;
        }
    }
}

