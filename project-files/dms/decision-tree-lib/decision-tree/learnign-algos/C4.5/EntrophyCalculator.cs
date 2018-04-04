using dms.solvers.decision.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.solvers.decision.tree.algo
{
    public class EntrophyCalculator
    {
        public static double Info(LearningTable T)
        {
            double info = 0;
            double P = 0;
            LearningClassInfo[] classInfo = T.ClassInfoInit(T);
            for(int i = 0; i < classInfo.Length; i++)
            {
                P = (double)classInfo[i].number_of_checked / T.LearningClasses.Length;
                if (P != 0)
                    info += P * Math.Log(P, 2);
            }

            return -1 * info;
        }

        public static double Info(LearningClassInfo[] classInfo)
        {
            double info = 0;
            double P = 0;
            double exampleCnt = 0;
            for (int i = 0; i < classInfo.Length; i++)
            {
                exampleCnt += classInfo[i].number_of_checked;
            }
            for (int i = 0; i < classInfo.Length; i++)
            {
                P = (double)classInfo[i].number_of_checked / exampleCnt;
                if (P != 0)
                    info += P * Math.Log(P, 2);
            }

            return -1 * info;
        }

        public static double EntrophyCalc(LearningClassInfo[] leftClassInf, LearningClassInfo[] rightClassInf)
        {
            double res = 0;
            int examplCntLeft = 0, examplCntRight = 0;
            for (int i = 0; i < leftClassInf.Length; i++)
            {
                examplCntLeft += leftClassInf[i].number_of_checked;
                examplCntRight += rightClassInf[i].number_of_checked;
            }

            res = ((double)examplCntLeft / (examplCntLeft + examplCntRight) * Info(leftClassInf)) + ((double)examplCntRight / (examplCntLeft + examplCntRight) * Info(rightClassInf));

            return res;
        }

    }
}
