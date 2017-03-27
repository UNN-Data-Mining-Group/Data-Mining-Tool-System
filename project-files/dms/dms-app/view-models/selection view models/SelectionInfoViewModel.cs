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
        public class Pair
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
        private string selectedPreprocessing;

        private string[][] originalData;
        private string[] originalColumns;

        private int SelectionId { get; set; }

        public SelectionInfoViewModel(int taskId, int selectionId)
        {
            Selection selection = ((Selection)services.DatabaseManager.SharedManager.entityById(selectionId, typeof(Selection)));
            SelectionId = selectionId;
            SelectionName = selection.Name;
            CountRows = selection.RowCount;
            TaskId = taskId;
            TaskName = ((models.Task)services.DatabaseManager.SharedManager.entityById(taskId, typeof(models.Task))).Name;

            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", taskId.ToString()), typeof(TaskTemplate));
            int size = taskTemplates.Count;
            PreprocessingIdList = new int[size];
            PreprocessingList = new string[size];
            int index = 0;
            foreach (Entity entity in taskTemplates)
            {
                List<Entity> sels = TaskTemplate.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateId", "=", entity.ID.ToString())
                .addCondition("Name", "=", SelectionName), typeof(Selection));
                if (sels.Count == 0)
                {
                    continue;
                }
                PreprocessingList[index] = ((TaskTemplate)entity).Name;
                PreprocessingIdList[index] = entity.ID;
                index++;
            }
            string[] lst = new string[index];
            int[] lstId = new int[index];
            for (int i = 0; i < index; i++)
            {
                lst[i] = PreprocessingList[i];
                lstId[i] = PreprocessingIdList[i];
            }
            PreprocessingIdList = lstId;
            PreprocessingList = lst;

            updateTable(selection.TaskTemplateID);
            TaskTemplate taskTemplate = ((TaskTemplate)services.DatabaseManager.SharedManager.entityById(selection.TaskTemplateID, typeof(TaskTemplate)));

            SelectedPreprocessing = taskTemplate.Name;
            
        }
        
        public string TaskName { get; }
        public int TaskId { get; }
        public string SelectionName { get; }
        public int CountRows { get; }
        public string[] PreprocessingList { get; }
        public int[] PreprocessingIdList { get; }
        public string SelectedPreprocessing
        {
            get { return selectedPreprocessing; }
            set
            {
                List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", TaskId.ToString())
                .addCondition("Name", "=", value), typeof(TaskTemplate));
                int taskTemplateId = taskTemplates[0].ID;
                selectedPreprocessing = value;
                updateTable(taskTemplateId);
                for (int i = 0; i < PreprocessingList.Length; i++)
                {
                    if (selectedPreprocessing.Equals(PreprocessingList[i]))
                    {
                        Data = originalData;
                        DataColumns = originalColumns;
                        taskTemplateId = PreprocessingIdList[i];
                        break;
                    }
                }
                NotifyPropertyChanged("Data");
                NotifyPropertyChanged("DataColumns");
            }
        }
        public string[][] Data { get; private set; }
        public string[] DataColumns { get; private set; }

        private void updateTable(int taskTemplateId)
        {
            //рисуем заголовки
            List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(models.Parameter));
            originalColumns = new string[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                models.Parameter parameter = (models.Parameter)parameters[i];
                originalColumns[i] = parameter.Name;
            }
            //рисуем содержимое
            originalData = new string[CountRows][];
            for (int i = 0; i < CountRows; i++)
            {
                originalData[i] = new string[parameters.Count];
            }
            List<Entity> sels = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString())
                .addCondition("Name", "=", SelectionName), typeof(Selection));
            if (sels.Count != 0)
            {
                List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", sels[0].ID.ToString()), typeof(SelectionRow));

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
}
