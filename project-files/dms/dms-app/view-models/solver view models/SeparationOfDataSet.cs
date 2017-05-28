using dms.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers;
using dms.solvers.neural_nets;
using dms.solvers.neural_nets.kohonen;
using dms.services.preprocessing;
using dms.solvers.decision_tree;

namespace dms.view_models.solver_view_models
{
    class SeparationOfDataSet
    {
        private float mistakeTrain = 0;
        private float mistakeTest = 0;
        public SeparationOfDataSet(ISolver solver, LearningScenario learnignScenario, float [][]x, float []y)
        {
            ISolver = solver;
            LS = learnignScenario;
            InputData = x;
            OutputData = y;
        }

        public float[][] InputData { get; private set; }
        public float MistakeTest {
            get
            {
                return mistakeTest * 100;
            }
        }
        public float MistakeTrain
        {
            get
            {
                return mistakeTrain * 100;
            }
        }
        public float ClosingError { get; set; }
        public LearningScenario LS { get; private set; }
        public float[] OutputData { get; private set; }
        public ISolver ISolver { get; private set; }
        public int SelectionID { get; private set; }
        public int ParameterID { get; private set; }

        public ISolver separationAndLearn(int selectionID, int parameterID)
        {
            SelectionID = selectionID;
            ParameterID = parameterID;
            string selectionType = LS.SelectionParameters.Split(',')[0];
            mixDataset();
            if (selectionType.Equals("Тестовая/обучающая"))
            {
                int percentTrain = int.Parse(LS.SelectionParameters.Split(',')[2]);
                return simpleSeparation(percentTrain); 
            }
            else if (selectionType.Equals("Кроссвалидация"))
            {
                int kfolds = int.Parse(LS.SelectionParameters.Split(',')[2]);
                return crossValidation(kfolds);
            }
            else throw new Exception("Unknown error");
        }

        private ISolver crossValidation(int folds)
        {
            ISolver curSolver = ISolver.Copy();
            List<float> listOfTestMistakes = new List<float>();
            for (int k = 0; k < folds; k++)
            {
                float kMistakeTrain = 0;
                float kMistakeTest = 0;
                float[][] trainInputDataset = GetInputTrainData(InputData, folds, k);
                float[][] testInputDataset = GetInputTestData(InputData, folds, k);
                float[] trainOutputDataset = GetOutputTrainData(OutputData, folds, k);
                float[] testOutputDataset = GetTestOutputData(OutputData, folds, k);

                LearningAlgoManager la = new LearningAlgoManager()
                {
                    usedAlgo = LS.LearningAlgorithmName,
                    LAParams = LS.LAParameters

                };
                PreprocessingManager preprocessing = new PreprocessingManager();
                ClosingError = la.startLearn(ISolver, trainInputDataset, trainOutputDataset);
                int sizeTrainDataset = trainInputDataset.Length;
                List<string> expectedOutputValues = trainOutputDataset.Select(x => Convert.ToString(x)).ToList();
                List<string> obtainedOutputValues = new List<string>();

                if (ISolver is KohonenManaged)
                {
                    KohonenManaged koh = ISolver as KohonenManaged;
                    koh.startLogWinners();
                }
                for (int i = 0; i < sizeTrainDataset; i++)
                {
                    obtainedOutputValues.Add(Convert.ToString(ISolver.Solve(trainInputDataset[i])[0]));
                }
                List<bool> comparisonOfResult = preprocessing.compareExAndObValues(expectedOutputValues, obtainedOutputValues, SelectionID, ParameterID);
                if (ISolver is KohonenManaged)
                {
                    KohonenManaged koh = ISolver as KohonenManaged;
                    koh.stopLogWinners();
                    koh.declareWinnersOutput(comparisonOfResult);
                }

                var counts = comparisonOfResult.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
                kMistakeTrain = (float)counts[false] / (float)sizeTrainDataset;

                int sizeTestDataset = testOutputDataset.Length;
                expectedOutputValues = testOutputDataset.Select(x => Convert.ToString(x)).ToList();
                obtainedOutputValues.Clear();
                for (int i = 0; i < sizeTestDataset; i++)
                {
                    obtainedOutputValues.Add(Convert.ToString(ISolver.Solve(testInputDataset[i])[0]));
                }
                comparisonOfResult = preprocessing.compareExAndObValues(expectedOutputValues, obtainedOutputValues, SelectionID, ParameterID);
                counts = comparisonOfResult.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
                kMistakeTest = (float)counts[false] / (float)sizeTestDataset;
                if (k != 0 && kMistakeTest < listOfTestMistakes.Min())
                {
                    curSolver = ISolver.Copy();
                    mistakeTest = kMistakeTest;
                    mistakeTrain = kMistakeTrain;
                }
                listOfTestMistakes.Add(kMistakeTest);
            }
            ISolver = curSolver.Copy() as INeuralNetwork;

            return ISolver;
        }

        public ISolver simpleSeparation(int percentTrain)
        {
            int sizeTrainDataset = Convert.ToInt32(InputData.Length * ((double)percentTrain / 100));
            int sizeTestDataset = InputData.Length - sizeTrainDataset;
            float[][] trainInputDataset = new float[sizeTrainDataset][];
            float[][] testInputDataset = new float[InputData.Length - sizeTrainDataset][];
            float[] trainOutputDataset = new float[sizeTrainDataset];
            float[] testOutputDataset = new float[InputData.Length - sizeTrainDataset];
            Array.Copy(InputData, trainInputDataset, sizeTrainDataset);
            Array.Copy(InputData, sizeTrainDataset, testInputDataset, 0, sizeTestDataset);
            Array.Copy(OutputData, trainOutputDataset, sizeTrainDataset);
            Array.Copy(OutputData, sizeTrainDataset, testOutputDataset, 0, sizeTestDataset);

            if (ISolver is INeuralNetwork)
            {
                LearningAlgoManager la = new LearningAlgoManager()
                {
                    usedAlgo = LS.LearningAlgorithmName,
                    LAParams = LS.LAParameters

                };
                ClosingError = la.startLearn(ISolver, trainInputDataset, trainOutputDataset);
            }
            else if (ISolver is DecisionTree)
            {
                DecisionTreeCARTLearningAlgo la = new DecisionTreeCARTLearningAlgo();
                ClosingError = la.startLearn(ISolver, trainInputDataset, trainOutputDataset);
            }

            PreprocessingManager preprocessing = new PreprocessingManager();
            mistakeTrain = 0;
            List<string> expectedOutputValues = trainOutputDataset.Select(x => Convert.ToString(x)).ToList();
            List<string> obtainedOutputValues = new List<string>();

            if (ISolver is KohonenManaged)
            {
                KohonenManaged koh = ISolver as KohonenManaged;
                koh.startLogWinners();
            }
            for (int i = 0; i < sizeTrainDataset; i++)
            {
                obtainedOutputValues.Add(Convert.ToString(ISolver.Solve(trainInputDataset[i])[0]));
            }
            List<bool> comparisonOfResult = preprocessing.compareExAndObValues(expectedOutputValues, obtainedOutputValues, SelectionID, ParameterID);
            if (ISolver is KohonenManaged)
            {
                KohonenManaged koh = ISolver as KohonenManaged;
                koh.stopLogWinners();
                koh.declareWinnersOutput(comparisonOfResult);
            }

            var counts = comparisonOfResult.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            if (counts.ContainsKey(false))
            {
                mistakeTrain = (float)counts[false] / (float)sizeTrainDataset;
            }
            else
            {
                mistakeTrain = 0;
            }

            mistakeTest = 0;
            expectedOutputValues = testOutputDataset.Select(x => Convert.ToString(x)).ToList();
            obtainedOutputValues.Clear();
            for (int i = 0; i < sizeTestDataset; i++)
            {
                obtainedOutputValues.Add(Convert.ToString(ISolver.Solve(testInputDataset[i])[0]));
            }
            comparisonOfResult = preprocessing.compareExAndObValues(expectedOutputValues, obtainedOutputValues, SelectionID, ParameterID);
            counts = comparisonOfResult.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            if (counts.ContainsKey(false))
            {
                mistakeTest = (float)counts[false] / (float)sizeTestDataset;
            }
            else
                mistakeTest = 0;

            return ISolver;
        }

        private void mixDataset()
        {
            int seed = int.Parse(LS.SelectionParameters.Split(',')[1]);
            Random r = new Random(seed);
            for (int i = InputData.Length - 1; i >= 1; i--)
            {
                int j = r.Next(i + 1);

                var inputTemp = InputData[j];
                InputData[j] = InputData[i];
                InputData[i] = inputTemp;

                var outputTemp = OutputData[j];
                OutputData[j] = OutputData[i];
                OutputData[i] = outputTemp;
            }
        }

        float[][] GetInputTrainData(float[][] inputData, int numFolds, int fold)
        {
            int[][] firstAndLastTest = GetFirstLastTest(inputData.Length, numFolds); 
            int numTrain = inputData.Length - (firstAndLastTest[fold][1] - firstAndLastTest[fold][0] + 1);
            float[][] result = new float[numTrain][];
            int i = 0; 
            int ia = 0; 
            while (i < result.Length)
            {
                if (ia < firstAndLastTest[fold][0] || ia > firstAndLastTest[fold][1]) 
                {
                    result[i] = inputData[ia];
                    ++i;
                }
                ++ia;
            }
            return result;
        }

        float[] GetOutputTrainData(float[] outputData, int numFolds, int fold)
        {
            int[][] firstAndLastTest = GetFirstLastTest(outputData.Length, numFolds); 
            int numTrain = outputData.Length - (firstAndLastTest[fold][1] - firstAndLastTest[fold][0] + 1);
            float[] result = new float[numTrain];
            int i = 0; 
            int ia = 0; 
            while (i < result.Length)
            {
                if (ia < firstAndLastTest[fold][0] || ia > firstAndLastTest[fold][1]) 
                {
                    result[i] = outputData[ia];
                    ++i;
                }
                ++ia;
            }
            return result;
        }

        float[][] GetInputTestData(float[][] inputData, int numFolds, int fold)
        {
            
            int[][] firstAndLastTest = GetFirstLastTest(inputData.Length, numFolds); 
            int numTest = firstAndLastTest[fold][1] - firstAndLastTest[fold][0] + 1;
            float[][] result = new float[numTest][];
            int ia = firstAndLastTest[fold][0]; 
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = inputData[ia]; 
                ++ia;
            }
            return result;
        }

        float[] GetTestOutputData(float[] outputData, int numFolds, int fold)
        {
            
            int[][] firstAndLastTest = GetFirstLastTest(outputData.Length, numFolds); 
            int numTest = firstAndLastTest[fold][1] - firstAndLastTest[fold][0] + 1;
            float[] result = new float[numTest];
            int ia = firstAndLastTest[fold][0]; 
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = outputData[ia]; 
                ++ia;
            }
            return result;
        }

        int[][] GetFirstLastTest(int numDataItems, int numFolds)
        {
            int interval = numDataItems / numFolds;  
            int[][] result = new int[numFolds][]; 
            for (int i = 0; i < result.Length; ++i)
                result[i] = new int[2];

            for (int k = 0; k < numFolds; ++k) 
            {
                int first = k * interval; 
                int last = (k + 1) * interval - 1; 
                result[k][0] = first;
                result[k][1] = last;
            }

            result[numFolds - 1][1] = result[numFolds - 1][1] + numDataItems % numFolds;
            return result;
        }
    }
}
