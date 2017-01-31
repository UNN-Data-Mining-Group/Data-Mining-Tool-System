using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    public enum TypeParameter
    {
        Int,
        Real,
        Bool,
        Enum
    }

    public class Parameter : Entity
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

        private string comment;
        public string Comment
        {
            get
            {
                return comment;
            }

            set
            {
                comment = value;
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

        private int index;
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        private int isOutput;
        public int IsOutput
        {
            get
            {
                return isOutput;
            }

            set
            {
                isOutput = value;
            }
        }

        private TypeParameter type;
        public TypeParameter Type
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

        public Parameter()
        {
            this.nameTable = "Parameter";
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("Name", "Name");
            mappingTable.Add("Comment", "Comment");
            mappingTable.Add("Type", "Type");
            mappingTable.Add("Index", "Index");
            mappingTable.Add("IsOutput", "IsOutput");
            mappingTable.Add("TaskTemplateID", "TaskTemplateID");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }
    }
}
