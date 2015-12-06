using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Neuron_definition;
using NeuroWnd.Activate_functions;

namespace NeuroWnd.Neuro_Nets
{
    public class NeuroNet
    {
        private Neuron[] neurons;
        private InputNeuron[] input_neurons;
        private OutputNeuron[] output_neurons;
        private Queue<Queue<Neuron>> evaluation_machine;
        private bool isIterationsFinished;
        private bool isWaveCameToOutputNeuron;
        private int[] neuronsInLayers;

        private bool[,] topology;
        private double[,] weights;

        public int NeuronsCount { get { return neurons.Length; } }
        public int InputNeuronsCount { get { return input_neurons.Length; } }
        public int OutputNeuronsCount { get { return output_neurons.Length; } }
        public bool IsIterationsFinished { get { return isIterationsFinished; } }
        public bool IsWaveCameToOutputNeuron { get { return isWaveCameToOutputNeuron; } }
    
        public int[] NeuronsInLayers 
        { 
            get 
            { 
                int[] res = new int[neuronsInLayers.Length];
                for(int i = 0; i < res.Length; i++)
                {
                    res[i] = neuronsInLayers[i];
                }
                return res; 
            } 
        }
        public bool[,] ConnectionsOfNeurons
        {
            get
            {
                return topology;
            }
        }
        public double[,] WeightsOfConnections
        {
            get
            {
                return weights;
            }
            set
            {
                weights = value;
                setInputConnections();
            }
        }

        private void addInputNeuronsToFirstPool()
        {
            if (evaluation_machine.Count > 0)
            {
                Queue<Neuron> first_pool = evaluation_machine.Peek();
                foreach (Neuron item in input_neurons)
                {
                    if (first_pool.Contains(item) == false)
                    {
                        first_pool.Enqueue(item);
                    }
                }
            }
            else
            {
                Queue<Neuron> first_pool = new Queue<Neuron>();
                foreach (Neuron item in input_neurons)
                {
                    first_pool.Enqueue(item);
                }
                evaluation_machine.Enqueue(first_pool);
            }
        }
        private void setInputConnections()
        {
            for (int indexNeuron = 0; indexNeuron < NeuronsCount; indexNeuron++)
            {
                int count = 0;
                for (int i = 0; i < NeuronsCount; i++)
                {
                    if (topology[i, indexNeuron] == true)
                    {
                        count++;
                    }
                }

                NeuronInputConnection[] res = new NeuronInputConnection[count];
                count = 0;
                for (int i = 0; i < NeuronsCount; i++)
                {
                    if (topology[i, indexNeuron] == true)
                    {
                        res[count] = new NeuronInputConnection(weights[i, indexNeuron], i);
                        count++;
                    }
                }
                neurons[indexNeuron].SetInputConnections(res);
            }
        }

        public NeuroNet(int numb_input_neurons, int numb_output_neurons, int[] numbNeuronsInLayers,
            bool[,] _connections, double[,] _weights, ActivateFunction af)
        {
            if(_connections.GetLength(0) != _connections.GetLength(1) ||
                _weights.GetLength(0) != _weights.GetLength(1) ||
                _connections.GetLength(0) != _weights.GetLength(0))
            {
                throw new Exception("Invalid dimensions of connection or weight matrices");
            }

            int countNeurons = _connections.GetLength(0);
            if (numb_input_neurons <= 0 || numb_input_neurons >= countNeurons)
                throw new Exception("Number of input neurons is out of range");
            if (numb_output_neurons <= 0 || numb_output_neurons >= countNeurons)
                throw new Exception("Number of output neurons is out of range");
            if(af == null)
            {
                throw new Exception("Not initialized activate function");
            }
            if (numbNeuronsInLayers.GetLength(0) < 2 || numbNeuronsInLayers.GetLength(0) > countNeurons)
                throw new Exception("Invalid count of layers");

            int sum = 0;
            foreach (int item in numbNeuronsInLayers)
            {
                if (item <= 0)
                    throw new Exception("Must be at least one neuron in each layer");
                sum += item;
            }
            if(sum != countNeurons)
                throw new Exception("Invalid count of neurons in layers");

            neuronsInLayers = numbNeuronsInLayers;

            input_neurons = new InputNeuron[numb_input_neurons];
            neurons = new Neuron[countNeurons];
            output_neurons = new OutputNeuron[numb_output_neurons];
            topology = new bool[countNeurons, countNeurons];
            weights = new double[countNeurons, countNeurons];

            evaluation_machine = new Queue<Queue<Neuron>>();

            for (int i = 0; i < numb_input_neurons; i++)
            {
                input_neurons[i] = new InputNeuron(af);
                neurons[i] = input_neurons[i];
            }
            for (int i = numb_input_neurons; i < countNeurons - numb_output_neurons; i++)
            {
                neurons[i] = new Neuron(af);
            }
            for (int i = 0; i < numb_output_neurons; i++)
            {
                output_neurons[i] = new OutputNeuron(af);
                neurons[i + countNeurons - numb_output_neurons] = output_neurons[i];
            }
            for (int i = 0; i < countNeurons; i++)
            {
                Neuron cur = neurons[i];
                for (int j = 0; j < countNeurons; j++)
                {
                    topology[j, i] = false;
                    if (_connections[j, i] == true)
                    {
                        topology[j, i] = true;
                        weights[j, i] = _weights[j, i];
                    }
                }
            }
            setInputConnections();

            isIterationsFinished = true;
            isWaveCameToOutputNeuron = false;
        }
        public NeuroNet(NeuroNet net)
        {
            neurons = new Neuron[net.neurons.Length];
            input_neurons = new InputNeuron[net.input_neurons.Length];
            output_neurons = new OutputNeuron[net.output_neurons.Length];

            for (int i = 0; i < net.input_neurons.Length; i++)
            {
                input_neurons[i] = new InputNeuron(net.input_neurons[i]);
                neurons[i] = input_neurons[i];
            }
            for (int i = net.input_neurons.Length; 
                i < net.neurons.Length - net.output_neurons.Length; i++)
            {
                neurons[i] = new Neuron(net.neurons[i]);
            }
            for (int i = 0; i < net.output_neurons.Length; i++)
            {
                output_neurons[i] = new OutputNeuron(net.output_neurons[i]);
                neurons[i + net.neurons.Length - net.output_neurons.Length] = output_neurons[i];
            }
            evaluation_machine = new Queue<Queue<Neuron>>();

            neuronsInLayers = new int[net.neuronsInLayers.Length];
            Array.Copy(net.neuronsInLayers, neuronsInLayers, net.neuronsInLayers.Length);

            topology = new bool[net.neurons.Length, net.neurons.Length];
            Array.Copy(net.topology, topology, net.topology.Length);

            weights = new double[net.neurons.Length, net.neurons.Length];
            Array.Copy(net.weights, weights, net.weights.Length);

            setInputConnections();

            isIterationsFinished = true;
            isWaveCameToOutputNeuron = false;
        }

        public void ResetNeuroNet()
        {
            foreach (InputNeuron neu in input_neurons)
            {
                neu.InputValue = 0.0;
            }
            evaluation_machine.Clear();
            setInputConnections();
        }
        public Neuron GetNeuron(int index)
        {
            return neurons[index];
        }
        public int GetIndexNeuron(Neuron neuron)
        {
            int index = -1;
            for (int i = 0; i < NeuronsCount; i++)
            {
                if (neurons[i] == neuron)
                    return i;
            }
            return index;
        }
        public void SetNewConnection(int indexInput, int indexOutput, double weight)
        {
            topology[indexOutput, indexInput] = true;
            weights[indexOutput, indexInput] = weight;
            ResetNeuroNet();
        }
        public void DeleteConnection(int indexInput, int indexOutput)
        {
            topology[indexOutput, indexInput] = false;
            weights[indexOutput, indexInput] = 0.0;
            ResetNeuroNet();
        }

        public double[] MakeStep(double[] inputs)
        {
            isIterationsFinished = false;
            isWaveCameToOutputNeuron = false;
            double[] outputs = new double[output_neurons.Length];
            for (int i = 0; i < output_neurons.Length; i++)
            {
                outputs[i] = output_neurons[i].OutputValue;
            }
            for (int i = 0; i < input_neurons.Length; i++)
            {
                input_neurons[i].InputValue = inputs[i];
            }

            addInputNeuronsToFirstPool();

            while (true)
            {
                if (evaluation_machine.Count > 0)
                {
                    Queue<Neuron> pool = evaluation_machine.Dequeue();
                    Queue<Neuron> next_pool = new Queue<Neuron>();
                    bool isStop = false;

                    Queue<Neuron> poolCopy = new Queue<Neuron>(pool);
                    while (pool.Count > 0)
                    {
                        Neuron cur_evaluating_neuron = pool.Dequeue();
                        cur_evaluating_neuron.CalculateOutputValue();

                        int indexCurNeuron = -1;
                        for (int i = 0; i < NeuronsCount; i++)
                        {
                            if (cur_evaluating_neuron == neurons[i])
                            {
                                indexCurNeuron = i;
                                break;
                            }
                        }

                        for (int i = 0; i < NeuronsCount; i++)
                        {
                            if (topology[indexCurNeuron, i] == true)
                            {
                                Neuron neu = neurons[i];
                                if (next_pool.Contains(neu) == false)
                                {
                                    next_pool.Enqueue(neu);
                                }
                                if (input_neurons.Contains(neu) == true)
                                    isStop = true;
                            }
                        }

                        if (isWaveCameToOutputNeuron == false)
                            isWaveCameToOutputNeuron = output_neurons.Contains(cur_evaluating_neuron);

                        if (output_neurons.Contains(cur_evaluating_neuron) == true &&
                            (evaluation_machine.Count > 0 || next_pool.Count > 0))
                        {
                            isStop = true;
                            int index = -1;
                            for (int i = 0; i < output_neurons.Length; i++)
                            {
                                if (output_neurons[i] == cur_evaluating_neuron)
                                {
                                    index = i;
                                    break;
                                }
                            }
                            outputs[index] = cur_evaluating_neuron.OutputValue;
                        }
                    }
                    while (poolCopy.Count > 0)
                    {
                        Neuron curNeuron = poolCopy.Dequeue();

                        int indexCurNeuron = -1;
                        for (int i = 0; i < NeuronsCount; i++)
                        {
                            if (curNeuron == neurons[i])
                            {
                                indexCurNeuron = i;
                                break;
                            }
                        }

                        for (int i = 0; i < NeuronsCount; i++)
                        {
                            if (topology[indexCurNeuron, i] == true)
                            {
                                neurons[i].SetInputValue(neurons[indexCurNeuron].OutputValue, indexCurNeuron);
                            }
                        }
                    }
                    if (next_pool.Count > 0)
                    {
                        evaluation_machine.Enqueue(next_pool);
                    }

                    if (isStop == true)
                    {
                        if (pool.Count > 0)
                        {
                            evaluation_machine.Enqueue(pool);
                        }
                        return outputs;
                    }
                }
                else
                {
                    break;
                }
            }
            isIterationsFinished = true;
            for (int i = 0; i < output_neurons.Length; i++)
            {
                outputs[i] = output_neurons[i].OutputValue;
            }
            return outputs;
        }
        public double[] MakeIteration(double[] inputs)
        {
            double[] res = MakeStep(inputs);

            while (isWaveCameToOutputNeuron == false)
                res = MakeStep(inputs);

            return res;
        }
        public double[] MakeAnswer(double[] inputs, double eps = 1E-16)
        {
            ResetNeuroNet();
            double[] res = MakeIteration(inputs);
            if (isIterationsFinished != true)
            {
                double[] resNext = MakeIteration(inputs);
                double err = 0.0;
                for (int i = 0; i < res.GetLength(0); i++)
                {
                    err += Math.Pow((res[i] - resNext[i]), 2);
                }
                err = Math.Sqrt(err);
                err /= res.GetLength(0);

                while (isIterationsFinished != true && err > eps)
                {
                    res = MakeIteration(inputs);
                    if (isIterationsFinished != true)
                    {
                        resNext = MakeIteration(inputs);
                        err = 0.0;
                        for (int i = 0; i < res.GetLength(0); i++)
                        {
                            err += Math.Pow((res[i] - resNext[i]), 2);
                        }
                        err = Math.Sqrt(err);
                        err /= res.GetLength(0);
                    }
                    else
                        return res;
                }
                return resNext;
            }
            else
                return res;
        }

    }
}
