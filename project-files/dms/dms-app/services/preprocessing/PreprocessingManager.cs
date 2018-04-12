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

        public void updateTaskTemplate(int taskTemplateId, view_models.PreprocessingViewModel.PreprocessingTemplate pp)
        {
            DataHelper helper = new DataHelper();
            helper.updateTaskTemplate(taskTemplateId, pp);
        }

        public string[] getParametersTypes(string filePath, char delimiter, bool hasHeader, float enumPercent)
        {
            return Parser.SelectionParser.getParametersTypes(filePath, delimiter, hasHeader, enumPercent);
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

        public Dictionary<List<Entity>, IParameter> executePreprocessing(int newSelectionId, int oldSelectionId, 
            int oldParamId, string prepType, int parameterPosition, int newParamId, float left, float right, float a)
        {
            return Preprocessing.PreprocessingObj.executePreprocessing(newSelectionId, oldSelectionId, oldParamId, prepType, parameterPosition, newParamId, left, right, a);
        }

        public List<Entity> getNewParametersForBinarizationType(int oldSelectionId, int newTemplateId, int oldParamId)
        {
            models.Parameter oldParam = ((models.Parameter)services.DatabaseManager.SharedManager.entityById(oldParamId, typeof(models.Parameter)));
            List<string> classes = getClasses(oldSelectionId, newTemplateId, oldParamId);
            List<Entity> listNewParams = new List<Entity>(classes.Count);
            int index = 0;
            foreach (string cl in classes)
            {
                index++;
                models.Parameter parameter = new DataHelper().addParameter(cl, oldParam.Comment, newTemplateId, index, oldParam.IsOutput, TypeParameter.Int);
                listNewParams.Add(parameter);
            }
            DatabaseManager.SharedManager.insertMultipleEntities(listNewParams);

            List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", newTemplateId.ToString()), typeof(models.Parameter));
            List<Entity> lst = new List<Entity>();
            foreach (Entity en in parameters)
            {
                if (classes.Contains(((models.Parameter)en).Name))
                {
                    lst.Add(en);
                }
            }
            return lst;
        }
        public List<string> getClasses(int oldSelectionId, int newTemplateId, int oldParamId)
        {
            models.Parameter oldParam = ((models.Parameter)services.DatabaseManager.SharedManager.entityById(oldParamId, typeof(models.Parameter)));
            List<Entity> oldSelectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", oldSelectionId.ToString()), typeof(SelectionRow));

            List<string> oldValuesForOldParamId = new List<string>();
            foreach (Entity selRow in oldSelectionRows)
            {
                int selectionRowId = selRow.ID;
                List<Entity> value = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                        .addCondition("ParameterID", "=", oldParamId.ToString()).
                        addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));
                oldValuesForOldParamId.Add(((ValueParameter)value[0]).Value);
            }

            EnumeratedParameter p = new EnumeratedParameter(oldValuesForOldParamId);
            List<string> classes = p.getClasses();
            return classes;
        }
        public List<bool> compareExAndObValues(List<string> expectedValues, List<string> obtainedValues, int selectionId, int parameterId)
        {
            List<string> appropriateValues = InversePreprocessing.InversePreprocessingObj.getAppropriateValues(obtainedValues, selectionId, parameterId);
            return InversePreprocessing.InversePreprocessingObj.getComparisonResults(selectionId, parameterId, appropriateValues, expectedValues);
        }

        public List<string> getAppropriateValuesAfterInversePreprocessing(List<string> obtainedValues, int selectionId, int parameterId)
        {
            List<string> appropriateValues = InversePreprocessing.InversePreprocessingObj.getAppropriateValues(obtainedValues, selectionId, parameterId);
            return InversePreprocessing.InversePreprocessingObj.getAppropriateValuesAfterInversePreprocessing(selectionId, parameterId, appropriateValues);
        }

        public string getPreprocessingValue(int taskTemplateId, string operation, string value, int index)
        {
            return Preprocessing.PreprocessingObj.prepForSolver(taskTemplateId, operation, value, index);
        }
    }
}