using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Activate_functions;

namespace NeuroWnd.Neuro_Nets
{
    public class NeuroNetDefinition
    {
        private string name;
        private string topologyTypeName;
        private string task_name;
        private int neurons_count;
        private int layer_count;
        private int[] neurons_in_layer;
        private string activate_function;
        private double[] af_parameters;

        public string Name { get { return name; } }
        public string TopologyName { get { return topologyTypeName; } }
        public string TaskName { get { return task_name; } }
        public int NeuronsCount { get { return neurons_count; } }
        public int LayerCount { get { return layer_count; } }
        public int[] NeuronsInLayer { get { return neurons_in_layer; } }
        public string ActivateFunction { get { return activate_function; } }
        public double[] AFParameters { get { return af_parameters; } }

        public NeuroNetDefinition(string[] definition)
        {
            if (definition.Length != 8)
                throw new Exception("Definition is not valid");

            name = definition[0];
            topologyTypeName = definition[1];
            task_name = definition[2];
            neurons_count = Convert.ToInt32(definition[3]);
            layer_count = Convert.ToInt32(definition[4]);

            neurons_in_layer = new int[layer_count];
            int k = 0;
            for (int i = 0; i < layer_count; i++)
            {
                string buf = "";
                while (k < definition[5].Length && definition[5][k] != ' ')
                {
                    buf += definition[5][k];
                    k++;
                }
                k++;
                if (buf.Length != 0)
                    neurons_in_layer[i] = Convert.ToInt32(buf);
            }

            activate_function = definition[6];

            af_parameters = new double[LibraryOfActivateFunctions.GetCountParametersOfAF(activate_function, 
                LibraryOfActivateFunctions.GetterParameter.ActivateFunctionName)];
            k = 0;
            for (int i = 0; i < af_parameters.Length; i++)
            {
                string buf = "";
                while (k < definition[7].Length && definition[7][k] != ' ')
                {
                    buf += definition[7][k];
                    k++;
                }
                k++;
                if (buf.Length != 0)
                    af_parameters[i] = Convert.ToDouble(buf);
            }

        }
    }
}
