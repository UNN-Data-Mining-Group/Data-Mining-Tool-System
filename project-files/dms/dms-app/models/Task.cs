using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    public class Task : Entity
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

        private int paramCount;
        public int ParamCount
        {
            get
            {
                return paramCount;
            }

            set
            {
                paramCount = value;
            }
        }

        private int selectionCount = 0;
        public int SelectionCount
        {
            get
            {
                return selectionCount;
            }

            set
            {
                selectionCount = value;
            }
        }

        public Task()
        {
            this.nameTable = "Task";
        }

        public Task(archive.ArchiveTask task)
        {
            this.nameTable = "Task";
            this.Name = task.Name;
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("Name", "Name");
            mappingTable.Add("ParamCount", "ParamCount");
            mappingTable.Add("SelectionCount", "SelectionCount");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }
    }
}
