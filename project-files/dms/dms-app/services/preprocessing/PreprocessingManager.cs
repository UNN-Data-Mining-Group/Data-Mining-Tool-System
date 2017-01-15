using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.view_models;

namespace dms.services.preprocessing
{
    class PreprocessingManager
    {
        private static PreprocessingManager manager;
        public static PreprocessingManager Manager
        {
            get
            {
                if (manager == null)
                {
                    manager = new PreprocessingManager();

                }
                return manager;
            }
        }

        public PreprocessingManager()
        {
        }
        public int addTask(string name, int parameterCount, int selectionCount)
        {
            DataHelper helper = new DataHelper();
            int taskId = helper.addTask(name, parameterCount, selectionCount);
            return taskId;
        }
        public void parse(string taskTemplateName, string filePath, char delimiter, int taskId, string selectionName, ParameterCreationViewModel[] parameters)
        {
            Parser.ParserObj.parse(taskTemplateName, filePath, delimiter, taskId, selectionName, parameters);
        }
        public string[] getTypes(string filePath, char delimiter)
        {
            return Parser.ParserObj.getParameterTypes(filePath, delimiter);
        }
    }
}
