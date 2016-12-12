using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private string nameTable;
        public string NameTable
        {
            get
            {
                return nameTable;
            }
            set
            {
                nameTable = value;
            }
        }

        public static string PrimaryKey()
        {
            return "ID";
        }

        public Dictionary<string, string> mappingTable()
        {
            /*
             * Возвращает таблицу соответствий свойств класса, и аттрибутов сущности в БД
             * key соответствует названию свойства класса, value - аттрибут сущности
             */
            Dictionary<string, string> mappingTable = new Dictionary<string, string>();
            mappingTable.Add("id", "ID");
            return mappingTable;
        }

        public Dictionary<string, string> serializationParameters()
        {
            return null;
        }

        public void save()
        {
            DatabaseManager.sharedManager.saveEntity(this);
        }

        public void delete()
        {
            DatabaseManager.sharedManager.deleteEntity(this);
        }

        public static Entity getById(int id)
        {
            return null;//DatabaseManager.sharedManager.entityById(this.NameTable, id);
        }

        public static List<Entity> all()
        {
            return null;
        }

        public static List<Entity> where(Query query)
        {
            return null;
        }
    }
    
}
