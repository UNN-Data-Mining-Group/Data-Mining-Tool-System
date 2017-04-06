using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.solvers.decision_tree;
using dms.models;

namespace dms.view_models
{
    public class DecisionTreeParametersViewModel : ISolverParameterViewModel
    {
        public event Action CanCreateChanged;
        public int MaxTreeDepth { get; set; }

        public bool CanCreateSolver(string name, models.Task task)
        {
            return true;
        }

        public void CreateSolver(string name, models.Task task)
        {
            int depth = MaxTreeDepth;

            //WARNING!!!
            //Here must be selected task template 
            //and number of inputs and outputs in it.
            TreeDescription td = new TreeDescription(task.ParamCount - 1, 1, depth);
            
            TaskSolver ts = new TaskSolver()
            {
                Name = name,
                TypeName = "DecisionTree",
                TaskID = task.ID,
                Description = td
            };
            ts.save();
        }
    }
}
