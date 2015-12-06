using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Activate_functions;

namespace NeuroWnd.Neuron_definition
{
    public class NeuronInputConnection
    {
        public double inputValue;
        public double weigth;
        public int indexOfInputNeuron;

        public NeuronInputConnection(double _weight, int _indexOfInputNeuron)
        {
            weigth = _weight;
            inputValue = 0.0;
            indexOfInputNeuron = _indexOfInputNeuron;
        }
        public NeuronInputConnection(double _weight, double _inputValue, int _indexOfInputNeuron)
        {
            weigth = _weight;
            inputValue = _inputValue;
            indexOfInputNeuron = _indexOfInputNeuron;
        }
        public NeuronInputConnection(NeuronInputConnection nic)
        {
            inputValue = nic.inputValue;
            weigth = nic.weigth;
            indexOfInputNeuron = nic.indexOfInputNeuron;
        }
    }

    public class Neuron
    {
        protected double outputValue;
        protected ActivateFunction act_func;
        protected NeuronInputConnection[] inputs;

        public double OutputValue { get { return outputValue; } }
        public int InputsCount
        {
            get
            {
                if (inputs != null)
                    return inputs.Length;
                else
                    return 0;
            }
        }
        public ActivateFunction ActivateFunctionOfNeuron { get { return act_func; } }

        protected Neuron()
        {
            act_func = null;
            inputs = null;
        }
        public Neuron(ActivateFunction af)
        {
            act_func = af;
            inputs = null;
        }
        public Neuron(ActivateFunction af, NeuronInputConnection[] _inputs)
        {
            act_func = af;
            SetInputConnections(_inputs);
        }
        public Neuron(Neuron neu)
        {
            outputValue = neu.outputValue;
            inputs = new NeuronInputConnection[neu.inputs.Length];
            for (int i = 0; i < neu.inputs.Length; i++)
            {
                inputs[i] = new NeuronInputConnection(neu.inputs[i]);
            }
            act_func = LibraryOfActivateFunctions.GetActivateFunction(neu.act_func.Name,
                LibraryOfActivateFunctions.GetterParameter.ActivateFunctionName);
            for (int i = 0; i < neu.act_func.CountParameters; i++)
            {
                act_func.SetValueOfParameter(i, neu.act_func.GetValueOfParameter(i));
            }
        }
        public void SetInputConnections(NeuronInputConnection[] _inputs)
        {
            inputs = new NeuronInputConnection[_inputs.Length];
            for (int i = 0; i < _inputs.Length; i++)
            {
                inputs[i] = new NeuronInputConnection(_inputs[i].weigth, _inputs[i].inputValue, _inputs[i].indexOfInputNeuron);
            }
        }
        public void SetInputValue(double inputValue, int indexOfInputConnection)
        {
            for (int i = 0; i < InputsCount; i++)
            {
                if (inputs[i].indexOfInputNeuron == indexOfInputConnection)
                {
                    inputs[i].inputValue = inputValue;
                    break;
                }
            }
        }
        public void SetWeightValue(double weightValue, int indexOfInputConnection)
        {
            for (int i = 0; i < InputsCount; i++)
            {
                if (inputs[i].indexOfInputNeuron == indexOfInputConnection)
                {
                    inputs[i].weigth = weightValue;
                    break;
                }
            }
        }
        public virtual void CalculateOutputValue()
        {
            double sum = 0;
            foreach (NeuronInputConnection item in inputs)
            {
                sum += item.weigth * item.inputValue;
                item.inputValue = 0.0;
            }
            outputValue = act_func.Function(sum);
        }
    }
}
