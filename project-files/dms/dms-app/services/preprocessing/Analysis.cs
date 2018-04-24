using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.services.preprocessing
{
    public class Pair
    {
        public int oldPosition { get; set; }
        public string oldValue { get; set; }
        public double newValue { get; set; }
    }
    class Analysis
    {
        public enum TypeAnalysis
        {
            Empty,
            PearsonMethod,
            SpearmanMethod
        }
        public Analysis()
        {

        }
        public int rowSize { get; set; }
        public string[][] executeCorrelationAnalysis(TypeAnalysis type, int selectionId, int templateId)
        {
            
            models.Parameter[] parameters = models.Parameter.parametersOfTaskTemplateId(templateId).ToArray();
            Dictionary<string, Dictionary<string, double>> resultMap = getCorrelationAnalysisMap(type, selectionId, parameters);

            Dictionary <string, Dictionary<string, double>> mapStrong = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> mapMedium = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> mapSmall = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> mapVerySmall = new Dictionary<string, Dictionary<string, double>>();
            //0,99$\div $0,7, то это сильная статистическая взаимосвязь; 0,5$\div $0,69 - средняя; 0,2$\div $0,49 - слабая; 0,09$\div $0,19 - очень слабая.
            foreach (KeyValuePair<string, Dictionary<string, double>> entry in resultMap)
            {
                Dictionary<string, double> val = entry.Value;
                double r = val.Values.First();
                if (Math.Abs(r) >= 0.7)
                {
                    mapStrong.Add(entry.Key, entry.Value);
                }
                else if (Math.Abs(r) >= 0.5)
                {
                    mapMedium.Add(entry.Key, entry.Value);
                }
                else if (Math.Abs(r) >= 0.2)
                {
                    mapSmall.Add(entry.Key, entry.Value);
                }
                else
                {
                    mapVerySmall.Add(entry.Key, entry.Value);
                }
            }
            int maxSize = Math.Max(mapStrong.Count, mapMedium.Count);
            rowSize = maxSize;
            maxSize = Math.Max(maxSize, mapSmall.Count);
            maxSize = Math.Max(maxSize, mapVerySmall.Count);

            string[][] output = new string[maxSize][];
            for(int i = 0; i < maxSize; i++)
            {
                output[i] = new string[4];
                if(mapStrong.Count > i)
                {
                    string line = "";
                    foreach(KeyValuePair<string,double> entry in mapStrong.Values.ElementAt(i))
                    {
                        if ("r".Equals(entry.Key))
                        {
                            line = entry.Key + "=" + entry.Value;
                            break;
                        }
                    }
                    output[i][0] = mapStrong.Keys.ElementAt(i) + ": {" + line +"}";
                }
                if (mapMedium.Count > i)
                {
                    string line = "";
                    foreach (KeyValuePair<string, double> entry in mapMedium.Values.ElementAt(i))
                    {
                        if ("r".Equals(entry.Key))
                        {
                            line = entry.Key + "=" + entry.Value;
                            break;
                        }
                    }
                    output[i][1] = mapMedium.Keys.ElementAt(i) + ": {" + line + "}";
                }
                if (mapSmall.Count > i)
                {
                    string line = "";
                    foreach (KeyValuePair<string, double> entry in mapSmall.Values.ElementAt(i))
                    {
                        if ("r".Equals(entry.Key))
                        {
                            line = entry.Key + "=" + entry.Value;
                            break;
                        }
                    }
                    output[i][2] = mapSmall.Keys.ElementAt(i) + ": {" + line + "}";
                }
                if (mapVerySmall.Count > i)
                {
                    string line = "";
                    foreach (KeyValuePair<string, double> entry in mapVerySmall.Values.ElementAt(i))
                    {
                        if ("r".Equals(entry.Key))
                        {
                            line = entry.Key + "=" + entry.Value;
                            break;
                        }
                    }
                    output[i][3] = mapVerySmall.Keys.ElementAt(i) + ": {" + line + "}";
                }
            }
            return output;
        }
        public Dictionary<string, Dictionary<string, double>> getCorrelationAnalysisMap(TypeAnalysis type, int selectionId, models.Parameter[] parameters)
        {
            Dictionary<string, Dictionary<string, double>> map = new Dictionary<string, Dictionary<string, double>>();
            if(TypeAnalysis.PearsonMethod.Equals(type))
            {
                for(int i = 0; i < parameters.Length; i++)
                {
                    List<string> l = models.Selection.valuesByParmeterId(selectionId, parameters[i].ID);
                    List<double> xR = l.Select(x => double.Parse(x.Replace(".", ","))).ToList();
                    for (int j = i+1; j < parameters.Length; j++)
                    {
                        List<string> l1 = models.Selection.valuesByParmeterId(selectionId, parameters[j].ID);
                        List<double> yR = l1.Select(x => double.Parse(x.Replace(".", ","))).ToList();
                        map.Add("i = " + parameters[i].ID + "; j = " + parameters[j].ID, executePearsonMethod(xR, yR));

                    }
                }
            }
            else if (TypeAnalysis.SpearmanMethod.Equals(type))
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    List<Pair> x1 = models.Selection.pairValuesByParmeterId(selectionId, parameters[i].ID);

                    for (int j = i + 1; j < parameters.Length; j++)
                    {
                        List<Pair> x2 = models.Selection.pairValuesByParmeterId(selectionId, parameters[j].ID);
                        map.Add("i = " + parameters[i].ID + "; j = " + parameters[j].ID, executeSpearmanMethod(x1, parameters[i].Type, parameters[i].Comment, x2, parameters[j].Type, parameters[i].Comment));
                    }
                }
            }
            return map;
        }

        private double getDispersion(List<List<double>> values)
        {
            List<double> avgs = new List<double>();
            List<double> dispersions = new List<double>();
            foreach (var x in values)
            {
                double avg = x.Average();
                avgs.Add(avg);
                // check!!!
                double d = x.Sum(item => Math.Pow((item - avg), 2)) / x.Count;
                dispersions.Add(d);
            }
            return 0;
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
            if (r_xy != 1)
            {
                string res = "";
            }
            //mistake
            double m_r_xy = Math.Sqrt((1 - Math.Pow(r_xy, 2)) / (n - 2));
            //reliability
            double t = r_xy / m_r_xy;

            return new Dictionary<string, double>{ {"r", r_xy}, {"m", m_r_xy}, {"t", t} };
        }

        //Rank correlation coefficients
        private Dictionary<string, double> executeSpearmanMethod(List<Pair> x1, models.TypeParameter parameterTypeX1, string sortOrderX1, 
                                                                 List<Pair> x2, models.TypeParameter parameterTypeX2, string sortOrderX2)
        {
            int n = x1.Count;
            List<double> d_2 = new List<double>();

            //ранжирование
            List<double> y1 = new List<double>();
            List<double> y2 = new List<double>();
            getNewVlues(x1, parameterTypeX1, sortOrderX1, x2, parameterTypeX2, sortOrderX2, y1, y2);

            for (int i = 0; i < n; i++)
            {
                d_2.Add(Math.Pow(y1[i] - y2[i], 2));
            }
            // correlation coeff
            double ro_xy = 1 - (6 * d_2.Sum() / (n * (Math.Pow(n, 2) - 1)));
            //mistake
            double m_ro_xy = Math.Sqrt((1 - Math.Pow(ro_xy, 2)) / (n - 2));
            //reliability
            double t = ro_xy / m_ro_xy;

            return new Dictionary<string, double> { { "r", ro_xy }, { "m", m_ro_xy }, { "t", t } };
        }

        private void getNewVlues(List<Pair> x1, models.TypeParameter parameterTypeX1, string sortOrderX1, List<Pair> x2, models.TypeParameter parameterTypeX2, string sortOrderX2, 
            List<double> y1, List<double> y2)
        {
            if (models.TypeParameter.Enum.Equals(parameterTypeX1))
            {
                //берем sortOrderX1 в качетве порядка
                x1.Sort(new CompSortByOldStringValue<Pair>(sortOrderX1));
            }
            else
            {
                x1.Sort(new CompSortByOldDoubleValue<Pair>());
            }
            setNewValues(x1);

            if (models.TypeParameter.Enum.Equals(parameterTypeX2))
            {
                //берем sortOrderX2 в качетве порядка
                x2.Sort(new CompSortByOldStringValue<Pair>(sortOrderX2));
            }
            else
            {
                x2.Sort(new CompSortByOldDoubleValue<Pair>());
            }
            setNewValues(x2);
            x2.Sort(new CompSortByOldPosition<Pair>());

            getRelevantX1X2(x1, x2, y1, y2);
        }
        
        private void setNewValues(List<Pair> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                double sum = i + 1;
                int index = i;
                Pair value = values.ElementAt(index);
                while(index + 1 < values.Count && value.oldValue.Equals(values.ElementAt(index + 1).oldValue))
                {
                    index++;
                    sum += (index + 1);
                }
                double newValue = (double) sum / (index - i + 1);
                for(int j = i; j <= index; j++)
                {
                    values.ElementAt(j).newValue = newValue;
                }
                i = index; 
            }
        }

        private void getRelevantX1X2(List<Pair> x1, List<Pair> x2, List<double> y1, List<double> y2)
        {
            foreach (Pair item in x1)
            {
                y1.Add(item.newValue);
                y2.Add(x2.ElementAt(item.oldPosition - 1).newValue);
            }
        }
        
        class CompSortByOldStringValue<T> : IComparer<T>
        where T : Pair
        {
            string order;
            public CompSortByOldStringValue(string order)
            {
                this.order = order;//.Split(',').Select(x => double.Parse(x.Replace(".", ","))).ToArray();
            }
            public int Compare(T x, T y)
            {
                if(order == null)
                {
                    return String.Compare(x.oldValue, y.oldValue);
                }
                else
                {
                    int pos1 = order.IndexOf(x.oldValue);
                    int pos2 = order.IndexOf(y.oldValue);
                    if (pos1 < pos2) return -1;
                    else if (pos1 > pos2) return 1;
                    else return 0;
                }                
            }
        }
        class CompSortByOldDoubleValue<T> : IComparer<T>
        where T : Pair
        {
            public int Compare(T x, T y)
            {
                if (Double.Parse(x.oldValue.Replace(".", ",")) < Double.Parse(y.oldValue.Replace(".", ","))) return -1;
                else if (Double.Parse(x.oldValue.Replace(".", ",")) > Double.Parse(y.oldValue.Replace(".", ","))) return 1;
                else return 0;
            }
        }
        class CompSortByOldPosition<T> : IComparer<T>
        where T : Pair
        {
            public int Compare(T x, T y)
            {
                if (x.oldPosition < y.oldPosition) return -1;
                else if (x.oldPosition > y.oldPosition) return 1;
                else return 0;
            }
        }

        public void test()
        {
            Pair p1 = new Pair();
            p1.oldPosition = 1;
            p1.oldValue = "До 1 года";
            Pair p2 = new Pair();
            p2.oldPosition = 2;
            p2.oldValue = "1-2";
            Pair p3 = new Pair();
            p3.oldPosition = 3;
            p3.oldValue = "3-4";
            Pair p4 = new Pair();
            p4.oldPosition = 4;
            p4.oldValue = "5-6";
            Pair p5 = new Pair();
            p5.oldPosition = 5;
            p5.oldValue = "7 и более";
            List<Pair> x1 = new List<Pair> { p1, p2, p3, p4, p5 };
            Pair t1 = new Pair();
            t1.oldPosition = 1;
            t1.oldValue = "24";
            Pair t2 = new Pair();
            t2.oldPosition = 2;
            t2.oldValue = "16";
            Pair t3 = new Pair();
            t3.oldPosition = 3;
            t3.oldValue = "12";
            Pair t4 = new Pair();
            t4.oldPosition = 4;
            t4.oldValue = "12";
            Pair t5 = new Pair();
            t5.oldPosition = 5;
            t5.oldValue = "6";
            List<Pair> x2 = new List<Pair> { t1, t2, t3, t4, t5 };

            executeSpearmanMethod(x1, models.TypeParameter.Enum, "до 1 года,1 - 2,3 - 4,5 - 6,7 и более", x2, models.TypeParameter.Int, null);
        }
    }
}
