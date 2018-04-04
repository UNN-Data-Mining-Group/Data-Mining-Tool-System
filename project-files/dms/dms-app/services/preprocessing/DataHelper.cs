using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;

namespace dms.services.preprocessing
{
    class DataHelper
    {
        public void updateTaskTemplate(int taskTemplateId, view_models.PreprocessingViewModel.PreprocessingTemplate pp)
        {
            TaskTemplate entity = (TaskTemplate)DatabaseManager.SharedManager.entityById(taskTemplateId, typeof(TaskTemplate));
            entity.PreprocessingParameters = pp;
            entity.save();
        }
        public void updateTask(int taskId, int paramCount, int selectionCount)
        {
            dms.models.Task entity = (dms.models.Task) DatabaseManager.SharedManager.entityById(taskId, typeof(dms.models.Task));
            entity.ParamCount = paramCount;
            entity.SelectionCount = selectionCount;
            entity.save();
        }
        public int addTaskTemplate(string name, int taskId, IPreprocessingParameters ppParameters)
        {
            TaskTemplate entity = new TaskTemplate();
            entity.Name = name;
            entity.TaskID = taskId;
            entity.PreprocessingParameters = ppParameters;
            entity.save();
            return entity.ID;
        }
        public int addSelection(string name, int taskTemplateId, int count, string type)
        {
            Selection entity = new Selection();
            entity.Name = name;
            entity.TaskTemplateID = taskTemplateId;
            entity.RowCount = count;
            entity.Type = type;
            entity.save();
            return entity.ID;
        }
        public SelectionRow addSelectionRow(int selectionId, int rowNumber)
        {
            SelectionRow entity = new SelectionRow();
            entity.SelectionID = selectionId;
            entity.Number = rowNumber;
            return entity;
        }
        public int addOneParameter(string name, string comment, int taskTemplateID, int index, int isOutput, TypeParameter type)
        {
            Parameter entity = new Parameter();
            entity.Name = name;
            entity.Comment = comment;
            entity.TaskTemplateID = taskTemplateID;
            entity.Index = index;
            entity.IsOutput = isOutput;
            entity.Type = type;
            entity.save();
            return entity.ID;
        }
        public Parameter addParameter(string name, string comment, int taskTemplateID, int index, int isOutput, TypeParameter type)
        {
            Parameter entity = new Parameter();
            entity.Name = name;
            entity.Comment = comment;
            entity.TaskTemplateID = taskTemplateID;
            entity.Index = index;
            entity.IsOutput = isOutput;
            entity.Type = type;
            return entity;
        }
        public ValueParameter addValueParameter(int selectionRowID, int parameterID, string value)
        {
            ValueParameter entity = new ValueParameter();
            entity.SelectionRowID = selectionRowID;
            entity.ParameterID = parameterID;
            entity.Value = value;
            return entity;
        }
        public void updateValueParameter(int valueParameterId, string value)
        {
            dms.models.ValueParameter entity = (dms.models.ValueParameter)DatabaseManager.SharedManager.entityById(valueParameterId, typeof(dms.models.ValueParameter));
            entity.Value = value;
            entity.save();
        }

        public void deleteSelectionRowsWithValues(List<Entity> selectionRows)
        {
            List<Entity> listForDelete = new List<Entity>();
            listForDelete = listForDelete.Concat(selectionRows).ToList();
            for (int i = 0; i < selectionRows.Count; i++)
            {
                List<Entity> values = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                        .addCondition("SelectionRowID", "=", selectionRows[i].ID.ToString()), typeof(ValueParameter));
                listForDelete = listForDelete.Concat(values).ToList();
            }
            DatabaseManager.SharedManager.deleteMultipleEntities(listForDelete);
        }

        public void deleteSelection(Entity selection)
        {
            List<Entity> listForDelete = new List<Entity>();

            int templateId = ((Selection)selection).TaskTemplateID;
            TaskTemplate template = ((TaskTemplate)services.DatabaseManager.SharedManager.entityById(templateId, typeof(TaskTemplate)));

            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", template.TaskID.ToString()), typeof(TaskTemplate));
            List<Entity> selections = new List<Entity>();
            foreach (Entity entity in taskTemplates)
            {
                List<Entity> sels = TaskTemplate.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateId", "=", entity.ID.ToString())
                .addCondition("Name", "=", ((Selection)selection).Name), typeof(Selection));
                if (sels.Count == 0)
                {
                    continue;
                }
                else
                {
                    listForDelete = listForDelete.Concat(sels).ToList();
                    selections = selections.Concat(sels).ToList();
                }
            }

            foreach(Entity sel in selections)
            {
                List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", sel.ID.ToString()), typeof(SelectionRow));
                listForDelete = listForDelete.Concat(selectionRows).ToList();
                for (int i = 0; i < selectionRows.Count; i++)
                {
                    List<Entity> values = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                            .addCondition("SelectionRowID", "=", selectionRows[i].ID.ToString()), typeof(ValueParameter));
                    listForDelete = listForDelete.Concat(values).ToList();
                }

                List<Entity> learnedSolverList = LearnedSolver.where(new Query("LearnedSolver").addTypeQuery(TypeQuery.select)
                            .addCondition("SelectionID", "=", sel.ID.ToString()), typeof(LearnedSolver));
                listForDelete = listForDelete.Concat(learnedSolverList).ToList();
                foreach(Entity lSolver in learnedSolverList)
                {
                    List<Entity> learningQualityList = LearningQuality.where(new Query("LearningQuality").addTypeQuery(TypeQuery.select)
                            .addCondition("LearnedSolverID", "=", lSolver.ID.ToString()), typeof(LearningQuality));
                    listForDelete = listForDelete.Concat(learningQualityList).ToList();
                }
            }

            DatabaseManager.SharedManager.deleteMultipleEntities(listForDelete);
        }

        public void deleteTask(Entity task)
        {
            List<Entity> listForDelete = new List<Entity>();
            listForDelete.Add(task);

            List<Entity> templates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", task.ID.ToString()), typeof(TaskTemplate));
            listForDelete = listForDelete.Concat(templates).ToList();

            foreach (Entity template in templates)
            {
                List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", template.ID.ToString()), typeof(models.Parameter));
                listForDelete = listForDelete.Concat(parameters).ToList();

                List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", template.ID.ToString()), typeof(Selection));
                listForDelete = listForDelete.Concat(selections).ToList();

                foreach(Entity selection in selections)
                {
                    List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                        .addCondition("SelectionID", "=", selection.ID.ToString()), typeof(SelectionRow));
                    listForDelete = listForDelete.Concat(selectionRows).ToList();

                    for (int i = 0; i < selectionRows.Count; i++)
                    {
                        List<Entity> values = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                                .addCondition("SelectionRowID", "=", selectionRows[i].ID.ToString()), typeof(ValueParameter));
                        listForDelete = listForDelete.Concat(values).ToList();
                    }

                    List<Entity> learnedSolverList = LearnedSolver.where(new Query("LearnedSolver").addTypeQuery(TypeQuery.select)
                            .addCondition("SelectionID", "=", selection.ID.ToString()), typeof(LearnedSolver));
                    listForDelete = listForDelete.Concat(learnedSolverList).ToList();
                    foreach (Entity lSolver in learnedSolverList)
                    {
                        List<Entity> learningQualityList = LearningQuality.where(new Query("LearningQuality").addTypeQuery(TypeQuery.select)
                                .addCondition("LearnedSolverID", "=", lSolver.ID.ToString()), typeof(LearningQuality));
                        listForDelete = listForDelete.Concat(learningQualityList).ToList();
                    }
                }
            }

            DatabaseManager.SharedManager.deleteMultipleEntities(listForDelete);
        }
    }
}
