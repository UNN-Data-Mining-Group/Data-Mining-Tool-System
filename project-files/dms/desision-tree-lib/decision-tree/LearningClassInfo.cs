using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desision_tree_lib.decision_tree
{
    class LearningClassInfo
    {
        public String class_name;
        public int number_of_checked;

        public LearningClassInfo()
        {
            number_of_checked = 0;
            class_name = "";
        }
    }
}
