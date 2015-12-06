using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroWnd.Activate_functions
{
    class HeavisideActivateFunction : ActivateFunction
    {
        public override bool HasContinuousDerivative
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "Функция Хевисайда"; }
        }

        public HeavisideActivateFunction()
        {
            parameters = new List<ActivateFunctionParameter>();
            parameters.Add(new ActivateFunctionParameter("w0", 0.0));
            parameters.Add(new ActivateFunctionParameter("minVal", 0.0));
            parameters.Add(new ActivateFunctionParameter("maxVal", 1.0));
        }

        public override double Function(double x)
        {
            double w0 = parameters[0].Value;
            double minVal = parameters[1].Value;
            double maxVal = parameters[2].Value;

            if (x > w0)
                return maxVal;
            else
                return minVal;
        }

        public override double Derivative(double x)
        {
            double w0 = parameters[0].Value;
            double minVal = parameters[1].Value;
            double maxVal = parameters[2].Value;

            if (x != w0)
                return 0.0;
            else
                return Double.PositiveInfinity;
        }
    }
}
