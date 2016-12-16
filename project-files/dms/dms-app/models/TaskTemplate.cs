using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    interface IPreprocessingParameters
    {
        void testMethod();
    }

    [Serializable()]
    public class Test
    {
        public string tech = "test";
        public bool isText = false;
    }

    [Serializable()]
    public class Test1 : IPreprocessingParameters
    {
        public Test test = new Test();
        public string name = "b";
        public int color = 0;

        public void testMethod()
        {

        }
    }

    class TaskTemplate : Entity
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

        private int taskID;
        public int TaskID
        {
            get
            {
                return taskID;
            }

            set
            {
                taskID = value;
            }
        }

        private IPreprocessingParameters preprocessingParameters;
        public IPreprocessingParameters PreprocessingParameters
        {
            get
            {
                return preprocessingParameters;
            }

            set
            {
                preprocessingParameters = value;
            }
        }

        public TaskTemplate()
        {
            this.nameTable = "TaskTemplate";
        }

        public override Dictionary<string, Type> serializationParameters()
        {
            Dictionary<string, Type> serializationParameters = new Dictionary<string, Type>();
            serializationParameters.Add("PreprocessingParameters", typeof(IPreprocessingParameters));
            base.serializationParameters().ToList().ForEach(x => serializationParameters.Add(x.Key, x.Value));
            return serializationParameters;
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("Name", "Name");
            mappingTable.Add("TaskID", "TaskID");
            mappingTable.Add("PreprocessingParameters", "PreprocessingParameters");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }
    }
}
