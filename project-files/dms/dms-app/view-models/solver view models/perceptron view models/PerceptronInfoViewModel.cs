using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.models;

namespace dms.view_models
{
    public class Layer
    {
        public string Name { get; set; }
        public int NeuronsCount { get; set; }
        public string ActivateFunction { get; set; }
        public bool HasW0 { get; set; }
    }

    public class PerceptronInfoViewModel : ViewmodelBase
    {
        public string Name { get; }
        public int CountInputNeurons { get { return 10; } }
        public string TaskName { get; }

        public PerceptronInfoViewModel(string taskName, string solverName)
        {
            TaskName = taskName;
            Name = solverName;
 
            Layers = new Layer[]
            {
                new Layer { Name = "1 слой", NeuronsCount = 5, ActivateFunction = "Сигмоидальная", HasW0 = true},
                new Layer { Name = "2 слой", NeuronsCount = 15, ActivateFunction = "Сигмоидальная", HasW0 = true},
                new Layer { Name = "Выходной слой", NeuronsCount = 3, ActivateFunction = "Пороговая", HasW0 = false}
            };
        }

        public Layer[] Layers { get; }
    }
}
