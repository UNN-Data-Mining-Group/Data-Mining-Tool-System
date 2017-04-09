using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.solvers.neural_nets.kohonen;

namespace dms.view_models
{
    public class KohonenParametersViewModel : ISolverParameterViewModel
    {
        public event Action CanCreateChanged;
        private string classEps;

        public KohonenParametersViewModel()
        {
            Inputs = 1;
            Outputs = 1;
            Width = 1;
            Height = 1;
            SelectedMetric = Metrics[0];
            SelectedInitializer = ClassInitializers[0];
            ClassEps = "1e-5";
        }

        public int Inputs { get; set; }
        public int Outputs { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string ClassEps
        {
            get { return classEps; }
            set
            {
                classEps = value;
                CanCreateChanged?.Invoke();
            }
        }
        public string[] Metrics { get { return KohonenNNTopology.GetAvaliableMetrics(); } }
        public string SelectedMetric { get; set; }
        public string[] ClassInitializers { get { return KohonenNNTopology.GetClassInitializerList(); } }
        public string SelectedInitializer { get; set; }

        public bool CanCreateSolver(string name, models.Task task)
        {
            float eps;
            return float.TryParse(ClassEps, out eps);
        }

        public void CreateSolver(string name, models.Task task)
        {
            KohonenNNTopology t = new KohonenNNTopology(Inputs, Outputs, 
                Width, Height, float.Parse(ClassEps),
                SelectedInitializer, SelectedMetric);
            TaskSolver solver = new TaskSolver()
            {
                Name = name,
                TaskID = task.ID,
                Description = t,
                TypeName = "KohonenNet"
            };
            solver.save();
        }
    }
}
