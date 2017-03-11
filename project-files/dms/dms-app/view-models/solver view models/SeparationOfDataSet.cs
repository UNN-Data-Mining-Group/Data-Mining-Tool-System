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
        private int mistakeTrain;
        private int mistakeTest;
        public SeparationOfDataSet(solvers.ISolver solver, LearningScenario learnignScenario, float [][]x, float []y)
        {
            ISolver = solver;
            LS = learnignScenario;
            InputData = x;
            OutputData = y;
        }

        public float[][] InputData { get; private set; }
        public float TestError {
            get
            {
                return mistakeTest;
            }
        }
        public float TrainError
        {
            get
            {
                return mistakeTrain;
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

            }
            else throw new Exception("Unknown error");
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
            mistakeTest /= sizeTrainDataset;
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
    }
}
