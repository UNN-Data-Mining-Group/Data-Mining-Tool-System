using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.view_models;
using dms.services.preprocessing.normalization;

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

        public string[] getParametersTypes(string filePath, char delimiter, bool hasHeader)
        {
            return Parser.SelectionParser.getParametersTypes(filePath, delimiter, hasHeader);
        }

        public string[] getParametersName()
        {
            return Parser.SelectionParser.ParametersName;
        }

        public int parseSelection(int taskTemplateId, string filePath, char delimiter, int taskId, string selectionName, 
            ParameterCreationViewModel[] parameters, bool hasHeader, bool isUsingExitingTemplate)
        {
            return Parser.SelectionParser.parse(taskTemplateId, filePath, delimiter, taskId, selectionName, parameters, hasHeader, isUsingExitingTemplate);
        }

        public int getCountRows()
        {
            return Parser.SelectionParser.CountRows;
        }

        public int getCountParameters()
        {
            return Parser.SelectionParser.CountParameters;
        }

        public int addNewEntitiesForPreprocessing(string selectionName, int countRows, int taskId,
            int taskTemplateId)
        {
            return Preprocessing.PreprocessingObj.addNewEntitiesForPreprocessing(selectionName, countRows, taskTemplateId);
        }

        public void executePreprocessing(int taskTemplateId, int newSelectionId, int selectionId, 
            int paramId, string prepType, int paramCount, int parameterPosition, bool canAdd)
        {
            Preprocessing.PreprocessingObj.executePreprocessing(taskTemplateId, newSelectionId, selectionId, 
                paramId, prepType, paramCount, parameterPosition, canAdd);
        }
    }
}
