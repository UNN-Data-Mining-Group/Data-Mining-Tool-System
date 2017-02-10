using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desision_tree_lib.decision_tree
{
    class Rule
    {
        public int index_of_param;
        public String value;

        public Rule(int _param = 0, String _value = "")
        {
            index_of_param = _param;
            value = _value;
        }
    }
}
