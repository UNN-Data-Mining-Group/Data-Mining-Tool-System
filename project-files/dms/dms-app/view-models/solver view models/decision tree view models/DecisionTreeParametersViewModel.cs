using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class DecisionTreeParametersViewModel : ISolverParameterViewModel
    {
        public event Action CanCreateChanged;

        public bool CanCreateSolver(string name, string taskName)
        {
            return false;
        }

        public void CreateSolver(string name, string taskName)
        {
            throw new NotImplementedException();
        }
    }
}
