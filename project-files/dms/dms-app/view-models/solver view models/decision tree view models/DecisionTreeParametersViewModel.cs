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

        public bool CanCreateSolver(string name, models.Task task)
        {
            return false;
        }

        public void CreateSolver(string name, models.Task task)
        {
            throw new NotImplementedException();
        }
    }
}
