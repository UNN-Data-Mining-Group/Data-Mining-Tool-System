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
        readonly int m_trees;
        int m_featuresPrSplit;
        readonly int m_minimumSplitSize;
        readonly double m_minimumInformationGain;
        readonly double m_subSampleRatio;
        readonly int m_maximumTreeDepth;
        readonly Random m_random;
        readonly bool m_runParallel;

        public ClassificationRandomForestLearner(int trees = 100, int minimumSplitSize = 1, int maximumTreeDepth = 2000,
            int featuresPrSplit = 0, double minimumInformationGain = .000001, double subSampleRatio = 1.0, int seed = 42, bool runParallel = true)
        {
            if (trees < 1) { throw new ArgumentException("trees must be at least 1"); }
            if (featuresPrSplit < 0) { throw new ArgumentException("features pr split must be at least 1"); }
            if (minimumSplitSize <= 0) { throw new ArgumentException("minimum split size must be larger than 0"); }
            if (maximumTreeDepth <= 0) { throw new ArgumentException("maximum tree depth must be larger than 0"); }
            if (minimumInformationGain <= 0) { throw new ArgumentException("minimum information gain must be larger than 0"); }
            if (subSampleRatio <= 0.0 || subSampleRatio > 1.0) { throw new ArgumentException("subSampleRatio must be larger than 0.0 and at max 1.0"); }

            m_trees = trees;
            m_minimumSplitSize = minimumSplitSize;
            m_maximumTreeDepth = maximumTreeDepth;
            m_featuresPrSplit = featuresPrSplit;
            m_minimumInformationGain = minimumInformationGain;
            m_subSampleRatio = subSampleRatio;
            m_runParallel = runParallel;

            m_random = new Random(seed);
        }
        public float startLearn(ISolver solver, float[][] train_x, float[] train_y)
        {
            ClassificationForestModel dc_solver = (ClassificationForestModel)solver;
            DecisionTree[] trees = dc_solver.GetDecisionTrees();
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
            throw new NotImplementedException();
        }

        public string[] getParamsNames()
        {
            throw new NotImplementedException();
        }
        public string getType()
        {
            return "RandomForest";
        }
    }
}