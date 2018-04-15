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
            return (float)((val - minValue) / (maxValue - minValue) * (xRight - xLeft) + xLeft);
        }

        public float GetNonlinearNormalizedFloat(string value)
        {
            float val = GetInt(value);
            return (float)((xRight - xLeft) / (Math.Exp(-a * (val - centerValue)) + 1) + xLeft);
        }

        public int GetNormalizedInt(string value)
        {
            setRange(0, 1);
            double val = GetLinearNormalizedFloat(value);
            return Convert.ToInt32(val * Math.Pow(10, countNumbers));
        }

        public string GetFromNormalized(int value)
        {
            setRange(0, 1);
            return GetFromLinearNormalized((float)(value / Math.Pow(10, countNumbers)));
        }

        public string GetFromLinearNormalized(float value)
        {
            if (value < xLeft)
                value = xLeft;
            else if (value > xRight)
                value = xRight;

            float size = maxValue - minValue;
            float res = (value - xLeft) / (xRight - xLeft) * size + minValue;
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
        private int minValue, maxValue, countValues, countNumbers, centerValue;
        private float xLeft = 0, xRight = 1;
    }
}