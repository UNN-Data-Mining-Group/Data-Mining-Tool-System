using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models
{
    public enum TypeQuery
    {
        none = -1,
        select,
        insert,
        update,
        delete
    }

    public class Query
    {
        private TypeQuery typeQuery = TypeQuery.none;
        private string nameTable = "";
        private string conditionString = "";
        /*
         * changedValues используется для обновления и сохранения элементов в БД
         * key - имя аттрибута сущности, value - измененное значение
         */
        private Dictionary<string, string> changedValues;

        public Query(string nameTable)
        {
            this.nameTable = nameTable;
        }

        public Query addTypeQuery(TypeQuery type)
        {
            typeQuery = type;
            return this;
        }

        public Query addCondition(string key, string op, string value)
        {
            if (conditionString == "")
            {
                conditionString = key + op + "'" + value + "'";
            }
            else
            {
                conditionString += " AND " + key + op + "'" + value + "'";
            }
            return this;
        }

        public Query addInArray(string key, Array array)
        {
            if (conditionString == "")
            {
                conditionString = key + " IN (" + SQLArrayToInString(array) + ")";
            }
            else
            {
                conditionString += " AND " + key +  "IN (" + SQLArrayToInString(array) + ")";
            }
            return this;
        }

        public Query addTable(string nameTable)
        {
            this.nameTable = nameTable;
            return this;
        }

        public Query setChangedValues(Dictionary<string, string>changedValues)
        {
            this.changedValues = changedValues;
            return this;
        }

        public string StatementForDatabase()
        {
            if (typeQuery == TypeQuery.none)
            {
                throw new System.ArgumentException("Не задан TypeQuery у запроса", "original");
            }
            if (nameTable == "")
            {
                throw new System.ArgumentException("Не задано имя таблицы, запрос не может быть сделан", "original");
            }
            string statementForDatabase = "";
            switch (typeQuery) {
                case TypeQuery.select:
                    statementForDatabase = "SELECT * FROM " + nameTable;
                    break;
                case TypeQuery.delete:
                    statementForDatabase = "DELETE FROM " + nameTable;
                    break;
                case TypeQuery.update:
                    statementForDatabase = "UPDATE " + nameTable;
                    break;
                case TypeQuery.insert:
                    statementForDatabase = "INSERT INTO " + nameTable;
                    break;
            }
            if (typeQuery == TypeQuery.insert)
            {
                string keys = "(";
                string values = "(";
                for (int index = 0; index < changedValues.Count; index++)
                {
                    var entry = changedValues.ElementAt(index);
                    keys += "[" + entry.Key + "]";
                    if (entry.Value.Contains("@data"))
                    {
                        values += entry.Value;
                    }
                    else
                    {
                        values += "'" + entry.Value + "'";
                    }                    
                    if (index == changedValues.Count - 1)
                    {
                        keys += ")";
                        values += ")";
                    }
                    else
                    {
                        keys += ",";
                        values += ",";
                    }
                }
                statementForDatabase += " " + keys + " VALUES " + values;
            }
            if (typeQuery == TypeQuery.update)
            {
                string updateString = "";
                for (int index = 0; index < changedValues.Count; index++)
                {
                    var entry = changedValues.ElementAt(index);
                    if (entry.Value.Contains("@data"))
                    {
                        updateString += "[" + entry.Key + "]" + "=" + entry.Value;
                    }
                    else
                    {
                        updateString += "[" + entry.Key + "]" + "='" + entry.Value + "'";
                    }
                    if (index != changedValues.Count - 1)
                    {
                        updateString += ",";
                    }
                }
                statementForDatabase += " SET " + updateString;
            }
            if (conditionString != "" && typeQuery != TypeQuery.insert)
            {
                statementForDatabase += " WHERE " + conditionString;
            }
            return statementForDatabase;
        }

        private string SQLArrayToInString(Array a)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
                sb.AppendFormat("'{0}',", a.GetValue(i));
            string retVal = sb.ToString();
            return retVal.Substring(0, retVal.Length - 1);
        }
    }
}
