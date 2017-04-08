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
        float startLearn(ISolver solver, float[][] train_x, float[] train_y);
    }
}

namespace dms.neroNetLearningAlgoritms
{
    public class NeroNetLearningAlgoritm : iLearningAlgo.ILearningAlgo
    {
        private NeroNetLearningAlgoritms lrAlgo;

        public NeroNetLearningAlgoritm()
        {
            lrAlgo = new NeroNetLearningAlgoritms();
        }

        public string[] getTeacherTypesList()
        {
            return lrAlgo.getTeacherTypesList();
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
            return lrAlgo.startLearn(solver, train_x, train_y);
        }
    }
}


namespace dms.models
{

    

    public class LearningAlgoManger 
    {
        //     [DllImport("dms-learning-algo.dll")]
        //     private static extern float genom();
        private NeroNetLearningAlgoritm lrAlgo;

        [Serializable()]
        private class GeneticParam : ILAParameters
        {
            public float[] geneticParams;
        }

        public LearningAlgoManger()
        {
            lrAlgo = new NeroNetLearningAlgoritm();
            geneticParams = new GeneticParam();
            TeacherTypesList = lrAlgo.getTeacherTypesList();


            //new string[] { "Обучатель 1", "Обучатель 2", "Обучатель 3" };
            ParamsName = lrAlgo.getParamsNames();
            
            ParamsValue = lrAlgo.getParams(); //new float[] { 0, 0.3f, 1f, 5f };
            
           
        }
        public float startLearn(ISolver solver,float[][] train_x,float[] train_y)
        {
            float res = lrAlgo.startLearn(solver, train_x, train_y);
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
