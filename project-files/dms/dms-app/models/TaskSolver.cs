using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.solvers;

namespace dms.models
{
    class TaskSolver : Entity
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

        private int taskID;
        public int TaskID
        {
            get
            {
                return taskID;
            }

            set
            {
                taskID = value;
            }
        }

        private ISolverDescription description;
        public ISolverDescription Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        private string typeName;
        public string TypeName
        {
            get
            {
                return typeName;
            }

            set
            {
                typeName = value;
            }
        }

        public TaskSolver()
        {
            this.nameTable = "TaskSolver";
        }

        public override Dictionary<string, string> mappingTable()
        {
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("Name", "Name");
            mappingTable.Add("Description", "Description");
            mappingTable.Add("TypeName", "TypeName");
            mappingTable.Add("TaskID", "TaskID");
            base.mappingTable().ToList().ForEach(x => mappingTable.Add(x.Key, x.Value));
            return mappingTable;
        }

        public override Dictionary<string, Type> serializationParameters()
        {
            Dictionary<string, Type> serializationParameters = new Dictionary<string, Type>();
            serializationParameters.Add("Description", typeof(ISolverDescription));
            base.serializationParameters().ToList().ForEach(x => serializationParameters.Add(x.Key, x.Value));
            return serializationParameters;
        }

        public static List<TaskSolver> solversOfTaskId(int taskId)
        {
            return TaskSolver.where(new Query("TaskSolver").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", taskId.ToString()), typeof(TaskSolver)).Cast<TaskSolver>().ToList();
        }
    }
}
