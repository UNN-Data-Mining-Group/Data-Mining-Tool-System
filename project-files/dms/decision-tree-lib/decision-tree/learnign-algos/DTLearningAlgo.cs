using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree.algo
{
    public interface DTLearningAlgo
    {
        float[] getParams();
        string[] getParamsNames();
        float startLearn(ISolver solver, float[][] train_x, float[] train_y);
        string getType();
    }
}
