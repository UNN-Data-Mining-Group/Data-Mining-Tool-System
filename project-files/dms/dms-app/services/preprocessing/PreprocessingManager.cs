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
        private static PreprocessingManager prepManager;
        public static PreprocessingManager PrepManager
        {
            get
            {
                if (prepManager == null)
                {
                    prepManager = new PreprocessingManager();

                }
                return prepManager;
            }
        }

        public PreprocessingManager()
        {
        }
  
        public void updateTask(int taskId, int parameterCount, int selectionCount)
        {
            DataHelper helper = new DataHelper();
            helper.updateTask(taskId, parameterCount, selectionCount);
        }

        public string[] getParameterTypes(string filePath, char delimiter)
        {
            return Parser.SelectionParser.getParameterTypes(filePath, delimiter);
        }

        public void parseSelection(string taskTemplateName, string filePath, char delimiter, int taskId, string selectionName, ParameterCreationViewModel[] parameters)
        {
            Parser.SelectionParser.parse(taskTemplateName, filePath, delimiter, taskId, selectionName, parameters);
        }

        public int getCountRows()
        {
            return Parser.SelectionParser.CountRows;
        }

        public int getCountParameters()
        {
            return Parser.SelectionParser.CountParameters;
        }
    }
}
