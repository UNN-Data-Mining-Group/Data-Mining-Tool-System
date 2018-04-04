using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree
{
    public class LearningClassInfo
    {
        public float class_name;
        public int number_of_checked;

        public LearningClassInfo()
        {
            number_of_checked = 0;
            class_name = -1;
        }
    }
}
