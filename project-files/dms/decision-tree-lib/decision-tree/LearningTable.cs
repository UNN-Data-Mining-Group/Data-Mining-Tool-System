using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using dms.solvers.decision.tree.algo;

namespace dms.solvers.decision.tree
{
    public class LearningTable
    {
        public float[][] LearningData;
        public float[] LearningClasses;

        public LearningTable()
        {
            LearningClasses = null;
            LearningData = null;
        }

        public LearningTable(float[][] x, float[] y)
        {
            LearningData = x;
            LearningClasses = y;
        }

        public LearningClassInfo[] ClassInfoInit(LearningTable educationTable)
        {
            List<float> paramValues = new List<float>();
            paramValues.Add(educationTable.LearningClasses[0]);
            for (int i = 0; i < educationTable.LearningClasses.Length; i++)
            {
                if (!paramValues.Contains(educationTable.LearningClasses[i]))
                {
                    paramValues.Add(educationTable.LearningClasses[i]);
                }
            }

            LearningClassInfo[] newClassInfo = new LearningClassInfo[paramValues.Count];
            for (int i = 0; i < paramValues.Count; i++)
            {
                newClassInfo[i] = new LearningClassInfo();
                newClassInfo[i].class_name = paramValues[i];
            }

            for (int i = 0; i < educationTable.LearningData.Length; i++)
            {
                foreach (LearningClassInfo clinf in newClassInfo)
                {
                    if (clinf.class_name == educationTable.LearningClasses[i])
                    {
                        clinf.number_of_checked++;
                    }
                }
            }
            return newClassInfo;
        }

        public LearningClassInfo[] ClassInfoInit(LearningTable educationTable, int start, int finish)
        {
            List<float> paramValues = new List<float>();
            paramValues.Add(educationTable.LearningClasses[0]);
            for (int i = 0; i < educationTable.LearningClasses.Length; i++)
            {
                if (!paramValues.Contains(educationTable.LearningClasses[i]))
                {
                    paramValues.Add(educationTable.LearningClasses[i]);
                }
            }

            LearningClassInfo[] newClassInfo = new LearningClassInfo[paramValues.Count];
            for (int i = 0; i < paramValues.Count; i++)
            {
                newClassInfo[i] = new LearningClassInfo();
                newClassInfo[i].class_name = paramValues[i];
            }
            for (int i = start; i < finish; i++)
            {
                foreach (LearningClassInfo clinf in newClassInfo)
                {
                    if (clinf.class_name == educationTable.LearningClasses[i])
                    {
                        clinf.number_of_checked++;
                    }
                }
            }
            return newClassInfo;
        }


        public void BubbleSortByParam(int index_of_param)
        {
            for (int i = 0; i < LearningData.Length; i++)
            {
                for (int j = i + 1; j < LearningData.Length; j++)
                {
                    if (LearningData[j][index_of_param] < LearningData[i][index_of_param])
                    {
                        var temp = LearningData[i];
                        LearningData[i] = LearningData[j];
                        LearningData[j] = temp;
                        var temp2 = LearningClasses[i];
                        LearningClasses[i] = LearningClasses[j];
                        LearningClasses[j] = temp2;
                    }
                }
            }
        }

        public void QuickSortByParam(int first, int last, int index_of_param)
        {
            int i = first, j = last;
            var x = LearningData[(first + last) / 2][index_of_param];
            do
            {
                while (LearningData[i][index_of_param] < x) i++;
                while (LearningData[j][index_of_param] > x) j--;

                if (i <= j)
                {
                    if (i < j)
                    {
                        var temp = LearningData[i];
                        LearningData[i] = LearningData[j];
                        LearningData[j] = temp;
                        var temp2 = LearningClasses[i];
                        LearningClasses[i] = LearningClasses[j];
                        LearningClasses[j] = temp2;
                    }
                    i++;
                    j--;
                }
            } while (i <= j);
            if (i < last)
                QuickSortByParam(i, last, index_of_param);
            if (first < j)
                QuickSortByParam(first, j, index_of_param);

        }

        public void SplitLearningTable(LearningTable education_table, Rule split_rule, ref LearningTable left_table, ref LearningTable right_table)
        {
            List<float[]> temp_right_x = new List<float[]>();
            List<float[]> temp_left_x = new List<float[]>();
            List<float> temp_right_y = new List<float>();
            List<float> temp_left_y = new List<float>();

            for (int i = 0; i < education_table.LearningData.Length; i++)
            {
                float curVal = education_table.LearningData[i][split_rule.index_of_param];
                float splitVal = split_rule.value;
                if (curVal <= splitVal)
                {
                    temp_right_x.Add(education_table.LearningData[i]);
                    temp_right_y.Add(education_table.LearningClasses[i]);
                }
                else
                {
                    temp_left_x.Add(education_table.LearningData[i]);
                    temp_left_y.Add(education_table.LearningClasses[i]);
                }
            }
            if (temp_left_x.Count == 0)
            {
                float tmpy = temp_right_y[0];
                for (int i = 1; i < temp_right_y.Count; i++)
                {
                    if (tmpy != temp_right_y[i])
                    {
                        temp_left_x.Add(temp_right_x[i]);
                        temp_right_x.RemoveAt(i);
                        temp_left_y.Add(temp_right_y[i]);
                        temp_right_y.RemoveAt(i);
                    }
                }
            }
            else if (temp_right_x.Count == 0)
            {
                float tmpy = temp_left_y[0];
                for (int i = 1; i < temp_left_y.Count; i++)
                {
                    if (tmpy != temp_left_y[i])
                    {
                        temp_right_x.Add(temp_left_x[i]);
                        temp_left_x.RemoveAt(i);
                        temp_right_y.Add(temp_left_y[i]);
                        temp_left_y.RemoveAt(i);
                    }
                }
            }
           
            left_table.LearningData = temp_left_x.ToArray();
            right_table.LearningData = temp_right_x.ToArray();
            left_table.LearningClasses = temp_left_y.ToArray();
            right_table.LearningClasses = temp_right_y.ToArray();
        }

        public void FindBetterParameter(LearningTable education_table, ref int index_of_parametr, ref string best_value_for_split, int inputs ,int outputs)
        {
            index_of_parametr = 0;
            best_value_for_split = "";
            LearningClassInfo[] leftClassInf;
            LearningClassInfo[] rightClassInf;
            double giniValue = -100000;
            for (int index = 0; index < inputs; index++)
            {
                education_table.QuickSortByParam(0, education_table.LearningData.Length - 1, index);
                double average = 0;
                for (int prevRowInd = 0, nextRowInd = 1; nextRowInd < education_table.LearningData.Length; prevRowInd++, nextRowInd++)
                {
                    average = (education_table.LearningData[prevRowInd][index] + education_table.LearningData[nextRowInd][index]) / 2.0;
                    leftClassInf = ClassInfoInit(education_table, 0, nextRowInd);
                    rightClassInf = ClassInfoInit(education_table, nextRowInd, education_table.LearningClasses.Length);
                    double newGiniValue = GiniCalculator.GiniSplitCalc(leftClassInf, rightClassInf);
                    if (newGiniValue > giniValue)
                    {
                        giniValue = newGiniValue;
                        index_of_parametr = index;
                        best_value_for_split = average.ToString();
                    }
                    for (int i = 0; i < leftClassInf.Length; i++)
                    {
                        leftClassInf[i].number_of_checked = 0;
                        rightClassInf[i].number_of_checked = 0;
                    }
                }
            }
        }
    }
}
