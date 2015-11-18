using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;

namespace NeuroWnd
{
    public class SQLiteConnector
    {
        public SQLiteConnection connection;
        public string dbPath = "../../../SII.db";

        public void ConnectToDB()
        {
            connection = new SQLiteConnection("Data Source = " + dbPath + "; Version = 3;");
            try
            {
                connection.Open();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void DisconnectFromDB()
        {
            connection.Dispose();
        }
    }
}
