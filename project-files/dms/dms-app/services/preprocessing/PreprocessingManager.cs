using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.view_models;

namespace dms.services.preprocessing
{
    class PreprocessingManager
    {
        private static PreprocessingManager prepManager;
        public static PreprocessingManager PrepManager
        {
            get
            {
                if (prepManager == null)
                {
                    prepManager = new PreprocessingManager();

                }
                return prepManager;
            }
        }

        public PreprocessingManager()
        {
        }
  /*      public int TaskId { get; set; }
        public void setTaskId(int id)
        {
            TaskId = id;
        }
        //нужно ли это теперь здесь ?
        public int addTask(string name, int parameterCount, int selectionCount)
        {
            DataHelper helper = new DataHelper();
            int taskId = helper.addTask(name, parameterCount, selectionCount);
            return taskId;
        }*/
        public void updateTask(int taskId, int parameterCount, int selectionCount)
        {
            DataHelper helper = new DataHelper();
            helper.updateTask(taskId, parameterCount, selectionCount);
        }

        public string[] getParameterTypes(string filePath, char delimiter)
        {
            return Parser.SelectionParser.getParameterTypes(filePath, delimiter);
        }

        public void parseSelection(string taskTemplateName, string filePath, char delimiter, int taskId, string selectionName, ParameterCreationViewModel[] parameters)
        {
            Parser.SelectionParser.parse(taskTemplateName, filePath, delimiter, taskId, selectionName, parameters);
        }

        public int getCountRows()
        {
            return Parser.SelectionParser.CountRows;
        }

        public int getCountParameters()
        {
            return Parser.SelectionParser.CountParameters;
        }

        public int getTaskId(string taskName)
        {
            Query query = new models.Query("Task");
            query.addTypeQuery(TypeQuery.select);
            query.addCondition("Name", "=", "\"" +taskName + "\"");
            List<Entity> entities = DatabaseManager.SharedManager.entitiesByQuery(query, typeof(dms.models.Task));
            int taskId = entities[0].ID;
            return taskId;
        }
    }
}
