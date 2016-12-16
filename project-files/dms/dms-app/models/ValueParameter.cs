using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    class ValueParameter : Entity
    {
        private int selectionRowID;
        public int SelectionRowID
        {
            get
            {
                return selectionRowID;
            }

            set
            {
                selectionRowID = value;
            }
        }

        private int parameterID;
        public int ParameterID
        {
            get
            {
                return parameterID;
            }

            set
            {
                parameterID = value;
            }
        }

        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }

        public ValueParameter()
        {
            this.nameTable = "ValueParameter";
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("ParameterID", "ParameterID");
            mappingTable.Add("SelectionRowID", "SelectionRowID");
            mappingTable.Add("Value", "Value");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }
    }
}
