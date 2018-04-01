﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers.decision.tree;

namespace dms.view_models
{
    public class DecisionTreeCARTInfoViewModel : ViewmodelBase
    {
        public string Name { get; }
        public string TaskName { get; }
        public int MaxTreeDepth { get; }

        public DecisionTreeCARTInfoViewModel(models.Task task, models.TaskSolver solver)
        {
            TaskName = task.Name;
            Name = solver.Name;

            TreeDescription td = solver.Description as TreeDescription;
            MaxTreeDepth = td.MaxDepth;
        }
    }
}
