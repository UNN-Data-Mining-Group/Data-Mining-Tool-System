using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SII
{
    public class SQLManager
    {
        private const string PATH_DB = "../../../SII.db";

        private SQLiteConnection conn;
        private SQLiteTransaction trans;

        private static SQLManager mainSQLManager;

        public SQLManager()
        {
            
            conn = new SQLiteConnection("Data Source=" + SQLManager.PATH_DB + "; Version=3;");
            conn.Open();
        }

        public static SQLManager MainSQLManager
        {
            get
            {
                if (mainSQLManager == null)
                {
                    mainSQLManager = new SQLManager();
                }
                return mainSQLManager;
            }
        }

        public List<Task> GetTasksWithRequest(String req)
        {
            List<Task> res = null;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new List<Task>();
                Task task;
                while (r.Read())
                {
                    task = new Task();
                    task.ID = int.Parse(string.Format("{0}", r["ID"]));
                    task.CountParameters = int.Parse(string.Format("{0}", r["PARAM_COUNT"]));
                    task.CountSelections = int.Parse(string.Format("{0}", r["SELECTION_COUNT"]));
                    task.Name = r["NAME"].ToString();
                    res.Add(task);
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

        public List<ValueParametr> GetValuesWithRequest(String req)
        {
            List<ValueParametr> res = null;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new List<ValueParametr>();
                ValueParametr valueParametr;
                while (r.Read())
                {
                    valueParametr = new ValueParametr();
                    valueParametr.ID = int.Parse(string.Format("{0}", r["ID"]));
                    valueParametr.ParametrID = int.Parse(string.Format("{0}", r["PARAM_ID"]));
                    valueParametr.SelectionID = int.Parse(string.Format("{0}", r["SELECTION_ID"]));
                    valueParametr.RowIndex = int.Parse(string.Format("{0}", r["ROW_INDEX"]));
                    valueParametr.Value = r["VALUE"].ToString();
                    res.Add(valueParametr);
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

        public List<Selection> GetSelectionsWithRequest(String req)
        {
            List<Selection> res = null;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new List<Selection>();
                Selection selection;
                while (r.Read())
                {
                    selection = new Selection();
                    selection.ID = int.Parse(string.Format("{0}", r["ID"]));
                    selection.TaskID = int.Parse(string.Format("{0}", r["TASK_ID"]));
                    selection.Name = r["NAME"].ToString();
                    selection.CountRows = int.Parse(string.Format("{0}", r["COUNT_ROWS"]));
                    int withResInt = int.Parse(string.Format("{0}", r["WITH_RESULT"]));
                    if (withResInt == 0)
                        selection.WithRes = false;
                    else
                        selection.WithRes = true;
                    res.Add(selection);
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

        public List<Parametr> GetParamsWithRequest(String req)
        {
            List<Parametr> res = null;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new List<Parametr>();
                Parametr param;
                while (r.Read())
                {
                    param = new Parametr();
                    param.ID = int.Parse(string.Format("{0}", r["ID"]));
                    param.Number = int.Parse(string.Format("{0}", r["NUMBER"]));
                    param.Range = r["RANGE"].ToString();
                    param.Type = (TypeParametr)Enum.Parse(typeof(TypeParametr), string.Format("{0}", r["TYPE"]));
                    param.TaskID = int.Parse(string.Format("{0}", r["TASK_ID"]));
                    param.Name = r["NAME"].ToString();
                    res.Add(param);
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

        public void StartTransaction()
        {
            trans = null;
            trans = conn.BeginTransaction(); 
        }

        public int EndTransaction(bool success) //-1 error, 1 - success
        {
            if (trans == null)
                return -1;
            if (success)
            {
                trans.Commit();
            }
            else
            {
                trans.Rollback();
            }
            trans = null;
            return 1;
        }

        public int SendInsertRequest(String req)
        {
            //conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = req;
            cmd.Transaction = trans;
            int key = 1;
            try
            {
                cmd.ExecuteNonQuery();
                string sql = @"select last_insert_rowid()";
                cmd.CommandText = sql;
                key = (int)(long)cmd.ExecuteScalar();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                trans.Rollback();
                trans = null;
                return 0;
            }            
            //conn.Close();
            return key;
        }

        public void SendUpdateRequest(String req)
        {
            //conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = req;
            cmd.Transaction = trans;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                trans.Rollback();
                trans = null;
            }
        }

        public void SendDeleteRequest(String req)
        {
            //conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = req;
            cmd.Transaction = trans;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                trans.Rollback();
                trans = null;
            }
        }
    }
}
