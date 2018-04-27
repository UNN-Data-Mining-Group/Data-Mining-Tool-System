using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers.decision.tree.random_forest.model;
using dms.solvers.decision.tree;
using dms.models;

namespace dms.view_models
{
    public class RandomForestParametersViewModel : ISolverParameterViewModel
    {
        private int numberTrees = 10;
        public event Action CanCreateChanged;
        public int MaxTreeDepth { get; set; }
        public int NumberTrees
        {
            get
            {
                return numberTrees;
            }
            set
            {
                numberTrees = value;
            }
        }
        public int Inputs { get; set; }
        public int Outputs { get; set; }

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
            RandomForestDescription rfd = new RandomForestDescription(Inputs, Outputs, NumberTrees);

            TaskSolver ts = new TaskSolver()
            {
                Name = name,
                TypeName = "RandomForest",
                TaskID = task.ID,
                Description = rfd
            };
            ts.save();
        }
    }
}
