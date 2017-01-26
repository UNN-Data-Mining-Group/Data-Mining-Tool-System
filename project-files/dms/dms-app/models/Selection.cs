using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    class Selection : Entity
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
            defaultTemplate = (TaskTemplate)templates[0];
            return Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", defaultTemplate.ID.ToString()), typeof(Selection)).Cast<Selection>().ToList();
        }
    }
}
