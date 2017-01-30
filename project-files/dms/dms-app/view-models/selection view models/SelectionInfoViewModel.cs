using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;

namespace dms.view_models
{
    public class SelectionInfoViewModel : ViewmodelBase
    {
        private string selectedPreprocessing;

        private string[][] originalData;
        private string[] originalColumns;

        private string[][] preprocData;
        private string[] preprocColumns;
        
        private int SelectionId { get; set; }

        public SelectionInfoViewModel(int taskId, int selectionId)
        {
            SelectionId = selectionId;
            TaskName = ((dms.models.Task)dms.services.DatabaseManager.SharedManager.entityById(taskId, typeof(dms.models.Task))).Name;
            Selection selection = ((Selection)dms.services.DatabaseManager.SharedManager.entityById(SelectionId, typeof(Selection)));
            SelectionName = selection.Name;
            CountRows = selection.RowCount;

            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", taskId.ToString()), typeof(TaskTemplate));
            PreprocessingListId = new int[taskTemplates.Count];
            PreprocessingList = new string[taskTemplates.Count];
            int index = 0;
            foreach (Entity entity in taskTemplates)
            {
                PreprocessingListId[index] = entity.ID;
                PreprocessingList[index] = ((TaskTemplate)entity).Name;
            }
            SelectedPreprocessing = "Без преобразования";
        }

        public string TaskName { get; }
        public string SelectionName { get; }
        public int CountRows { get; }
        public int[] PreprocessingListId { get; set; }
        public string[] PreprocessingList { get; }
        public string SelectedPreprocessing
        {
            get { return selectedPreprocessing; }
            set
            {
                int taskTemplateId = -1;//???
                selectedPreprocessing = value;
                for(int i = 0; i < PreprocessingList.Length; i++)
                {
                    if (selectedPreprocessing.Equals(PreprocessingList[i]))
                    {
                        Data = originalData;
                        DataColumns = originalColumns;
                        taskTemplateId = PreprocessingListId[i];
                        break;
                    }
                }
                updateTable(taskTemplateId);
                NotifyPropertyChanged("Data");
                NotifyPropertyChanged("DataColumns");
            }
        }
        public string[][] Data { get; private set; }
        public string[] DataColumns { get; private set; }

        private void updateTable(int taskTemplateId)
        {
            //рисуем заголовки
            List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(Parameter));
            for (int i = 0; i < parameters.Count; i++)
            {
                dms.models.Parameter parameter = (dms.models.Parameter)parameters[i];
                originalColumns[i] = parameter.Name;
            }
            //рисуем содержимое
            Selection selection = ((Selection)dms.services.DatabaseManager.SharedManager.entityById(SelectionId, typeof(Selection)));
            originalData = new string[selection.RowCount][];
            for (int i = 0; i < selection.RowCount; i++)
            {
                originalData[i] = new string[parameters.Count];
            }
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", SelectionId.ToString()), typeof(SelectionRow));

            int stepRow = 0;
            foreach (Entity selRow in selectionRows)
            {
                List<Entity> valueParam = new List<Entity>();
                int selectionRowId = selRow.ID;
                int stepParam = 0;
                foreach (Entity param in parameters)
                {
                    int paramId = param.ID;
                    List<Entity> value = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                .addCondition("ParameterID", "=", paramId.ToString()).
                addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));
                    originalData[stepRow][stepParam] = ((ValueParameter)value[0]).Value;
                    stepParam++;
                }
                stepRow++;
            }
        }
    }
}
