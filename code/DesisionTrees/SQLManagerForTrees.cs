using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using SII;

namespace DesisionTrees
{
    public class SQLManagerForTrees: SQLManager
    {
        private const string PATH_DB = "../../../SII.db";

        private SQLiteConnection conn;
        private SQLiteTransaction trans;

        private static DesisionTrees.SQLManagerForTrees mainSQLManager;

        public SQLManagerForTrees()
        {
            conn = new SQLiteConnection("Data Source=" + DesisionTrees.SQLManagerForTrees.PATH_DB + "; Version=3;");
            conn.Open();
        }

        public static DesisionTrees.SQLManagerForTrees MainSQLManager
        {
            get
            {
                if (mainSQLManager == null)
                {
                    mainSQLManager = new SQLManagerForTrees();
                }
                return mainSQLManager;
            }
        }

        public int GetMaxFeature(String req, String feature)  //gets max id or row count from req table
        {
            int res = 0;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = 0;
                while (r.Read())
                {
                    res = int.Parse(string.Format("{0}", r[feature]));
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            return res;
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

        public Selection GetOneSelectionWithRequest(String req)
        {
            Selection res;// = null;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new Selection();
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
                    res = selection;
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

        public List<Tree> GetTreeWithRequest(String req)
        {
            List<Tree> res = null;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new List<Tree>();
                Tree tree;
                while (r.Read())
                {
                    tree = new Tree();
                    tree.ID = int.Parse(string.Format("{0}", r["ID"]));
                    tree.TASK_ID = int.Parse(string.Format("{0}", r["TASK_ID"]));
                    tree.SELECTION_ID = int.Parse(string.Format("{0}", r["SELECTION_ID"]));
                    tree.ROOT_ID = int.Parse(string.Format("{0}", r["ROOT_ID"]));
                    res.Add(tree);
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


        public Parametr GetOneParametrWithRequest(String req)
        {
            Parametr res = null;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new Parametr();
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
                    res = param;
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

        public Rule GetRuleWithRequest(String req)
        {
            Rule res = null;
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                res = new Rule();              
                while (r.Read())
                {
                    
                    res.index_of_param = int.Parse(string.Format("{0}", r["PARAM_INDEX"]));                    
                    res.value = r["VALUE"].ToString();
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


        public TreeNode GetNodeWithRequest(int node_ID)
        {
            String req = "SELECT * FROM NODE WHERE ID = '" + node_ID +"'"; 
            TreeNode res = new TreeNode();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            int rule_id = 0;
            int left_child_id=0;
            int right_child_id=0;
            int is_leaf=0;
            cmd.Transaction = trans;
            cmd.CommandText = req;
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                //res = new TreeNode();
                //TreeNode node;
                while (r.Read())
                {
                    //node = new TreeNode();
                    //node.ID = int.Parse(string.Format("{0}", r["ID"]));
                    left_child_id = int.Parse(string.Format("{0}", r["LEFT_CHILD_ID"]));
                    right_child_id = int.Parse(string.Format("{0}", r["RIGHT_CHILD_ID"]));
                    rule_id = int.Parse(string.Format("{0}", r["RULE_ID"]));
                    is_leaf = int.Parse(string.Format("{0}", r["IS_LEAF"]));                 
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            res.rule = GetRuleWithRequest("SELECT * FROM RULE WHERE ID ='"+ rule_id +"'");

            if (is_leaf == 0)//если тек узел не лист
            {
                res.is_leaf = false;
                res.left_child = GetNodeWithRequest(left_child_id);
                res.right_child = GetNodeWithRequest(right_child_id);
            }
            else 
            {
                res.is_leaf = true;
                res.left_child = null;
                res.right_child = null;
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
