using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    class SelectionRow : Entity
    {
        private int selectionID;
        public int SelectionID
        {
            get
            {
                return selectionID;
            }

            set
            {
                selectionID = value;
            }
        }

        private int number;
        public int Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }

        public SelectionRow()
        {
            this.nameTable = "SelectionRow";
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("SelectionID", "SelectionID");
            mappingTable.Add("Number", "Number");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }
    }
}
