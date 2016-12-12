using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using dms.models;

namespace dms.services
{
    class DatabaseManager
    {
        public static DatabaseManager sharedManager = new DatabaseManager();

        public void saveEntity(Entity entity)
        {
            Query query = new Query(entity.NameTable);
            Dictionary<string, string> changedValues = new Dictionary<string, string>();
            PropertyInfo []properties = entity.GetType().GetProperties();
            Dictionary<string, string>mappingTable = entity.mappingTable();
            foreach (PropertyInfo property in properties)
            {
                changedValues.Add(mappingTable[property.Name], property.GetValue(entity).ToString());
            }
            if (entity.ID == -1)
            {
                query.addTypeQuery(TypeQuery.insert);
            }
            else
            {
                query.addTypeQuery(TypeQuery.update)
                    .addCondition(Entity.PrimaryKey(), "=", entity.ID.ToString());
            }
            query.setChangedValues(changedValues);
            int insertId = executeUpdateInsertQuery(query);
            if (entity.ID == -1)
            {
                entity.ID = insertId;
            }
        }

        public void deleteEntity(Entity entity)
        {
            if (entity.ID == -1)
            {
                return;
            }
            Query query = new Query(entity.NameTable);
            query.addTypeQuery(TypeQuery.delete)
                    .addCondition(Entity.PrimaryKey(), "=", entity.ID.ToString());
            executeDeleteQuery(query);
        }

        public Entity entityById(string nameTable, int id)
        {
            return null;
        }

        public List<Entity> entitiesByQuery(Query query)
        {
            return null;
        }

        public List<Entity> allEntities(string nameTable)
        {
            return null;
        }

        // private sqlconnector
        // private sqlconnection

        private int executeUpdateInsertQuery(Query query)
        {
            int insertId = 0;
            return insertId;
        }

        private void executeDeleteQuery(Query query)
        {
            
        }
    }
}
