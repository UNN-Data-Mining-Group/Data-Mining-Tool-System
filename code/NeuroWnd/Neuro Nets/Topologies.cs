using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NeuroWnd.Neuro_Nets
{
    public abstract class Topology
    {
        public Topology()
        {
        }
        public abstract bool[,] CreateNet(int countNeurons, int countLayers, int[] neuronsInLayers);
        public abstract string Name { get; }
    }

    public class Perceptron : Topology
    {
        public override string Name
        {
            get { return "Персептрон"; }
        }
        public Perceptron()
        {
        }
        public override bool[,] CreateNet(int countNeurons, int countLayers, int[] neuronsInLayers)
        {
            if (countNeurons <= 0)
                throw new Exception("Invalid count of neurons");
            if (countLayers < 2 || countLayers > countNeurons)
                throw new Exception("Invalid count of layers");
            if (neuronsInLayers.Length != countLayers)
                throw new Exception("Invalid dimension of 'neurons in layers' array");

            int sum = 0;
            for (int i = 0; i < neuronsInLayers.Length; i++)
            {
                if (neuronsInLayers[i] <= 0)
                    throw new Exception(String.Format("Invalid count of neurons in {0} layer", i + 1));
                sum += neuronsInLayers[i];
            }
            if (sum != countNeurons)
                throw new Exception("Invalid sum of count of neurons in array");

            bool[,] connections = new bool[countNeurons, countNeurons];
            for (int i = 0; i < countNeurons; i++)
            {
                for (int j = 0; j < countNeurons; j++)
                {
                    connections[i, j] = false;
                }
            }

            int out_neuron_index = 0;
            int in_neuron_index = neuronsInLayers[0];
            for (int i = 0; i < countLayers - 1; i++)
            {
                for (int j = 0; j < neuronsInLayers[i]; j++)
                {
                    for (int k = 0; k < neuronsInLayers[i + 1]; k++)
                    {
                        connections[out_neuron_index, in_neuron_index] = true;
                        in_neuron_index++;
                    }
                    out_neuron_index++;
                    in_neuron_index -= neuronsInLayers[i + 1];
                }
                in_neuron_index += neuronsInLayers[i + 1];
            }

            return connections;
        }
    }

    public static class LibraryOfTopologies
    {
        private static List<Type> topologyTypes;
        public enum GetterParameter { TopologyName, TypeOfTopologyName };

        static LibraryOfTopologies()
        {
            Type ourtype = typeof(Topology);
            IEnumerable<Type> en = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype));
            if (en == null)
            {
                throw new Exception("Empty list of topologies");
            }
            topologyTypes = new List<Type>(en);
        }

        public static Topology GetTopology(string name, GetterParameter par)
        {
            switch (par)
            {
                case GetterParameter.TopologyName:
                    foreach (Type item in topologyTypes)
                    {
                        Topology tp = (Topology)Activator.CreateInstance(item);
                        if (String.Compare(tp.Name, name) == 0)
                        {
                            return tp;
                        }
                    }
                    throw new Exception("Invalid topology name");

                case GetterParameter.TypeOfTopologyName:
                    foreach (Type item in topologyTypes)
                    {
                        if (String.Compare(item.Name, name) == 0)
                        {
                            return (Topology)Activator.CreateInstance(item);
                        }
                    }
                    throw new Exception("Invalid topology type name");

                default:
                    throw new Exception("Invalid mode");
            }
        }
        public static List<string> GetAllTopologyTypeNames()
        {
            List<string> ls = new List<string>();
            foreach (Type item in topologyTypes)
            {
                Topology tp = (Topology)Activator.CreateInstance(item);
                ls.Add(tp.Name);
            }
            return ls;
        }
        public static string GetTopologyName(string typeName)
        {
            foreach (Type item in topologyTypes)
            {
                if (String.Compare(item.Name, typeName) == 0)
                {
                    return ((Topology)Activator.CreateInstance(item)).Name;
                }
            }
            throw new Exception("Invalid type name");
        }
        public static string GetTopologyTypeName(string topologyName)
        {
            foreach (Type item in topologyTypes)
            {
                Topology tp = (Topology)Activator.CreateInstance(item);
                if (String.Compare(tp.Name, topologyName) == 0)
                {
                    return item.Name;
                }
            }
            throw new Exception("Invalid topology name");
        }
    }
}
