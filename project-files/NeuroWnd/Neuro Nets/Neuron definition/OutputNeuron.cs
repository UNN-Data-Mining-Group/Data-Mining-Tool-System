using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Activate_functions;

namespace NeuroWnd.Neuron_definition
{
    public class OutputNeuron : Neuron
    {
        public OutputNeuron(ActivateFunction af)
        {
            act_func = af;
        }
        public OutputNeuron(ActivateFunction af, NeuronInputConnection[] _inputs)
        {
            act_func = af;
            SetInputConnections(_inputs);
        }

        public OutputNeuron(OutputNeuron neu)
            : base(neu)
        {
        }

        public override void CalculateOutputValue()
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
