using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using dms.solvers.decision.tree.algo;
using dms.solvers.decision.tree.random_forest.model;

namespace dms.solvers.decision.tree.random_forest.learning_algos
{
    public sealed class ClassificationRandomForestLearner : DTLearningAlgo
    {
        string usedAlgo;
        string[] TeacherTypesList;
        readonly int m_trees;
        int m_featuresPrSplit;
        readonly int m_minimumSplitSize;
        readonly double m_minimumInformationGain;
        readonly double m_subSampleRatio;
        readonly int m_maximumTreeDepth;
        readonly Random m_random;
        readonly bool m_runParallel;

        public ClassificationRandomForestLearner()
        {
            TeacherTypesList = new string[1];
            TeacherTypesList[0] = "Случайный лес";
            usedAlgo = TeacherTypesList[0];
        }

        public void setUsedAlgo(string used_algo)
        {
            usedAlgo = used_algo;
        }


        public string[] getTeacherTypesList()
        {
            return TeacherTypesList;
        }

        public string[] getTeacherTypesList(ISolver solver)
        {
            return TeacherTypesList;
        }
        public float startLearn(ISolver solver, float[][] train_x, float[] train_y)
        {
            ClassificationForestModel dc_solver = (ClassificationForestModel)solver;
            DecisionTree[] trees = dc_solver.GetDecisionTrees();
            RandomForestDescription rfd = dc_solver.GetRandomForestDescription();
            foreach (DecisionTree dt in trees)
            {
                DecisionTreeCARTLearningAlgo algo = new DecisionTreeCARTLearningAlgo();
                algo.startLearn(dt, train_x, train_y);
            }
            return 0;
        }
        /* public ClassificationForestModel Learn(float[][] observations, float[] targets)
        {
            if (m_featuresPrSplit == 0)
            {
                var count = (int)Math.Sqrt(observations.Length);
                m_featuresPrSplit = count <= 0 ? 1 : count;
            }

            var results = new ConcurrentBag<DecisionTree>();

            if (!m_runParallel)
            {
                for (int i = 0; i < m_trees; i++)
                {
                    results.Add(CreateTree(observations, targets, new Random(m_random.Next())));
                };
            }
            else
            {
                var workItems = Enumerable.Range(0, m_trees).ToArray();
                var rangePartitioner = Partitioner.Create(workItems, true);
                Parallel.ForEach(rangePartitioner, (work, loopState) =>
                {
                    results.Add(CreateTree(observations, targets, new Random(m_random.Next())));
                });
            }

            var models = results.ToArray();
            var rawVariableImportance = VariableImportance(models, observations.Length);

            return new ClassificationForestModel(new TreeDescription(0, 0, 10), models, rawVariableImportance);
        }
        */
        double[] VariableImportance(DecisionTree[] models, int numberOfFeatures)
        {
            var rawVariableImportance = new double[numberOfFeatures];

            /* foreach (var model in models)
            {
                var modelVariableImportance = model.GetRawVariableImportance();

                for (int j = 0; j < modelVariableImportance.Length; j++)
                {
                    rawVariableImportance[j] += modelVariableImportance[j];
                }
            } */
            return rawVariableImportance;
        }

        DecisionTree CreateTree(float[][] observations, float[] targets, Random random)
        {
            var learner = new DecisionTreeCARTLearningAlgo();

            DecisionTree tree = new DecisionTree(new TreeDescription(0, 0, 10));
            learner.startLearn(tree, observations, targets);

            return tree;
        }

        public float[] getParams()
        {
            return new float[0];
        }

        public string[] getParamsNames()
        {
            return new string[1] { "Алгоритм обучения дерева решений" };
        }
        public string getType()
        {
            return "Random Forest";
        }
    }
}