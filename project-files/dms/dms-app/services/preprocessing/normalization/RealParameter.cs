using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace dms.services.preprocessing.normalization
{
    [Serializable]
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
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                MinRange = Math.Min(MinRange, Math.Abs(numbers[i] - numbers[i + 1]));
            }

            countNumbers = -Convert.ToInt32(Math.Log10(MinRange)) + 1;

            centerValue = (minValue + maxValue) / 2;
        }

        public float GetFloat(string value)
        {
            float temp = Convert.ToSingle(value.Replace(".", ","));

            if (temp < minValue || temp > maxValue)
                throw new ArgumentOutOfRangeException();

            return temp;
        }

        public float GetLinearNormalizedFloat(string value)
        {
            float val = GetFloat(value);
            return (float)((val - minValue) * (xRight - xLeft) / (maxValue - minValue) + xLeft);
        }

        public float GetNonlinearNormalizedFloat(string value)
        {
            float val = GetFloat(value);
            float res = (float)((xRight - xLeft) / (Math.Exp(-a * (val - centerValue)) + 1) + xLeft);
            return res;
        }

        public int GetNormalizedInt(string value)
        {
            setRange(0, 1);
            float val = GetLinearNormalizedFloat(value);
            return Convert.ToInt32(val * Math.Pow(10, countNumbers));
        }

        public string GetFromNormalized(int value)
        {
            setRange(0, 1);
            float dval = (float)(value / Math.Pow(10, countNumbers));
            return GetFromLinearNormalized(dval);
        }

        public string GetFromLinearNormalized(float value)
        {
            if (value < xLeft)
                value = xLeft;
            else if (value > xRight)
                value = xRight;

            float size = maxValue - minValue;
            float res = (value - xLeft) * size / (xRight - xLeft) + minValue;
            return Convert.ToString(res);
        }

        public string GetFromNonlinearNormalized(float value)
        {
            if (value < xLeft)
                value = xLeft;
            else if (value > xRight)
                value = xRight;

            float output = (float)(centerValue - 1 / a * Math.Log((xRight - xLeft) / (value - xLeft) - 1));
            return Convert.ToString(output);
        }

        public void setRange(float left, float right)
        {
            xLeft = left;
            xRight = right;
        }

        public void setParam(float param)
        {
            a = param;
        }

        private float a = 1.0f; //Параметр aвлияет на степень нелинейности изменения переменной в нормализуемом интервале.
        private float minValue, maxValue, centerValue;
        private float xLeft = 0, xRight = 1;
        private int countNumbers;
    }
}