using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.solvers.neural_nets;

namespace dms.view_models
{
    public class WardNetGroup
    {
        public string LayerName { get; set; }
        public string GroupName { get; set; }
        public int NeuronsCount { get; set; }
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
        public int CountInputNeurons { get; }
        public WardNetGroup[] Groups { get; }
        public AdditionalConnection[] AdditionalConns { get; }

        public WardNetInfoViewModel(models.Task task, models.TaskSolver solver)
        {
            TaskName = task.Name;
            Name = solver.Name;

            WardNNTopology t = solver.Description as WardNNTopology;
            var groups = t.GetGroupsCount();
            var adcons = t.GetAdditionalConnections();
            var afs = t.GetActivationFunctions();
            var neurons = t.GetNeuronsCount();
            var w0s = t.GetDelays();

            int gsize = 0;
            for(int i = 0; i < t.getLayersCount() - 1; i++)
                gsize += groups[i];

            CountInputNeurons = neurons[0][0];
            Groups = new WardNetGroup[gsize];

            int adsize = 0;
            for (int i = 0; i < adcons.Length; i++)
                if (adcons[i] > 0)
                    adsize++;

            AdditionalConns = new AdditionalConnection[adsize];

            int gindex = 0;
            int last_layer_index = t.getLayersCount() - 1;
            for(int i = 1; i < last_layer_index; i++)
            {
                for(int j = 0; j < groups[i-1]; j++)
                {
                    Groups[gindex++] = new WardNetGroup
                    {
                        LayerName = String.Format("{0} слой", i),
                        GroupName = String.Format("{0} группа", j + 1),
                        NeuronsCount = neurons[i][j],
                        ActivateFunction = afs[i-1][j],
                        HasW0 = w0s[i-1][j]
                    };
                }
            }

            for (int j = 0; j < groups[last_layer_index - 1]; j++)
            {
                Groups[gindex++] = new WardNetGroup
                {
                    LayerName = "Выходной слой",
                    GroupName = String.Format("{0} группа", j + 1),
                    NeuronsCount = neurons[last_layer_index][j],
                    ActivateFunction = afs[last_layer_index - 1][j],
                    HasW0 = w0s[last_layer_index - 1][j]
                };
            }

            int adindex = 0;
            for (int i = 0; i < adcons.Length; i++)
            {
                if (adcons[i] > 0)
                {
                    AdditionalConns[adindex++] = new AdditionalConnection
                    {
                        Start = String.Format("{0} слой", i),
                        End = String.Format("{0} слой", i + adcons[i] + 1)
                    };
                }
            }
        }
    }
}
