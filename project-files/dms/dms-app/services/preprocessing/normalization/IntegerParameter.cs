using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.services.preprocessing.normalization
{
    public class IntegerParameter : IParameter
    {
        public string Type { get { return "Integer"; } }

        public int CountNumbers
        {
            get { return countNumbers; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                countNumbers = value;
            }
        }

        public IntegerParameter(List<string> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("values must contain at least one element");

            minValue = maxValue = Convert.ToInt32(values[0]);

            foreach (string item in values)
            {
                int val = Convert.ToInt32(item);

                minValue = Math.Min(minValue, val);
                maxValue = Math.Max(maxValue, val);
            }

            countValues = (maxValue - minValue) + 1;
            countNumbers = Convert.ToInt32(Math.Log10(2 * countValues)) + 1;
        }

        public int GetInt(string value)
        {
            int temp = Convert.ToInt32(value);

            if (temp < minValue)
            {
                string t = "fgb";
            }
            if (temp > maxValue)
            {
                string ty = "fdf";
            }
            if (temp < minValue || temp > maxValue)
                throw new ArgumentOutOfRangeException();

            return temp;
        }

        public double GetNormalizedDouble(string value)
        {
            int val = GetInt(value);
            double step = 1.0 / countValues;
            double temp = minValue;
            for (int i = 0; i < countValues; i++)
            {
                if (Math.Abs(val - temp) < 1e-10)
                {
                    return step / 2.0 + i * step;
                }
                temp++;
            }
            return Double.NaN;
        }

        public int GetNormalizedInt(string value)
        {
            double val = GetNormalizedDouble(value);
            return Convert.ToInt32(val * Math.Pow(10, countNumbers));
        }

        private int minValue, maxValue, countValues, countNumbers;
    }
}
