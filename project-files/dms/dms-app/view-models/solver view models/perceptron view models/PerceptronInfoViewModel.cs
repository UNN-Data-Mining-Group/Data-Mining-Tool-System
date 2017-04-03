using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.models;
using dms.solvers.neural_nets;
using dms.solvers.neural_nets.perceptron;

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
        public int CountInputNeurons { get; }
        public string TaskName { get; }

        public PerceptronInfoViewModel(models.Task task, models.TaskSolver solver)
        {
            TaskName = task.Name;
            Name = solver.Name;

            PerceptronTopology topology = solver.Description as PerceptronTopology;

            Layers = new Layer[topology.GetLayersCount() - 1];
            var neurons = topology.GetNeuronsInLayersCount();
            var delays = topology.HasLayersDelayWeight();
            var afs = topology.GetActivationFunctionsNames();

            CountInputNeurons = Convert.ToInt32(topology.GetInputsCount());
            for (int i = 0; i < Layers.Length - 1; i++)
            {
                Layers[i] = new Layer
                {
                    Name = String.Format("{0} слой", i + 1),
                    ActivateFunction = afs[i],
                    NeuronsCount = neurons[i+1],
                    HasW0 = delays[i]
                };
            }
            Layers[Layers.Length - 1] = new Layer
            {
                Name = "Выходной слой",
                ActivateFunction = afs[Layers.Length - 1],
                NeuronsCount = neurons[Layers.Length],
                HasW0 = delays[Layers.Length - 1]
            };
        }

        public Layer[] Layers { get; }
    }
}
