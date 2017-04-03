using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using System.Globalization;

namespace desision_tree_lib.decision_tree
{
    class LearningTable
    {
        CultureInfo UsCulture = new CultureInfo("en-US");
        public List<ValueParameter[]> Rows;
        public int ParameterCount;

        public LearningTable()
        {
            Rows = new List<ValueParameter[]>();
            ParameterCount = 0;
        }

       /* public LearningTable(List<ValueParameter> arrValues, List<Parameter> arrParams)
        {
            Rows = new List<ValueParameter[]>();
            ParameterCount = arrParams.Count();
            if (arrValues != null)
            {
                int prevIndex = arrValues[0].RowIndex;
                int indexInRow = 0;
                ValueParameter[] row = new ValueParameter[arrParams.Count()];
                foreach (ValueParameter valueParam in arrValues)
                {
                    if (valueParam.RowIndex != prevIndex)
                    {
                        Rows.Add(row);
                        indexInRow = 0;
                        row = new ValueParameter[arrParams.Count()];
                    }
                    row[indexInRow] = valueParam;
                    indexInRow++;
                    prevIndex = valueParam.RowIndex;
                }
            }

        }*/

        public void BubbleSortByParam(int index_of_param, Parameter param)
        {
            if ((param.Type == TypeParameter.Real) || (param.Type == TypeParameter.Int))
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    for (int j = i + 1; j < Rows.Count; j++)
                    {
                        if (Convert.ToDouble(Rows.ElementAt(j)[index_of_param].Value, UsCulture) < Convert.ToDouble(Rows.ElementAt(i)[index_of_param].Value, UsCulture))
                        {
                            var temp = Rows[i];
                            Rows[i] = Rows[j];
                            Rows[j] = temp;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    for (int j = i + 1; j < Rows.Count; j++)
                    {
                        if (String.Compare(Rows.ElementAt(j)[index_of_param].Value, Rows.ElementAt(i)[index_of_param].Value) < 0)
                        {
                            var temp = Rows[i];
                            Rows[i] = Rows[j];
                            Rows[j] = temp;
                        }
                    }
                }
            }
        }

        public void QuickSortByParam(int first, int last, int index_of_param, Parameter param)
        {
            if ((param.Type == TypeParameter.Real) || (param.Type == TypeParameter.Int))
            {
                int i = first, j = last;
                var x = Convert.ToDouble(Rows[(first + last) / 2][index_of_param].Value, UsCulture);//  a[(first + last) / 2];
                do
                {
                    while (Convert.ToDouble(Rows[i][index_of_param].Value, UsCulture) < x) i++;
                    while (Convert.ToDouble(Rows[j][index_of_param].Value, UsCulture) > x) j--;

                    if (i <= j)
                    {
                        if (i < j)
                        {
                            var temp = Rows[i];
                            Rows[i] = Rows[j];
                            Rows[j] = temp;
                        }
                        i++;
                        j--;
                    }
                } while (i <= j);
                if (i < last)
                    QuickSortByParam(i, last, index_of_param, param);
                if (first < j)
                    QuickSortByParam(first, j, index_of_param, param);
            }
            else
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    for (int j = i + 1; j < Rows.Count; j++)
                    {
                        if (String.Compare(Rows.ElementAt(j)[index_of_param].Value, Rows.ElementAt(i)[index_of_param].Value) < 0)
                        {
                            var temp = Rows[i];
                            Rows[i] = Rows[j];
                            Rows[j] = temp;
                        }
                    }
                }

            }
        }

        public static void SplitLearningTable(LearningTable education_table, Rule split_rule, Parameter param, ref LearningTable left_table, ref LearningTable right_table)
        {
            if ((param.Type == TypeParameter.Real) || (param.Type == TypeParameter.Int))
            {
                for (int i = 0; i < education_table.Rows.Count; i++)
                {
                    double curVal = Convert.ToDouble(education_table.Rows.ElementAt(i)[split_rule.index_of_param].Value, UsCulture);
                    double splitVal = Convert.ToDouble(split_rule.value, UsCulture);
                    if (curVal <= splitVal)
                    {
                        right_table.Rows.Add(education_table.Rows.ElementAt(i));
                    }
                    else
                    {
                        left_table.Rows.Add(education_table.Rows.ElementAt(i));
                    }
                }
                right_table.ParameterCount = education_table.ParameterCount;
                left_table.ParameterCount = education_table.ParameterCount;
            }
            else
            {
                for (int i = 0; i < education_table.Rows.Count; i++)
                {
                    String curVal = education_table.Rows.ElementAt(i)[split_rule.index_of_param].Value;
                    String splitVal = split_rule.value;
                    if (curVal == splitVal)
                    {
                        right_table.Rows.Add(education_table.Rows.ElementAt(i));
                    }
                    else
                    {
                        left_table.Rows.Add(education_table.Rows.ElementAt(i));
                    }
                }
                right_table.ParameterCount = education_table.ParameterCount;
                left_table.ParameterCount = education_table.ParameterCount;
            }
        }

        public void FindBetterParameter(LearningTable education_table, ref int index_of_parametr, ref string best_value_for_split, ref Parametr _param)
        {
            index_of_parametr = 0;
            best_value_for_split = "";
            LearningClassInfo[] leftClassInf = ClassInfoInit();//ClassInfoInit2(education_table);
            LearningClassInfo[] rightClassInf = ClassInfoInit();//ClassInfoInit2(education_table);
            Parameter param;
            double giniValue = -100000;
            for (int index = 1; index < education_table.ParameterCount; index++)
            {
                param = sqlManager.GetOneParametrWithRequest("SELECT * FROM PARAM WHERE ID ='" + education_table.Rows.ElementAt(1)[index].ParameterID + "'");
                //education_table.BubbleSortByParam(index, param);
                education_table.QuickSortByParam(0, education_table.Rows.Count - 1, index, param);
                if ((param.Type == TypeParameter.Real) || (param.Type == TypeParameter.Int))
                {
                    double average = 0;
                    for (int prevRowInd = 0, nextRowInd = 1; nextRowInd < education_table.Rows.Count; prevRowInd++, nextRowInd++)
                    {
                        average = (Convert.ToDouble(education_table.Rows.ElementAt(prevRowInd)[index].Value, UsCulture) + Convert.ToDouble(education_table.Rows.ElementAt(nextRowInd)[index].Value, UsCulture)) / 2.0;
                        for (int i = 0; i < education_table.Rows.Count; i++)
                        {
                            if (Convert.ToDouble(education_table.Rows.ElementAt(i)[index].Value, UsCulture) <= average)
                            {
                                foreach (LearningClassInfo clinf in rightClassInf)
                                {
                                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                                    {
                                        clinf.number_of_checked++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (LearningClassInfo clinf in leftClassInf)
                                {
                                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                                    {
                                        clinf.number_of_checked++;
                                    }
                                }
                            }
                        }
                        double newGiniValue = GiniCalculator.GiniSplitCalc(leftClassInf, rightClassInf);
                        if (newGiniValue > giniValue)
                        {
                            giniValue = newGiniValue;
                            index_of_parametr = index;
                            best_value_for_split = average.ToString(UsCulture);
                            _param = param;
                        }
                        for (int i = 0; i < leftClassInf.Length; i++)
                        {
                            leftClassInf[i].number_of_checked = 0;
                            rightClassInf[i].number_of_checked = 0;
                        }
                    }

                }
                else
                {
                    //param.Range
                    int number_of_var = 0;
                    for (int i = 0; i < param.Range.Length; i++)
                    {
                        if (param.Range[i] == '|')
                            number_of_var++;
                    }

                    String[] variables = new String[number_of_var + 1];
                    for (int i = 0, j = 0; i < param.Range.Length; i++)
                    {
                        if (param.Range[i] == '|')
                        {
                            j++;
                            continue;
                        }
                        if (param.Range[i] != ' ')
                            variables[j] += param.Range[i];
                    }

                    for (int j = 0; j < number_of_var; j++)
                    {
                        for (int i = 0; i < education_table.Rows.Count; i++)
                        {
                            if (education_table.Rows.ElementAt(i)[index].Value == variables[j])
                            {
                                foreach (LearningClassInfo clinf in rightClassInf)
                                {
                                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                                    {
                                        clinf.number_of_checked++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (LearningClassInfo clinf in leftClassInf)
                                {
                                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                                    {
                                        clinf.number_of_checked++;
                                    }
                                }
                            }
                        }
                        double newGiniValue = GiniCalculator.GiniSplitCalc(leftClassInf, rightClassInf);
                        if (newGiniValue > giniValue)
                        {
                            giniValue = newGiniValue;
                            index_of_parametr = index;
                            best_value_for_split = variables[j];
                            _param = param;
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
}
