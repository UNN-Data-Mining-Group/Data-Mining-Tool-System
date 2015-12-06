using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroWnd.Activate_functions
{
    class LinearActivateFunction: ActivateFunction
    {
        public override bool HasContinuousDerivative
        {
            get { return false; }
        }
        public override string Name
        {
            get { return "Линейная функция"; }
        }

        public LinearActivateFunction()
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
            double k = parameters[3].Value;

            if (x <= w0)
            {
                return minVal;
            }
            else
            {
                double val = minVal + k * (x - w0);
                if (val > maxVal)
                    return maxVal;
                else
                    return val;
            }
        }
        public override double Derivative(double x)
        {
            double w0 = parameters[0].Value;
            double minVal = parameters[1].Value;
            double maxVal = parameters[2].Value;
            double k = parameters[3].Value;

            if (x <= w0 || minVal + k * (x - w0) >= maxVal)
            {
                return 0.0;
            }
            else
            {
                return k;
            }
        }
    }
}
