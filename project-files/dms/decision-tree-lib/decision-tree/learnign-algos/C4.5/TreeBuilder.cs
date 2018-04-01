using dms.solvers.decision.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree.algo
{
    class TreeBuilder
    {
        public static void FindBetterParameter(LearningTable education_table, ref int index_of_parametr, ref string best_value_for_split, int inputs, int outputs)
        {
            index_of_parametr = 0;
            best_value_for_split = "";
            LearningClassInfo[] leftClassInf;
            LearningClassInfo[] rightClassInf;
            double entrValue = -100000;
            for (int index = 0; index < inputs; index++)
            {
                education_table.QuickSortByParam(0, education_table.LearningData.Length - 1, index);
                double average = 0;
                for (int prevRowInd = 0, nextRowInd = 1; nextRowInd < education_table.LearningData.Length; prevRowInd++, nextRowInd++)
                {
                    average = (education_table.LearningData[prevRowInd][index] + education_table.LearningData[nextRowInd][index]) / 2.0;
                    leftClassInf = education_table.ClassInfoInit(education_table, 0, nextRowInd);
                    rightClassInf = education_table.ClassInfoInit(education_table, nextRowInd, education_table.LearningClasses.Length);
                    double newEntrValue = EntrophyCalculator.Info(education_table) - EntrophyCalculator.EntrophyCalc(leftClassInf, rightClassInf);
                    if (newEntrValue > entrValue)
                    {
                        entrValue = newEntrValue;
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
