using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.iLearningAlgo;
using dms.solvers;
using dms.kohonen_learning_algorithms;

namespace dms.models.kohonen_learning
{
    public class KohonenLearningAlgorithms : ILearningAlgo
    {
        private KohonenTrainer[] trainers;
        private int currentTrainer;

        public KohonenLearningAlgorithms()
        {
            trainers = new KohonenTrainer[]
            {
                new KohonenClassifier(),
                new KohonenSelfOrganizer()
            };
            currentTrainer = 0;
        }

        public float[] getParams()
        {
            return trainers[currentTrainer].getParams();
        }

        public string[] getParamsNames()
        {
            return trainers[currentTrainer].getParamsNames();
        }

        public string[] getTeacherTypesList()
        {
            List<string> types = new List<string>();
            foreach (KohonenTrainer tr in trainers)
                types.Add(tr.getType());
            return types.ToArray();
        }

        public string[] getTeacherTypesList(ISolver solver)
        {
            List<string> types = new List<string>();

            foreach (KohonenTrainer tr in trainers)
                if (tr.canTrain(solver) == true)
                    types.Add(tr.getType());
            return types.ToArray();
        }

        public void setUsedAlgo(string usedAlgo)
        {
            for(int i = 0; i < trainers.Length; i++)
            {
                if (trainers[i].getType() == usedAlgo)
                {
                    currentTrainer = i;
                    break;
                }
            }
        }

        public float startLearn(ISolver solver, float[][] train_x, float[] train_y)
        {
            return trainers[currentTrainer].learn(solver, train_x, train_y);
        }
    }
}
