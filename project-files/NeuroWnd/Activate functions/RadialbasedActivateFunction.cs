using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroWnd.Activate_functions
{
    class RadialbasedActivateFunction : ActivateFunction
    {
        public override bool HasContinuousDerivative
        {
            get { return true; }
        }
        public override string Name
        {
            get { return "Радиально-базисная функция"; }
        }

        public RadialbasedActivateFunction()
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

            return minVal + (maxVal + 1) * Math.Exp(-Math.Pow((x - w0) / speed, 2));
        }

        public override double Derivative(double x)
        {
            double w0 = parameters[0].Value;
            double maxVal = parameters[2].Value;
            double speed = parameters[3].Value;

            double exp = Math.Exp(-Math.Pow((x - w0) / speed, 2));
            return 2 * (maxVal + 1) * (w0 - x) * exp / (speed * speed);
        }
    }
}
