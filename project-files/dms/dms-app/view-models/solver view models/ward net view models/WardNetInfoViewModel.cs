using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.solvers.neural_nets.ward_net;

namespace dms.view_models
{
    public class WardNetGroup
    {
        public string LayerName { get; set; }
        public string GroupName { get; set; }
        public Int64 NeuronsCount { get; set; }
        public string ActivateFunction { get; set; }
        public bool HasW0 { get; set; }
    }

    public class AdditionalConnection
    {
        public string Start { get; set; }
        public string End { get; set; }
    }

    public class WardNetInfoViewModel : ViewmodelBase
    {
        public string TaskName { get; }
        public string Name { get; }
        public Int64 CountInputNeurons { get; }
        public WardNetGroup[] Groups { get; }
        public AdditionalConnection[] AdditionalConns { get; }

        public WardNetInfoViewModel(models.Task task, models.TaskSolver solver)
        {
            TaskName = task.Name;
            Name = solver.Name;

            WardNNTopology t = solver.Description as WardNNTopology;
            var input_layer = t.GetInputLayer();
            var layers = t.GetLayers();

            int gsize = 0;
            int adsize = (input_layer.ForwardConnection > 0) ? 1 : 0;
            for (int i = 0; i < layers.Count; i++)
            {
                gsize += layers[i].Groups.Count;
                if (layers[i].ForwardConnection > 0)
                    adsize++;
            }

            CountInputNeurons = input_layer.NeuronsCount;
            Groups = new WardNetGroup[gsize];
            AdditionalConns = new AdditionalConnection[adsize];

            int gindex = 0;
            int last_layer_index = layers.Count - 1;
            for(int i = 0; i < last_layer_index; i++)
            {
                for(int j = 0; j < layers[i].Groups.Count; j++)
                {
                    Groups[gindex++] = new WardNetGroup
                    {
                        LayerName = String.Format("{0} слой", i + 1),
                        GroupName = String.Format("{0} группа", j + 1),
                        NeuronsCount = layers[i].Groups[j].NeuronsCount,
                        ActivateFunction = layers[i].Groups[j].ActivationFunction,
                        HasW0 = layers[i].Groups[j].HasDelay
                    };
                }
            }

            for (int j = 0; j < layers[last_layer_index].Groups.Count; j++)
            {
                Groups[gindex++] = new WardNetGroup
                {
                    LayerName = "Выходной слой",
                    GroupName = String.Format("{0} группа", j + 1),
                    NeuronsCount = layers[last_layer_index].Groups[j].NeuronsCount,
                    ActivateFunction = layers[last_layer_index].Groups[j].ActivationFunction,
                    HasW0 = layers[last_layer_index].Groups[j].HasDelay
                };
            }

            int adindex = 0;
            if (input_layer.ForwardConnection > 0)
            {
                AdditionalConns[adindex++] = new AdditionalConnection
                {
                    Start = String.Format("Входной слой"),
                    End = String.Format("{0} слой", input_layer.ForwardConnection + 1)
                };
            }
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].ForwardConnection > 0)
                {
                    AdditionalConns[adindex++] = new AdditionalConnection
                    {
                        Start = String.Format("{0} слой", i + 1),
                        End = String.Format("{0} слой", i + 1 + layers[i].ForwardConnection + 1)
                    };
                }
            }
        }
    }
}
