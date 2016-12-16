using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using dms.services;

namespace dms.models
{
    class Entity
    {
        private int id = -1;
        public int ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        protected string nameTable;
        public string NameTable
        {
            get
            {
                return nameTable;
            }
        }

        public static string PrimaryKey()
        {
            return "ID";
        }

        virtual public Dictionary<string, string> mappingTable()
        {
            /*
             * Возвращает таблицу соответствий свойств класса, и аттрибутов сущности в БД
             * key соответствует названию свойства класса, value - аттрибут сущности
             */
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("ID", "ID");
            return mappingTable;
        }

        virtual public Dictionary<string, Type> serializationParameters()
        {
            /*
             * Возвращает таблицу соответствий аттрибутов БД, и классов, к которым они должны быть сериализованы
             * key соответствует аттрибуту сущности, value - тип класса
             */
            return new Dictionary<string, Type>();
        }

        public void save()
        {
            DatabaseManager.SharedManager.saveEntity(this);
        }

        public void delete()
        {
            DatabaseManager.SharedManager.deleteEntity(this);
        }

        public static Entity getById(int id, Type typeEntity)
        {
            return DatabaseManager.SharedManager.entityById(id, typeEntity);
        }

        public static List<Entity> all(Type typeEntity)
        {
            return DatabaseManager.SharedManager.allEntities(typeEntity); ;
        }

        public static List<Entity> where(Query query, Type typeEntity)
        {
            return DatabaseManager.SharedManager.entitiesByQuery(query, typeEntity);
        }
    }
    
}
