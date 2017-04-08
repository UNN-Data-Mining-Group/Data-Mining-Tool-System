using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace dms.services.preprocessing.normalization
{
    [Serializable]
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

            centerValue = (minValue + maxValue) / 2;
        }

        public int GetInt(string value)
        {
            int temp = Convert.ToInt32(value);

            if (temp < minValue || temp > maxValue)
                throw new ArgumentOutOfRangeException();

            return temp;
        }

        public float GetLinearNormalizedFloat(string value)
        {
            float val = GetInt(value);
            return (float)(val - minValue) / (maxValue - minValue);
        }

        public float GetNonlinearNormalizedFloat(string value)
        {
            float val = GetInt(value);
            return (float)(1 / (Math.Exp(-a * (val - centerValue)) + 1));
        }

        public int GetNormalizedInt(string value)
        {
            double val = GetLinearNormalizedFloat(value);
            return Convert.ToInt32(val * Math.Pow(10, countNumbers));
        }

        public string GetFromNormalized(int value)
        {
            return GetFromLinearNormalized((float)(value / Math.Pow(10, countNumbers)));
        }

        public string GetFromLinearNormalized(float value)
        {
            if (value < 0.0f)
                value = 0.0f;
            else if (value > 1.0f)
                value = 1.0f;

            float size = maxValue - minValue;
            return Convert.ToString(minValue + value * size);
        }

        public string GetFromNonlinearNormalized(float value)
        {
            if (value < 0.0f)
                value = 0.0f;
            else if (value > 1.0f)
                value = 1.0f;

            float output = (float)(centerValue - 1 / a * Math.Log(1 / value - 1));
            return Convert.ToString(output);
        }

        private float a = 1.0f; //Параметр aвлияет на степень нелинейности изменения переменной в нормализуемом интервале.
        private int minValue, maxValue, countValues, countNumbers, centerValue;
    }
}