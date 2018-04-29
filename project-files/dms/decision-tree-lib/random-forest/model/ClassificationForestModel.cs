using System;
using System.Collections.Generic;
using System.Linq;

namespace dms.solvers.decision.tree.random_forest.model
{
    [Serializable]
    public sealed class ClassificationForestModel : ISolver
    {
        private DecisionTree[] m_models;
        readonly double[] m_rawVariableImportance;
        private RandomForestDescription randomForestDescription;
        public ClassificationForestModel(RandomForestDescription randomForestDescription) : base(randomForestDescription)
        {
            this.randomForestDescription = randomForestDescription;
            m_models = new DecisionTree[randomForestDescription.GetNumberTrees()];
            for (int i = 0; i < m_models.Length; i++)
                m_models[i] = new DecisionTree(new TreeDescription(randomForestDescription.GetInputsCount(),
                                                                    randomForestDescription.GetOutputsCount(),
                                                                    10));

        }
        public override float[] Solve(float[] observation)
        {
            var prediction = m_models.Select(m => m.Solve(observation))
                .GroupBy(p => p).OrderByDescending(g => g.Count())
                .First().Key;
            return prediction;
        }
        public Dictionary<string, double> GetVariableImportance(Dictionary<string, int> featureNameToIndex)
        {
            var max = m_rawVariableImportance.Max();

            var scaledVariableImportance = m_rawVariableImportance
                .Select(v => (v / max) * 100.0)
                .ToArray();

            return featureNameToIndex.ToDictionary(kvp => kvp.Key, kvp => scaledVariableImportance[kvp.Value])
                        .OrderByDescending(kvp => kvp.Value)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        public double[] GetRawVariableImportance()
        {
            return m_rawVariableImportance;
        }

        public DecisionTree[] GetDecisionTrees()
        {
            return m_models;
        }

        public RandomForestDescription GetRandomForestDescription()
        {
            return randomForestDescription;
        }

        public override ISolver Copy() {
            RandomForestDescription dtDescr = new RandomForestDescription(randomForestDescription.GetInputsCount(), randomForestDescription.GetOutputsCount(), randomForestDescription.GetNumberTrees());
            ClassificationForestModel newForest = new ClassificationForestModel(dtDescr);
            return newForest;
        }
    }
}