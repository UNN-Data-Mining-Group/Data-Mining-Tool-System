using dms.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers;

namespace dms.view_models.solver_view_models
{
    class SeparationOfDataSet
    {
        private float mistakeTrain = 0;
        private float mistakeTest = 0;
        public SeparationOfDataSet(solvers.ISolver solver, LearningScenario learnignScenario, float [][]x, float []y)
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
        public LearningScenario LS { get; private set; }
        public float[] OutputData { get; private set; }
        public ISolver ISolver { get; private set; }

        public void separationAndLearn()
        {
            string selectionType = LS.SelectionParameters.Split(',')[0];
            mixDataset();
            if (selectionType.Equals("Тестовая/обучающая"))
            {
                simpleSeparation(); 
            }
            else if (selectionType.Equals("Кроссвалидация"))
            {
                crossValidation();
            }
            else throw new Exception("Unknown error");
        }

        private void crossValidation()
        {
            int folds = 5;
            for (int k = 0; k < folds; k++)
            {
                float kMistakeTrain = 0;
                float kMistakeTest = 0;
                float[][] trainInputDataset = GetInputTrainData(InputData, folds, k);
                float[][] testInputDataset = GetInputTestData(InputData, folds, k);
                float[] trainOutputDataset = GetOutputTrainData(OutputData, folds, k);
                float[] testOutputDataset = GetTestOutputData(OutputData, folds, k);

                LearningAlgo la = new LearningAlgo()
                {
                    usedAlgo = LS.LearningAlgorithmName,
                    GeneticParams = (GeneticParam)LS.LAParameters

                };

                la.startLearn(ISolver, trainInputDataset, trainOutputDataset);
                int sizeTrainDataset = trainInputDataset.Length;
                for (int i = 0; i < sizeTrainDataset; i++)
                {
                    float[] expectedOutputDataset = ISolver.Solve(trainInputDataset[i]);
                    if (expectedOutputDataset[0] != trainOutputDataset[i])
                    {
                        kMistakeTrain++;
                    }
                }
                kMistakeTrain /= sizeTrainDataset;
                
                int sizeTestDataset = trainOutputDataset.Length;
                for (int i = 0; i < sizeTestDataset; i++)
                {
                    float[] expectedOutputDataset = ISolver.Solve(trainInputDataset[i]);
                    if (expectedOutputDataset[0] != testOutputDataset[i])
                    {
                        kMistakeTest++;
                    }
                }
                kMistakeTest /= sizeTestDataset;

                mistakeTrain += kMistakeTrain;
                mistakeTest += kMistakeTest;
            }
            mistakeTrain /= folds;
            mistakeTest /= folds;
        }

        public void simpleSeparation()
        {
            int sizeTrainDataset = Convert.ToInt32(InputData.Length * 0.5);
            int sizeTestDataset = InputData.Length - sizeTrainDataset;
            float [][] trainInputDataset = new float[sizeTrainDataset][];
            float [][] testInputDataset = new float[InputData.Length - sizeTrainDataset][];
            float[] trainOutputDataset = new float[sizeTrainDataset];
            float[] testOutputDataset = new float[InputData.Length - sizeTrainDataset];
            Array.Copy(InputData, trainInputDataset, sizeTrainDataset);
            Array.Copy(InputData, sizeTrainDataset, testInputDataset, 0, sizeTrainDataset);
            Array.Copy(OutputData, trainOutputDataset, sizeTrainDataset);
            Array.Copy(OutputData, sizeTrainDataset, testOutputDataset, 0, sizeTrainDataset);


            LearningAlgo la = new LearningAlgo()
            {
                usedAlgo = LS.LearningAlgorithmName,
                GeneticParams = (GeneticParam)LS.LAParameters

            };

            la.startLearn(ISolver, trainInputDataset, trainOutputDataset);
            mistakeTrain = 0;
            for(int i = 0; i < sizeTrainDataset; i++)
            {
                float[] expectedOutputDataset = ISolver.Solve(trainInputDataset[i]);
                if (expectedOutputDataset[0] != trainOutputDataset[i])
                {
                    mistakeTrain++;
                }
            }
            mistakeTrain /= sizeTrainDataset;

            mistakeTest = 0;
            for (int i = 0; i < sizeTestDataset; i++)
            {
                float[] expectedOutputDataset = ISolver.Solve(trainInputDataset[i]);
                if (expectedOutputDataset[0] != testOutputDataset[i])
                {
                    mistakeTest++;
                }
            }
            mistakeTest /= sizeTestDataset;
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
            int[][] firstAndLastTest = GetFirstLastTest(inputData.Length, numFolds); // first and last index of rows tagged as TEST data
            int numTrain = inputData.Length - (firstAndLastTest[fold][1] - firstAndLastTest[fold][0] + 1); // tot num rows - num test rows
            float[][] result = new float[numTrain][];
            int i = 0; // index into result/test data
            int ia = 0; // index into all data
            while (i < result.Length)
            {
                if (ia < firstAndLastTest[fold][0] || ia > firstAndLastTest[fold][1]) // this is a TRAIN row
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
            int[][] firstAndLastTest = GetFirstLastTest(outputData.Length, numFolds); // first and last index of rows tagged as TEST data
            int numTrain = outputData.Length - (firstAndLastTest[fold][1] - firstAndLastTest[fold][0] + 1); // tot num rows - num test rows
            float[] result = new float[numTrain];
            int i = 0; // index into result/test data
            int ia = 0; // index into all data
            while (i < result.Length)
            {
                if (ia < firstAndLastTest[fold][0] || ia > firstAndLastTest[fold][1]) // this is a TRAIN row
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
            // return a reference to TEST data
            int[][] firstAndLastTest = GetFirstLastTest(inputData.Length, numFolds); // first and last index of rows tagged as TEST data
            int numTest = firstAndLastTest[fold][1] - firstAndLastTest[fold][0] + 1;
            float[][] result = new float[numTest][];
            int ia = firstAndLastTest[fold][0]; // index into all data
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = inputData[ia]; // the test data indices are contiguous
                ++ia;
            }
            return result;
        }

        float[] GetTestOutputData(float[] outputData, int numFolds, int fold)
        {
            // return a reference to TEST data
            int[][] firstAndLastTest = GetFirstLastTest(outputData.Length, numFolds); // first and last index of rows tagged as TEST data
            int numTest = firstAndLastTest[fold][1] - firstAndLastTest[fold][0] + 1;
            float[] result = new float[numTest];
            int ia = firstAndLastTest[fold][0]; // index into all data
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = outputData[ia]; // the test data indices are contiguous
                ++ia;
            }
            return result;
        }

        int[][] GetFirstLastTest(int numDataItems, int numFolds)
        {
            // return[fold][firstIndex][lastIndex] for k-fold cross validation TEST data
            int interval = numDataItems / numFolds;  // if there are 32 data items and k = num folds = 3, then interval = 32/3 = 10
            int[][] result = new int[numFolds][]; // pair of indices for each fold
            for (int i = 0; i < result.Length; ++i)
                result[i] = new int[2];

            for (int k = 0; k < numFolds; ++k) // 0, 1, 2
            {
                int first = k * interval; // 0, 10, 20
                int last = (k + 1) * interval - 1; // 9, 19, 29 (should be 31)
                result[k][0] = first;
                result[k][1] = last;
            }

            result[numFolds - 1][1] = result[numFolds - 1][1] + numDataItems % numFolds; // 29->31
            return result;
        }
    }
}
