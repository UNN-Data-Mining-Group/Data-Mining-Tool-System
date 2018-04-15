using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace dms.services.preprocessing.normalization
{
    [Serializable]
    public class EnumeratedParameter : IParameter
    {
        public string Type { get { return "Enum"; } }
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

        public List<string> getClasses()
        {
            return classes;
        }

        public EnumeratedParameter(List<string> values)
        {
            classes = new List<string>();

            foreach (string item in values)
            {
                if (!classes.Contains(item))
                    classes.Add(item);
            }
            countClasses = classes.Count;
            countNumbers = Convert.ToInt32(Math.Log10(2 * countClasses)) + 1;

            centerValue = (countClasses - 1) / 2;//(minValue + maxValue) / 2;
        }

        public int GetInt(string value)
        {
            return classes.IndexOf(value);
        }

        public float GetLinearNormalizedFloat(string value)
        {
            int val = GetInt(value);
            float step = (float)1.0 / countClasses;
            float temp = 0.0f;
            for (int i = 0; i < countClasses; i++)
            {
                if (Math.Abs(val - temp) < 1e-10)
                {
                    return (float) (xLeft + (xRight - xLeft) * step / 2.0 + i * step * (xRight - xLeft));
                }
                temp++;
            }
            return float.NaN;
        }

        public float GetNonlinearNormalizedFloat(string value)
        {
            float val = GetInt(value);
            return (float)((xRight - xLeft) / (Math.Exp(-a * (val - centerValue)) + 1) + xLeft);
        }

        public int GetNormalizedInt(string value)
        {
            return GetInt(value) + 1;
        }

        public string GetFromNormalized(int value)
        {
            return classes[value - 1];
        }

        public string GetFromLinearNormalized(float value)
        {
            float step = (float)((xRight - xLeft) * 1.0 / (2 * countClasses));

            if (value <= xLeft)
                value = xLeft + step;
            else if (value >= xRight)
                value = xRight - step;
            value -= xLeft;
            value -= step;
            value = value * countClasses / (xRight - xLeft);
            return classes[Convert.ToInt32(value)];
        }

        public string GetFromNonlinearNormalized(float value)
        {
            if (value < xLeft)
                value = xLeft;
            else if (value > xRight)
                value = xRight;

            float output = (float)(centerValue - 1 / a * Math.Log((xRight - xLeft) / (value - xLeft) - 1));
            return classes[Convert.ToInt32(output)];
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

        private float centerValue;
        private float a = 1.0f; //Параметр aвлияет на степень нелинейности изменения переменной в нормализуемом интервале.
        private readonly int countClasses;
        private int countNumbers;
        private readonly List<string> classes;
        private float xLeft = 0, xRight = 1;
    }
}