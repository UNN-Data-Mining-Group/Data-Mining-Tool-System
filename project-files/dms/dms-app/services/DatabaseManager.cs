using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SQLite;
using System.Dynamic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using dms.models;

namespace dms.services
{
    class DatabaseManager
    {
        private const string pathToDataBase = "dms";

        private static DatabaseManager sharedManager;

        public static DatabaseManager SharedManager
        {
            get
            {
                if (sharedManager == null)
                {
                    sharedManager = new DatabaseManager();
                }
                return sharedManager;
            }
        }

        public DatabaseManager()
        {
            connection = new SQLiteConnection("Data Source=" + pathToDataBase + "; Version=3;");
            connection.Open();
        }

        public void saveEntity(Entity entity)
        {
            Query query = new Query(entity.NameTable);
            List<object> binaryArray = new List<object>();
            Dictionary<string, string> changedValues = new Dictionary<string, string>();
            PropertyInfo []properties = entity.GetType().GetProperties();
            Dictionary<string, string> mappingTable = entity.mappingTable();
            Dictionary<string, Type> serializationParameters = entity.serializationParameters();
            foreach (PropertyInfo property in properties)
            {
                if (mappingTable.ContainsKey(property.Name) && property.Name != Entity.PrimaryKey())
                {
                    if (serializationParameters.ContainsKey(property.Name))//(property.GetValue(entity).GetType().IsArray && property.GetValue(entity).GetType().GetElementType() == typeof(byte))
                    {
                        if (property.GetValue(entity) != null)
                        {
                            BinaryFormatter serializer = new BinaryFormatter();
                            changedValues.Add(mappingTable[property.Name], "@data" + binaryArray.Count.ToString());
                            using (var ms = new MemoryStream())
                            {
                                serializer.Serialize(ms, property.GetValue(entity));
                                binaryArray.Add(ms.ToArray());
                            }
                        }                                                                      
                    }
                    else
                    {
                        if (property.GetValue(entity) != null)
                        {
                            if (!property.PropertyType.IsEnum)
                            {
                                changedValues.Add(mappingTable[property.Name], property.GetValue(entity).ToString());
                            }
                            else
                            {
                                changedValues.Add(mappingTable[property.Name], ((int)property.GetValue(entity)).ToString());
                            }                            
                        }                        
                    }                    
                }                
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
            int insertId = executeUpdateInsertQuery(query, binaryArray);
            if (entity.ID == -1)
            {
                entity.ID = insertId;
            }
        }

        public void insertMultipleEntities(List<Entity> list)
        {
            startTransaction();
            foreach (Entity entity in list)
            {
                saveEntity(entity);
            }
            endTransaction(true);
            int lastId = lastInsertId();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Entity entity = list[i];
                entity.ID = lastId;
                lastId--;
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

        public Entity entityById(int id, Type typeEntity)
        {
            Entity entity = (Entity)Activator.CreateInstance(typeEntity);
            Query query = new Query(entity.NameTable);
            query.addTypeQuery(TypeQuery.select)
                    .addCondition(Entity.PrimaryKey(), "=", id.ToString());
            List<Entity> list = executeSelectQuery(query, typeEntity);
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public List<Entity> entitiesByQuery(Query query, Type typeEntity)
        {
            return executeSelectQuery(query, typeEntity);
        }

        public List<Entity> allEntities(Type typeEntity)
        {
            Entity entity = (Entity)Activator.CreateInstance(typeEntity);
            Query query = new Query(entity.NameTable);
            query.addTypeQuery(TypeQuery.select);
            return executeSelectQuery(query, typeEntity);
        }

        private SQLiteConnection connection;
        private SQLiteTransaction transaction;
        private SQLiteCommand currentCmdInsert;

        private int executeUpdateInsertQuery(Query query, List<object>binaryObjects)
        {
            int insertId = 0;
            SQLiteCommand cmd = new SQLiteCommand(connection);
            cmd.CommandText = query.StatementForDatabase();
            cmd.Transaction = transaction;
            foreach (object obj in binaryObjects)
            {
                SQLiteParameter parameter = new SQLiteParameter("@data" + binaryObjects.IndexOf(obj).ToString(), System.Data.DbType.Binary);
                parameter.Value = obj;
                cmd.Parameters.Add(parameter);
            }            
            try
            {
                cmd.ExecuteNonQuery();
                if (transaction == null)
                {
                    string sql = @"select last_insert_rowid()";
                    cmd.CommandText = sql;
                    insertId = (int)(long)cmd.ExecuteScalar();
                }                
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            return insertId;
        }

        private void executeDeleteQuery(Query query)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            cmd.CommandText = query.StatementForDatabase();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<Entity> executeSelectQuery(Query query, Type typeEntity)
        {
            List<Entity> res = null;
            SQLiteCommand cmd = new SQLiteCommand(connection);
            cmd.CommandText = query.StatementForDatabase();
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new List<Entity>();
                Entity entity;
                while (r.Read())
                {
                    entity = (Entity)Activator.CreateInstance(typeEntity);
                    Dictionary<string, string> mappingTable = entity.mappingTable();
                    Dictionary<string, Type> serializationParameters = entity.serializationParameters();
                    PropertyInfo[] properties = entity.GetType().GetProperties();
                    foreach (var item in mappingTable)
                    {
                        List<PropertyInfo>list = properties.Where(o => o.Name == item.Key).ToList();
                        if (list.Count > 0)
                        {
                            PropertyInfo property = list[0];
                            if (serializationParameters.ContainsKey(item.Value))
                            {
                                if (!(r[item.Value] is DBNull))
                                {
                                    BinaryFormatter deserializer = new BinaryFormatter();
                                    byte[] byteArray = (byte[])r[item.Value];
                                    using (var ms = new MemoryStream(byteArray, 0, byteArray.Length))
                                    {
                                        ms.Write(byteArray, 0, byteArray.Length);
                                        ms.Position = 0;
                                        object obj = deserializer.Deserialize(ms);
                                        property.SetValue(entity, obj);
                                    }
                                }                                
                            }
                            else
                            {
                                if (!property.PropertyType.IsEnum)
                                {
                                    property.SetValue(entity, Convert.ChangeType(r[item.Value], property.PropertyType));
                                }
                                else
                                {
                                    property.SetValue(entity, Int32.Parse(r[item.Value].ToString()));
                                }
                                
                            }
                            
                        }                        
                    }
                    res.Add(entity);
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return res;
        }

        private int lastInsertId()
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            string sql = @"select last_insert_rowid()";
            cmd.CommandText = sql;
            int key = -1;
            try
            {
                key = (int)(long)cmd.ExecuteScalar();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            return key;
        }

        private void startTransaction()
        {
            transaction = null;
            transaction = connection.BeginTransaction();
        }

        private void endTransaction(bool commit)
        {
            if (transaction == null)
                return;

            if (commit)
            {
                transaction.Commit();
            }
            else
            {
                transaction.Rollback();
            }
            transaction = null;
        }
    }
}
