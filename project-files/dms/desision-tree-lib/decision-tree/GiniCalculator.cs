﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desision_tree_lib.decision_tree
{
    class GiniCalculator
    {
        public static double GiniSplitCalc(LearningClassInfo[] leftClassInf, LearningClassInfo[] rightClassInf) //TODO: Переделать
        {
            double res = 0;
            int examplCntLeft = 0, examplCntRight = 0;
            for (int i = 0; i < leftClassInf.Length; i++)
            {
                examplCntLeft += leftClassInf[i].number_of_checked;
                examplCntRight += rightClassInf[i].number_of_checked;
            }

            for (int i = 0; i < leftClassInf.Length; i++)
            {
                res += ((Math.Pow(leftClassInf[i].number_of_checked, 2) / examplCntLeft) + (Math.Pow(rightClassInf[i].number_of_checked, 2) / examplCntRight));
            }
            return res;
        }
    }
}
