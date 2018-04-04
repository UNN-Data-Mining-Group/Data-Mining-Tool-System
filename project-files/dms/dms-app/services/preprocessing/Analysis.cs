using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.services.preprocessing
{
    class Analysis
    {
        public Analysis()
        {

        }

        public void executeCorrelationAnalysis(List<double> x, List<double> y)
        {
            Dictionary<string, double> person = executePearsonMethod(x, y);
        }

        // Pearson correlation coefficient
        private Dictionary<string, double> executePearsonMethod(List<double> x, List<double> y)
        {
            int n = x.Count;
            double M_x = x.Sum() / n;
            double M_y = y.Sum() / n;

            List<double> dx_2 = new List<double>();
            List<double> dy_2 = new List<double>();
            List<double> dx_mult_dy = new List<double>();
            for (int i = 0; i < n; i++)
            {
                double dx = x[i] - M_x;
                double dy = y[i] - M_y;

                dx_mult_dy.Add(dx * dy);

                dx_2.Add(Math.Pow(dx, 2));
                dy_2.Add(Math.Pow(dy, 2));
            }
            //correlation coeff
            double r_xy = dx_mult_dy.Sum() / Math.Sqrt(dx_2.Sum() * dy_2.Sum());
            //mistake
            double m_r_xy = Math.Sqrt((1 - Math.Pow(r_xy, 2)) / (n - 2));
            //reliability
            double t = r_xy / m_r_xy;

            return new Dictionary<string, double>{ {"r", r_xy}, {"m", m_r_xy}, {"t", t} };
        }

        //Rank correlation coefficients
        private Dictionary<string, double> executeSpearmanMethod(List<string> x, List<string> y)
        {
            int n = x.Count;
            List<double> X = new List<double>();
            List<double> Y = new List<double>();
            List<double> d_2 = new List<double>();

            //todo:: не реализовано ранжирование
            
            for(int i = 0; i < n; i++)
            {
                d_2.Add(Math.Pow(X[i] - Y[i], 2));
            }
            // correlation coeff
            double ro_xy = 1 - (6 * d_2.Sum() / (n * (Math.Pow(n, 2) - 1)));
            //mistake
            double m_ro_xy = Math.Sqrt((1 - Math.Pow(ro_xy, 2)) / (n - 2));
            //reliability
            double t = ro_xy / m_ro_xy;

            return new Dictionary<string, double> { { "r", ro_xy }, { "m", m_ro_xy }, { "t", t } };
        }

        private double getDispersion(List<List<double>> values)
        {
            List<double> avgs = new List<double>();
            List<double> dispersions = new List<double>();
            foreach(var x in values)
            {
                double avg = x.Average();
                avgs.Add(avg);
                // check!!!
                double d = x.Sum(item => Math.Pow((item - avg), 2)) / x.Count;
                dispersions.Add(d);
            }
            return 0;
        }
    }
}
