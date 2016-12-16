using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    interface ILAParameters
    {

    }

    class LearningScenario : Entity
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

        public LearningScenario()
        {
            this.nameTable = "LearningScenario";
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("Name", "Name");
            mappingTable.Add("SelectionParameters", "SelectionParameters");
            mappingTable.Add("LearningAlgorithmName", "LearningAlgorithmName");
            mappingTable.Add("LAParameters", "LAParameters");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }

        public override Dictionary<string, Type> serializationParameters()
        {
            Dictionary<string, Type> serializationParameters = new Dictionary<string, Type>();
            serializationParameters.Add("LAParameters", typeof(ILAParameters));
            base.serializationParameters().ToList().ForEach(x => serializationParameters.Add(x.Key, x.Value));
            return serializationParameters;
        }
    }
}
