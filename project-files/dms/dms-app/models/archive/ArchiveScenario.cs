using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models.archive
{
    [Serializable]
    public class ArchiveScenario : ArchiveModel
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        private string learningAlgorithmName;
        public string LearningAlgorithmName
        {
            get
            {
                return learningAlgorithmName;
            }

            set
            {
                learningAlgorithmName = value;
            }
        }

        private ILAParameters laParameters;
        public ILAParameters LAParameters
        {
            get
            {
                return laParameters;
            }

            set
            {
                laParameters = value;
            }
        }

        private string selectionParameters;
        public string SelectionParameters
        {
            get
            {
                return selectionParameters;
            }

            set
            {
                selectionParameters = value;
            }
        }

        public ArchiveScenario(models.LearningScenario scenario)
        {
            this.ID = scenario.ID;
            this.Name = scenario.Name;
            this.LearningAlgorithmName = scenario.LearningAlgorithmName;
            this.SelectionParameters = scenario.SelectionParameters;
            this.LAParameters = scenario.LAParameters;
        }

        override public bool equalsEntity(models.Entity entity)
        {
            if (entity.GetType() != typeof(models.LearningScenario))
            {
                return false;
            }
            return this.Name == ((models.LearningScenario)entity).Name;
        }
    }
}
