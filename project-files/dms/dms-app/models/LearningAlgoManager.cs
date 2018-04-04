using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using dms.neroNetLearningAlgoritms;
using dms.models.kohonen_learning;
using dms.solvers;
using dms.solvers.decision.tree.algo;
using dms.solvers.decision.tree;

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

namespace dms.solvers.decision.tree
{
    public class DecisionTreeLearningAlgos : iLearningAlgo.ILearningAlgo
    {
        private DTLearningAlgo[] trainers;
        private int currentTrainer;

        public void setUsedAlgo(string usedAlgo)
        {
            for (int i = 0; i < trainers.Length; i++)
            {
                if (trainers[i].getType() == usedAlgo)
                {
                    currentTrainer = i;
                    break;
                }
            }
        }

        public DecisionTreeLearningAlgos()
        {
            trainers = new DTLearningAlgo[]
            {
                new DecisionTreeC4_5LearningAlgo(),
                new DecisionTreeCARTLearningAlgo()
            };
            currentTrainer = 0;
        }

        public string[] getTeacherTypesList()
        {
            List<string> types = new List<string>();
            foreach (DTLearningAlgo tr in trainers)
                types.Add(tr.getType());
            return types.ToArray();
        }

        public string[] getTeacherTypesList(ISolver solver)
        {
            List<string> types = new List<string>();

            foreach (DTLearningAlgo tr in trainers)
                    types.Add(tr.getType());
            return types.ToArray();
        }

        public float[] getParams()
        {
            return trainers[currentTrainer].getParams();
        }
        public string[] getParamsNames()
        {
            return trainers[currentTrainer].getParamsNames();
        }
        public float startLearn(ISolver solver, float[][] train_x, float[] train_y)
        {
            return trainers[currentTrainer].startLearn(solver, train_x, train_y);
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
        private const int countAlgoLib = 3;
        private string[][] myTeacherTypeList;
        private iLearningAlgo.ILearningAlgo usedLrAlgo;

        [Serializable()]
        private class AlgoParam : ILAParameters
        {
            public float[] geneticParams;
        }

        public LearningAlgoManager()
        {
            lrAlgo = new iLearningAlgo.ILearningAlgo[countAlgoLib];
            myTeacherTypeList = new string[countAlgoLib][];
            lrAlgo[0] = new NeroNetLearningAlgoritm();
            lrAlgo[1] = new KohonenLearningAlgorithms();
            lrAlgo[2] = new DecisionTreeLearningAlgos();
            algoParams = new AlgoParam();
            TeacherTypesList = new string[0];
            for (int i = 0; i < countAlgoLib; i++)
            {
                myTeacherTypeList[i] = lrAlgo[i].getTeacherTypesList();
                TeacherTypesList = TeacherTypesList.Concat(myTeacherTypeList[i]).ToArray();
            }
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

        private AlgoParam algoParams;
        public ILAParameters LAParams
        {
            get
            {
                algoParams.geneticParams = paramsValue;
                return (ILAParameters) algoParams;
            }
            set
            {
                algoParams = (AlgoParam)value;
                paramsValue = algoParams.geneticParams;
            }
        }
    }
}
