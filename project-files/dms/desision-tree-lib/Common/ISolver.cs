using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desision_tree_lib.Common
{
    interface ISolver
    {
        float[] Solve(float[] x);
        int getInputsCount();
        int getOutputsCount();
    }
}
