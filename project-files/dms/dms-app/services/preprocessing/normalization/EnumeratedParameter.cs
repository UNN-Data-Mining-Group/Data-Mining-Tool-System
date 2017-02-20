using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.services.preprocessing.normalization
{
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
        }

        public int GetInt(string value)
        {
            return classes.IndexOf(value);
        }

        public float GetNormalizedfloat(string value)
        {
            int val = GetInt(value);
            float step = 1.0 / countClasses;
            float temp = 0.0;
            for (int i = 0; i < countClasses; i++)
            {
                if (Math.Abs(val - temp) < 1e-10)
                {
                    return step / 2.0 + i * step;
                }
                temp++;
            }
            return float.NaN;
        }

        public int GetNormalizedInt(string value)
        {
            float val = GetNormalizedfloat(value);
            return Convert.ToInt32(val * Math.Pow(10, countNumbers));
        }

        private readonly int countClasses;
        private int countNumbers;
        private readonly List<string> classes;
    }
}
