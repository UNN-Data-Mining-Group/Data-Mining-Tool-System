using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroWnd.Activate_functions
{
    class TangentialActivateFunction : ActivateFunction
    {
        public override bool HasContinuousDerivative
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "Тангенциальная функция"; }
        }

        public TangentialActivateFunction()
        {
            parameters = new List<ActivateFunctionParameter>();
            parameters.Add(new ActivateFunctionParameter("w0", 0.0));
            parameters.Add(new ActivateFunctionParameter("minVal", 0.0));
            parameters.Add(new ActivateFunctionParameter("maxVal", 1.0));
            parameters.Add(new ActivateFunctionParameter("speed", 1.0));
        }

        public override double Function(double x)
        {
            double w0 = parameters[0].Value;
            double minVal = parameters[1].Value;
            double maxVal = parameters[2].Value;
            double speed = parameters[3].Value;

            return (minVal + maxVal) / 2 + ((maxVal - minVal) / 2) * Math.Tanh((x - w0) / speed);
        }

        public override double Derivative(double x)
        {
            double w0 = parameters[0].Value;
            double minVal = parameters[1].Value;
            double maxVal = parameters[2].Value;
            double speed = parameters[3].Value;

            return (maxVal - minVal) / 4 * Math.Pow(1 / Math.Cosh((x - w0) / 2), 2);
        }
    }
}
