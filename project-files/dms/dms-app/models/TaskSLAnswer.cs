using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    class TaskSLAnswer : Entity
    {
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

        private int learnedSolverID;
        public int LearnedSolverID
        {
            get
            {
                return learnedSolverID;
            }

            set
            {
                learnedSolverID = value;
            }
        }

        private string value;
        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public TaskSLAnswer()
        {
            this.nameTable = "TaskSLAnswer";
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("Value", "Value");
            mappingTable.Add("LearnedSolverID", "LearnedSolverID");
            mappingTable.Add("SelectionRowID", "SelectionRowID");
            mappingTable.Add("ParameterID", "ParameterID");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }
    }
}
