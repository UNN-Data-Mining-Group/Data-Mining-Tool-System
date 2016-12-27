using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class DecisionTreeInfoViewModel : ViewmodelBase
    {
        public string Name { get; }
        public string TaskName { get; }

        public DecisionTreeInfoViewModel(string taskName, string solverName)
        {
            TaskName = taskName;
            Name = solverName;
        }
    }
}
