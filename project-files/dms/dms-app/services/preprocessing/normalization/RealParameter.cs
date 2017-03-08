using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.services.preprocessing.normalization
{
    public class RealParameter : IParameter
    {
        public string Type { get { return "Real"; } }
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

        public double MinRange { get; private set; }

        public RealParameter(List<string> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("values must contain at least one element");

            minValue = maxValue = Convert.ToSingle(values[0].Replace(".", ","));
            List<float> numbers = new List<float>();
            foreach (string item in values)
            {
                float val = Convert.ToSingle(item.Replace(".", ","));

                if (!numbers.Contains(val))
                    numbers.Add(val);

                minValue = Math.Min(minValue, val);
                maxValue = Math.Max(maxValue, val);
            }
            numbers.Sort();
            MinRange = double.PositiveInfinity;
            for (int i = 0; i < numbers.Count - 1; i ++)
            {
                MinRange = Math.Min(MinRange, Math.Abs(numbers[i] - numbers[i+1]));
            }

            countNumbers = -Convert.ToInt32(Math.Log10(MinRange)) + 1;
        }

        public float GetFloat(string value)
        {
            float temp = Convert.ToSingle(value.Replace(".", ","));

            if (temp < minValue || temp > maxValue)
                throw new ArgumentOutOfRangeException();

            return temp;
        }

        public float GetNormalizedFloat(string value)
        {
            double val = GetFloat(value);
            return (float)(val - minValue) / (maxValue - minValue);
        }

        public int GetNormalizedInt(string value)
        {
            double val = GetNormalizedFloat(value);
            return Convert.ToInt32(val * Math.Pow(2, countNumbers)) + Convert.ToInt32(minValue);
        }

        private float minValue, maxValue;
        private int countNumbers;
    }
}
