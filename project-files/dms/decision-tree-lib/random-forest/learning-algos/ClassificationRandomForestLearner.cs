using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using dms.solvers.decision.tree.algo;
using dms.solvers.decision.tree.random_forest.model;

namespace dms.solvers.decision.tree.random_forest.learning_algos
{
    public class ClassificationRandomForestLearner : DTLearningAlgo
    {
        string usedAlgo;
        string[] TeacherTypesList;
        readonly Random m_random;
        public float[] ParamsValue;
        public ClassificationRandomForestLearner()
        {
            m_random = new Random(15);
            TeacherTypesList = new string[1];
            TeacherTypesList[0] = "Случайный лес";
            usedAlgo = TeacherTypesList[0];
            ParamsValue = new float[2] { 0, 1 };
        }        
        public float startLearn(ISolver solver, float[][] train_x, float[] train_y)
        {
            var indices = Enumerable.Range(0, train_y.Length).ToArray();
            Random random = new Random(m_random.Next());
            ClassificationForestModel dc_solver = (ClassificationForestModel)solver;
            DecisionTree[] trees = dc_solver.GetDecisionTrees();
            RandomForestDescription rfd = dc_solver.GetRandomForestDescription();
            foreach (DecisionTree dt in trees)
            {
                var treeIndicesLength = (int)Math.Round(ParamsValue[1] * (double)indices.Length);
                var treeIndices = new int[treeIndicesLength];
                float[][] new_train_x = new float[treeIndicesLength][];
                float[] new_train_y = new float[treeIndicesLength];
                for (int j = 0; j < treeIndicesLength; j++)
                {
                    int index = indices[random.Next(indices.Length)];
                    new_train_x[j] = train_x[index];
                    new_train_y[j] = train_y[index];
                }
                DTLearningAlgo algo = null;
                if (ParamsValue[0] == 0)
                {
                    algo = new DecisionTreeCARTLearningAlgo();
                }
                else if (ParamsValue[0] == 1)
                {
                    algo = new DecisionTreeC4_5LearningAlgo();
                }
                algo.startLearn(dt, new_train_x, new_train_y);
            }
            return 0;
        }
        public float[] getParams()
        {
            return ParamsValue;
        }
        public string[] AlgosTypesList { get { return new string[] { "CART", "C4.5" }; } }
        public string[] getParamsNames()
        {
            return new string[2] { "Алгоритм обучения дерева решений", "Процент [0, 1] разбиения выборки внутри леса" };
        }
        public string getType()
        {
            return "Random Forest";
        }
    }
}