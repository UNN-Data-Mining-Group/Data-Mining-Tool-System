using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Activate_functions;

namespace NeuroWnd.Neuron_definition
{
    public class InputNeuron : Neuron
    {
        public double InputValue;

        public InputNeuron(ActivateFunction af)
        {
            act_func = af;
            InputValue = 0.0;
        }
        public InputNeuron(ActivateFunction af, double inputValue)
        {
            act_func = af;
            InputValue = inputValue;
        }
        public InputNeuron(ActivateFunction af, double inputValue, NeuronInputConnection[] _inputs)
        {
            act_func = af;
            InputValue = inputValue;
            SetInputConnections(_inputs);
        }
        public InputNeuron(InputNeuron neu) : base(neu)
        {
            InputValue = neu.InputValue;
        }

        public override void CalculateOutputValue()
        {
            double sum = 0;
            foreach (NeuronInputConnection item in inputs)
            {
                sum += item.weigth * item.inputValue;
                item.inputValue = 0.0;
            }
            sum += InputValue;
            outputValue = act_func.Function(sum);
        }
    }
}
