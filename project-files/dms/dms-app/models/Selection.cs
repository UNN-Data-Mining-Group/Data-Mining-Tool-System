using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    public class Selection : Entity
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

        private int taskTemplateID;
        public int TaskTemplateID
        {
            get
            {
                return taskTemplateID;
            }

            set
            {
                taskTemplateID = value;
            }
        }

        private int rowCount;
        public int RowCount
        {
            get
            {
                return rowCount;
            }

            set
            {
                rowCount = value;
            }
        }

        private string type;
        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public Selection()
        {
            this.nameTable = "Selection";
        }

        public Selection(archive.ArchiveSelection selection)
        {
            this.nameTable = "Selection";
            this.Name = selection.Name;
            this.RowCount = selection.RowCount;
            this.Type = selection.Type;
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("Name", "Name");
            mappingTable.Add("RowCount", "RowCount");
            mappingTable.Add("Type", "Type");
            mappingTable.Add("TaskTemplateID", "TaskTemplateID");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }

        public static List<Selection> selectionsOfDefaultTemplateWithTaskId(int taskId)
        {
            TaskTemplate defaultTemplate = null;
            List<Entity> templates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", taskId.ToString()), typeof(TaskTemplate));
            if (templates.Count == 0)
            {
                return new List<Selection>();
            }
            int i = 0;
            List<Selection> selections = new List<Selection>();
            foreach (models.Entity template in templates)
            {
                List<Selection> temp = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", template.ID.ToString()), typeof(Selection)).Cast<Selection>().ToList();
                selections = selections.Concat(temp).ToList();
            }
            return selections;
        }

        public static List<Selection> selectionsOfTaskTemplateId(int taskTemplateId)
        {
            return Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(Selection)).Cast<Selection>().ToList();
        }

        public static string[][] valuesOfSelectionId(int selectionId)
        {
            Selection selection = (Selection)Selection.getById(selectionId, typeof(Selection));
            int templateId = selection.TaskTemplateID;
            Parameter[] parameters = Parameter.parametersOfTaskTemplateId(templateId).ToArray();
            parameters = parameters.OrderBy(c => c.Index).ToArray();
            Query query = new Query("SelectionRow").addTypeQuery(TypeQuery.select).addCondition("SelectionID", "=", selectionId.ToString());
            List<Entity> rows = SelectionRow.where(query, typeof(SelectionRow));
            Query valQuery = new Query("ValueParameter").addTypeQuery(TypeQuery.select).addInArray("SelectionRowID", rows.Select(x => x.ID).ToArray());
            ValueParameter[] values = ValueParameter.where(valQuery, typeof(ValueParameter)).Cast<ValueParameter>().ToArray();
            int[] ids = rows.Select(x => x.ID).ToArray();
            string[][] res = new string[ids.Length][];
            var index = 0;
            foreach (int id in ids)
            {
                ValueParameter[] vals = values.Where(x => x.SelectionRowID == id).ToArray();
                res[index] = new string[parameters.Length];
                int j = 0;
                foreach (Parameter par in parameters)
                {
                    ValueParameter[] find = vals.Where(x => x.ParameterID == par.ID).ToArray();
                    if (find.Length > 0)
                    {
                        string value = find[0].Value;
                        res[index][j] = value;
                    }
                    else
                    {
                        res[index][j] = "";
                    }
                    
                    
                    j++;
                }
                index++;
            }
            
            return res;
        }

        public static Entity[] valueParametersOfColumn(int selectionId, int paramId)
        {
            Selection selection = (Selection)Selection.getById(selectionId, typeof(Selection));
            int templateId = selection.TaskTemplateID;
            Query query = new Query("SelectionRow").addTypeQuery(TypeQuery.select).addCondition("SelectionID", "=", selectionId.ToString());
            List<Entity> rows = SelectionRow.where(query, typeof(SelectionRow));
            Query valQuery = new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                .addInArray("SelectionRowID", rows.Select(x => x.ID).ToArray())
                .addCondition("ParameterID", "=", paramId.ToString());
            ValueParameter[] values = ValueParameter.where(valQuery, typeof(ValueParameter)).Cast<ValueParameter>().ToArray();

            return values;
        }

        public static string[] valuesOfColumnParameters(int selectionId, int paramId)
        {
            Selection selection = (Selection)Selection.getById(selectionId, typeof(Selection));
            int templateId = selection.TaskTemplateID;
            Query query = new Query("SelectionRow").addTypeQuery(TypeQuery.select).addCondition("SelectionID", "=", selectionId.ToString());
            List<Entity> rows = SelectionRow.where(query, typeof(SelectionRow));
            Query valQuery = new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                .addInArray("SelectionRowID", rows.Select(x => x.ID).ToArray())
                .addCondition("ParameterID", "=", paramId.ToString());
            ValueParameter[] values = ValueParameter.where(valQuery, typeof(ValueParameter)).Cast<ValueParameter>().ToArray();
            int[] ids = rows.Select(x => x.ID).ToArray();
            string[] res = new string[ids.Length];
            var index = 0;
            foreach (int id in ids)
            {
                ValueParameter[] vals = values.Where(x => x.SelectionRowID == id).ToArray();
                res[index] = "";
                if (vals.Length > 0)
                {
                    res[index] = vals[0].Value;
                }
                index++;
            }

            return res;
        }
    }
}
