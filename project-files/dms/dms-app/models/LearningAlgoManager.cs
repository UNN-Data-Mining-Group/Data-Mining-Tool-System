using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using dms.neroNetLearningAlgoritms;
using dms.solvers;

namespace dms.iLearningAlgo
{
    public interface ILearningAlgo
    {
        string[] getTeacherTypesList();
        float[] getParams();
        string[] getParamsNames();
        void setUsedAlgo(string usedAlgo);
        float startLearn(ISolver solver, float[][] train_x, float[] train_y);
        string[] getTeacherTypesList(ISolver solver);
    }
}

namespace dms.neroNetLearningAlgoritms
{
    public class NeroNetLearningAlgoritm : iLearningAlgo.ILearningAlgo
    {
        private NeroNetLearningAlgoritms lrAlgo;

        public void setUsedAlgo(string usedAlgo)
        {
            lrAlgo.setUsedAlgo(usedAlgo);
        }

        public NeroNetLearningAlgoritm()
        {
            lrAlgo = new NeroNetLearningAlgoritms();
        }

        public string[] getTeacherTypesList()
        {
            return lrAlgo.getTeacherTypesList();
        }

        public string[] getTeacherTypesList(ISolver solver)
        {
            return lrAlgo.getTeacherTypesList(solver);
        }

        public float[] getParams()
        {
            return lrAlgo.getParams();
        }
        public string[] getParamsNames()
        {
            return lrAlgo.getParamsNames();
        }
        public float startLearn(ISolver solver, float[][] train_x, float[] train_y)
        {
            float res;
            try
            {
                res = lrAlgo.startLearn(solver, train_x, train_y);
                ((dms.solvers.neural_nets.INeuralNetwork)solver).FetchNativeParameters();
            }catch(ArgumentException e)
            {
                res = -1;
                System.Windows.MessageBox.Show(e.Message);
            }
            
            return res;
        }
    }
}


namespace dms.models
{

    

    public class LearningAlgoManager 
    {
        //     [DllImport("dms-learning-algo.dll")]
        //     private static extern float genom();
        private iLearningAlgo.ILearningAlgo[] lrAlgo;
        private const int countAlgoLib = 1;
        private string[][] myTeacherTypeList;
        private iLearningAlgo.ILearningAlgo usedLrAlgo;

        [Serializable()]
        private class GeneticParam : ILAParameters
        {
            public float[] geneticParams;
        }

        public LearningAlgoManager()
        {
            lrAlgo = new iLearningAlgo.ILearningAlgo[countAlgoLib];
            myTeacherTypeList = new string[countAlgoLib][];
            lrAlgo[0] = new NeroNetLearningAlgoritm();
            geneticParams = new GeneticParam();
            TeacherTypesList = new string[0];
            for (int i = 0; i < countAlgoLib; i++)
            {
                myTeacherTypeList[i] = lrAlgo[i].getTeacherTypesList();
                TeacherTypesList = TeacherTypesList.Concat(myTeacherTypeList[i]).ToArray();
            }

  //          TeacherTypesList = lrAlgo[0].getTeacherTypesList();


            //new string[] { "Обучатель 1", "Обучатель 2", "Обучатель 3" };
//            ParamsName = lrAlgo[0].getParamsNames();
            
//            ParamsValue = lrAlgo[0].getParams(); //new float[] { 0, 0.3f, 1f, 5f };
            
           
        }
        public float startLearn(ISolver solver,float[][] train_x,float[] train_y)
        {
            float res = usedLrAlgo.startLearn(solver, train_x, train_y);
            return res;
            
        }
        private string[] ParamsName;
        public string[] paramsName
        {
            get
            {
                return ParamsName;
            }
        }

        public string[] getTeacherTypesList(ISolver solver)
        {
            string[] tmpTeacherTypeList = new string[0];
            for (int i = 0; i < countAlgoLib; i++)
            {
                tmpTeacherTypeList = tmpTeacherTypeList.Concat(lrAlgo[i].getTeacherTypesList(solver)).ToArray();
            }
            return tmpTeacherTypeList;
        }

        private float[] ParamsValue;
        public float[] paramsValue
        {
            get
            {
                return ParamsValue;
            }
            set
            {
                for (int i = 0; i < ParamsValue.Length; i++)
                {
                    ParamsValue[i] = value[i];
                }
            }
        }

        private string[] TeacherTypesList;
        public string[] teacherTypesList
        {
            get
            {
                return TeacherTypesList;
            }
        }

        private string UsedAlgo;
        public string usedAlgo
        {
            get
            {
                return UsedAlgo;
            }
            set
            {
                UsedAlgo = value;
                for (int i = 0; i < countAlgoLib; i++)
                {
                    foreach(string algo in myTeacherTypeList[i])
                    {
                        if(algo.Equals(UsedAlgo))
                        {
                            usedLrAlgo = lrAlgo[i];
                            usedLrAlgo.setUsedAlgo(UsedAlgo);
                            ParamsName = usedLrAlgo.getParamsNames();
                            ParamsValue = usedLrAlgo.getParams();
                            return;
                        }
                    }
                }
            }
        }

        private GeneticParam geneticParams;
        public ILAParameters GeneticParams
        {
            get
            {
                geneticParams.geneticParams = paramsValue;
                return (ILAParameters) geneticParams;
            }
            set
            {
                geneticParams = (GeneticParam)value;
                paramsValue = geneticParams.geneticParams;
            }
        }
    }
}
