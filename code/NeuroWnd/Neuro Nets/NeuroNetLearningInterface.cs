using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Neuron_definition;
using NeuroWnd.Activate_functions;
using LearningAlgorithms;

namespace NeuroWnd.Neuro_Nets
{
    public class NeuronLocation
    {
        public int Layer;
        public int Number;

        public NeuronLocation(int _layer, int _number)
        {
            Layer = _layer;
            Number = _number;
        }
    }

    public class NeuroNetLearningInterface : LearningAlgorithms.INeuroNetLearning
    {
        private DataBaseHandler dbHandler;
        private NeuroNet learned_net;
        private string netName;
        private string selectionName;

        public int CountLayers { get { return learned_net.NeuronsInLayers.GetLength(0); } }
        public int CountNeurons { get { return learned_net.NeuronsCount; } }
        public int CountInputNeurons { get { return learned_net.InputNeuronsCount; } }
        public int CountOutputNeurons { get { return learned_net.OutputNeuronsCount; } }
        public bool IsIterationsFinished { get { return learned_net.IsIterationsFinished; } }
        public bool IsWaveCameToOutputNeuron { get { return learned_net.IsWaveCameToOutputNeuron; } }

        public bool[,] get_bool_links()
        {
            return learned_net.ConnectionsOfNeurons;
        }
        public double[,] get_links()
        {
            return learned_net.WeightsOfConnections;
        }
        public void set_links(double[,] links)
        {
            learned_net.WeightsOfConnections = links;
        }
        public double get_res(double[] X)
        {
            return learned_net.MakeAnswer(X)[0];
        }
        public INeuroNetLearning copy()
        {
            return new NeuroNetLearningInterface(this);
        }
        public void write_result(string algorithm)
        {
            LoadingWindow lw = new LoadingWindow();
            lw.MakeLoading(
                    () => dbHandler.WriteLearnedWeights(netName, selectionName,
                LearningAlgorithmsLibrary.GetNameOfTypeOfAlgoritm(algorithm),
                get_links(), get_bool_links(), lw),
                "Запись обученных весов в БД");
        }

        private Neuron getNeuron(NeuronLocation location)
        {
            return learned_net.GetNeuron(getNeuronIndexInNet(location));
        }
        private int getNeuronIndexInNet(NeuronLocation location)
        {
            if (location.Layer < 1 || location.Layer > learned_net.NeuronsInLayers.GetLength(0))
                throw new Exception("Invalid index of layer");

            int index = 0;
            for (int i = 0; i < location.Layer - 1; i++)
            {
                index += learned_net.NeuronsInLayers[i];
            }

            if (index + location.Number > learned_net.NeuronsCount ||
                index + location.Number < 1)
                throw new Exception("Invalid number of neuron in layer");

            return index + location.Number - 1;
        }
        private NeuronLocation getNeuronLocation(Neuron neuron)
        {
            int index = learned_net.GetIndexNeuron(neuron);
            if (index >= 0)
            {
                int layer = 1;
                int number = 1;

                int curInd = 0;
                for (int i = 0; i < learned_net.NeuronsInLayers.GetLength(0); i++)
                {
                    int nextInd = curInd + learned_net.NeuronsInLayers[i];
                    if (index < nextInd && index >= curInd)
                    {
                        number = index - curInd + 1;
                        break;
                    }
                    curInd = nextInd;
                    layer++;
                }

                return new NeuronLocation(layer, number);
            }
            else
            {
                return null;
            }
        }

        public NeuroNetLearningInterface(NeuroNet net, string _neuroNetName, string _selectionName, DataBaseHandler _dbh)
        {
            learned_net = net;
            netName = _neuroNetName;
            selectionName = _selectionName;
            dbHandler = _dbh;
        }
        public NeuroNetLearningInterface(NeuroNetLearningInterface inn)
        {
            dbHandler = inn.dbHandler;
            learned_net = new NeuroNet(inn.learned_net);
            netName = inn.netName;
            selectionName = inn.selectionName;
        }

        public int GetCountNeuronsInLayer(int layerIndex)
        {
            if (layerIndex < 1 || layerIndex > learned_net.NeuronsInLayers.GetLength(0))
                throw new Exception("Invalid index of layer");

            return learned_net.NeuronsInLayers[layerIndex - 1];
        }
        public NeuronLocation[] GetInputsOfNeuron(NeuronLocation neuronLocation)
        {
            int indexCurNeuron = getNeuronIndexInNet(neuronLocation);
            Neuron neu = getNeuron(neuronLocation);
            NeuronLocation[] arr = new NeuronLocation[neu.InputsCount];

            int k = 0;
            for (int i = 0; i < learned_net.NeuronsCount; i++)
            {
                if (learned_net.ConnectionsOfNeurons[i, indexCurNeuron] == true)
                {
                    arr[k] = getNeuronLocation(learned_net.GetNeuron(i));
                    k++;
                }
            }

            return arr;
        }
        public NeuronLocation[] GetOutputsOfNeuron(NeuronLocation neuronLocation)
        {
            int indexCurNeuron = getNeuronIndexInNet(neuronLocation);
            Neuron neu = getNeuron(neuronLocation);
            
            int k = 0;
            for (int i = 0; i < learned_net.NeuronsCount; i++)
            {
                if (learned_net.ConnectionsOfNeurons[indexCurNeuron, i] == true)
                {
                    k++;
                }
            }

            NeuronLocation[] arr = new NeuronLocation[k];
            k = 0;
            for (int i = 0; i < learned_net.NeuronsCount; i++)
            {
                if (learned_net.ConnectionsOfNeurons[indexCurNeuron, i] == true)
                {
                    arr[k] = getNeuronLocation(learned_net.GetNeuron(i));
                }
            }

            return arr;
        }
        public bool IsConnection(NeuronLocation input, NeuronLocation output)
        {
            int indexInput = getNeuronIndexInNet(input);
            int indexOutput = getNeuronIndexInNet(output);

            return learned_net.ConnectionsOfNeurons[indexOutput, indexInput];
        }
        public double GetConnectionWeight(NeuronLocation input, NeuronLocation output)
        {
            int indexInput = getNeuronIndexInNet(input);
            int indexOutput = getNeuronIndexInNet(output);
            return learned_net.WeightsOfConnections[indexOutput, indexInput];
        }
        public Tuple<double, NeuronLocation>[] GetInputsOfNeuronWithWeights(NeuronLocation location)
        {
            NeuronLocation[] loc = GetInputsOfNeuron(location);
            Tuple<double, NeuronLocation>[] res = new Tuple<double, NeuronLocation>[loc.Length];
            for (int i = 0; i < loc.Length; i++)
            {
                res[i] = new Tuple<double, NeuronLocation>(GetConnectionWeight(loc[i], location), loc[i]);
            }
            return res;
        }
        public void ChangeConnectionWeight(NeuronLocation input, NeuronLocation output, double weight)
        {
            Neuron inp = getNeuron(input);
            Neuron oup = getNeuron(output);
            if (IsConnection(input, output) == true)
            {
                int indexOut = getNeuronIndexInNet(output);
                inp.SetWeightValue(weight, indexOut);
            }
            else
            {
                throw new Exception("Связь между нейронами не найдена");
            }
        }
        public void SetNewConnection(NeuronLocation input, NeuronLocation output, double weight)
        {
            Neuron inp = getNeuron(input);
            Neuron oup = getNeuron(output);
            if (IsConnection(input, output) == true)
            {
                throw new Exception("Связь между нейронами уже существует");
            }
            else
            {
                int indexIn = getNeuronIndexInNet(input);
                int indexOut = getNeuronIndexInNet(output);

                learned_net.SetNewConnection(indexIn, indexOut, weight);
            }
        }
        public void DeleteConnection(NeuronLocation input, NeuronLocation output)
        {
            int indexIn = getNeuronIndexInNet(input);
            int indexOut = getNeuronIndexInNet(output);

            learned_net.DeleteConnection(indexIn, indexOut);
        }
        public double GetOutputValueOfNeuron(NeuronLocation neuron)
        {
            return getNeuron(neuron).OutputValue;
        }
        public string GetNameOfAF(NeuronLocation neuron)
        {
            Neuron n = getNeuron(neuron);
            return n.ActivateFunctionOfNeuron.Name;
        }
        public bool HasAFContinuousDerivative(NeuronLocation neuron)
        {
            Neuron n = getNeuron(neuron);
            return n.ActivateFunctionOfNeuron.HasContinuousDerivative;
        }
        public int GetCountParametersAF(NeuronLocation neuron)
        {
            Neuron n = getNeuron(neuron);
            return n.ActivateFunctionOfNeuron.CountParameters;
        }
        public string[] GetNamesOfParametersAF(NeuronLocation neuron)
        {
            Neuron n = getNeuron(neuron);
            string[] res = new string[GetCountParametersAF(neuron)];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = n.ActivateFunctionOfNeuron.GetNameOfParameter(i);
            }
            return res;
        }
        public double GetValueOfParameterAF(NeuronLocation neuron, string parameterName)
        {
            Neuron n = getNeuron(neuron);
            return n.ActivateFunctionOfNeuron.GetValueOfParameter(parameterName);
        }
        public void SetValueOfParameterAF(NeuronLocation neuron, string parameterName, double value)
        {
            Neuron n = getNeuron(neuron);
            n.ActivateFunctionOfNeuron.SetValueOfParameter(parameterName, value);
        }
        public double GetAFValue(NeuronLocation neuron, double x)
        {
            Neuron n = getNeuron(neuron);
            return n.ActivateFunctionOfNeuron.Function(x);
        }
        public double GetDerivativeAFValue(NeuronLocation neuron, double x)
        {
            Neuron n = getNeuron(neuron);
            return n.ActivateFunctionOfNeuron.Derivative(x);
        }

        public void ResetNeuroNet()
        {
            learned_net.ResetNeuroNet();
        }
        public double[] MakeStep(double[] inputs)
        {
            return learned_net.MakeStep(inputs);
        }
        public double[] MakeIteration(double[] inputs)
        {
            return learned_net.MakeIteration(inputs);
        }
        public double[] MakeAnswer(double[] inputs, double eps = 1E-16)
        {
            return learned_net.MakeAnswer(inputs, eps);
        }
    }
}
