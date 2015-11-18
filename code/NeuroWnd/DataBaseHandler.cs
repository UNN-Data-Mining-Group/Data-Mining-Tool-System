using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;
using NeuroWnd.Neuro_Nets;
using NeuroWnd.Activate_functions;
using LearningAlgorithms;
using SII;

namespace NeuroWnd
{
    public class DataBaseHandler
    {
        private SQLiteConnector connector;

        public DataBaseHandler()
        {
            connector = new SQLiteConnector();
        }

        public double[,] SelectLearningSelection(String SelectionName)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID FROM SELECTION WHERE NAME='" + SelectionName + "'";
            int selID = Convert.ToInt32(cmd.ExecuteScalar());
            connector.DisconnectFromDB();

            SQLManager sqlManager = new SQLManager();
            List<ValueParametr> arrValues = sqlManager.GetValuesWithRequest(
                String.Format("select * from VALUE_PARAM where SELECTION_ID = '{0}';", selID));

            int prevIndex = arrValues[0].RowIndex;
            int countParams = 1;
            while (countParams < arrValues.Count)
            {
                if (prevIndex != arrValues[countParams].RowIndex)
                {
                    break;
                }
                countParams++;
            }

            double[,] sel = new double[arrValues.Count / countParams, countParams];

            int row = 0;
            int param = 0;
            prevIndex = arrValues[0].RowIndex;
            foreach (ValueParametr valueParam in arrValues)
            {
                if (valueParam.RowIndex != prevIndex)
                {
                    row++;
                    param = 0;
                }

                if (param == 0)
                {
                    sel[row, countParams - 1] = Convert.ToDouble(valueParam.Value);
                }
                else
                {
                    sel[row, param - 1] = Convert.ToDouble(valueParam.Value);
                }
                param++;
                prevIndex = valueParam.RowIndex;
            }
            return sel;
        }

        public List<Tuple<string, int>> SelectAllSelections(int TaskID)
        {
            List<Tuple<string, int>> ls = new List<Tuple<string, int>>();
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT NAME, COUNT_ROWS FROM SELECTION WHERE TASK_ID='" + Convert.ToString(TaskID) + "'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    string str = Convert.ToString(reader[0]);
                    int bf = Convert.ToInt32(reader[1]);
                    ls.Add(new Tuple<string, int>(str, bf));
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            reader.Close();
            connector.DisconnectFromDB();
            return ls;
        }

        public List<string> SelectSelectionsNames(string TaskName)
        {
            List<string> ls = new List<string>();
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT SELECTION.NAME FROM SELECTION, TASK WHERE TASK_ID=TASK.ID AND TASK.NAME = '" + Convert.ToString(TaskName) + "'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    string str = Convert.ToString(reader[0]);
                    ls.Add(str);
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            reader.Close();
            connector.DisconnectFromDB();
            return ls;
        }

        public List<string> SelectLearningStatistics(string NeuroNetName, string SelectionName)
        {
            List<string> ls = new List<string>(LearningAlgorithmsLibrary.GetAllNamesOfTypesOfAlgorithms());

            List<int> topologies = new List<int>();
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT NetTopology.ID FROM NetTopology, NeuroNet WHERE NeuroNetID = NeuroNet.ID AND NeuroNet.NAME = '" + Convert.ToString(NeuroNetName) + "'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    topologies.Add(Convert.ToInt32(reader[0]));
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            reader.Close();

            foreach(int item in topologies)
            {
                cmd.CommandText = "SELECT TypeLA FROM WeightsMatrix, SELECTION " +
                "WHERE SELECTIONID = SELECTION.ID AND SELECTION.NAME = '" + SelectionName
                + "' AND NetTopologyID = '" + Convert.ToString(item) + "'";

                List<string> bf = new List<string>();
                reader = cmd.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        bf.Add(Convert.ToString(reader[0]));
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                reader.Close();

                for (int i = 0; i < ls.Count; i++)
                {
                    bool isConsist = false;
                    foreach (string idBf in bf)
                    {
                        if (String.Compare(idBf, ls[i]) == 0)
                        {
                            isConsist = true;
                            break;
                        }
                    }

                    if (isConsist == false)
                    {
                        ls.Remove(ls[i]);
                        i--;
                    }
                }
            }

            connector.DisconnectFromDB();

            return ls;
        }

        public List<Tuple<int, string, int>> SelectAllTasks()
        {
            List<Tuple<int, string, int>> ls = new List<Tuple<int, string, int>>();
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID, NAME, SELECTION_COUNT FROM Task";
            SQLiteDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader[0]);
                    string str = Convert.ToString(reader[1]);
                    int bf = Convert.ToInt32(reader[2]);
                    ls.Add(new Tuple<int, string, int>(id, str, bf));
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            reader.Close();
            connector.DisconnectFromDB();
            return ls;
        }

        public void DeleteWeightsMatrix(string NeuroNetName, LoadingWindow loadingWindow)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID FROM NeuroNet WHERE Name = '" + NeuroNetName + "'";
            int NNID = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT ID FROM NetTopology WHERE NeuroNetID = '" + Convert.ToString(NNID) + "'";
            List<int> ls = new List<int>();

            double progress = 0.0;
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(Convert.ToInt32(reader[0]));
                }
                reader.Close();

                foreach (int item in ls)
                {
                    cmd.CommandText = "DELETE FROM WeightsMatrix WHERE NetTopologyID = '" + Convert.ToString(item) + "'";
                    cmd.ExecuteNonQuery();

                    progress += 100.0 / Convert.ToDouble(ls.Count);
                    Action f = new Action(() => loadingWindow.LoadingBar.Value = Convert.ToInt32(progress));
                    if (loadingWindow.LoadingBar.InvokeRequired)
                    {
                        loadingWindow.LoadingBar.Invoke(f);
                    }
                    else
                    {
                        f();
                    }
                }

            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            connector.DisconnectFromDB();
            loadingWindow.Invoke(new Action(() => loadingWindow.Close()));
        }

        public void DeleteWeightsMatrix(string NeuroNetName, string SelectionName, string NameOfLA, LoadingWindow loadingWindow)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID FROM NeuroNet WHERE Name = '" + NeuroNetName + "'";
            int NNID = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT ID FROM SELECTION WHERE NAME = '" + SelectionName + "'";
            int SelID = Convert.ToInt32(cmd.ExecuteScalar());

            string TypeLA = LearningAlgorithmsLibrary.GetNameOfTypeOfAlgoritm(NameOfLA);

            cmd.CommandText = "SELECT ID FROM NetTopology WHERE NeuroNetID = '" + Convert.ToString(NNID) + "'";
            List<int> ls = new List<int>();

            double progress = 0.0;
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(Convert.ToInt32(reader[0]));
                }
                reader.Close();

                foreach (int item in ls)
                {
                    cmd.CommandText = "DELETE FROM WeightsMatrix WHERE NetTopologyID = '" + Convert.ToString(item) + "' and " +
                        "TypeLA = '" + TypeLA + "' and SELECTIONID = " + SelID;
                    cmd.ExecuteNonQuery();

                    progress += 100.0 / Convert.ToDouble(ls.Count);
                    Action f = new Action(() => loadingWindow.LoadingBar.Value = Convert.ToInt32(progress));
                    if (loadingWindow.LoadingBar.InvokeRequired)
                    {
                        loadingWindow.LoadingBar.Invoke(f);
                    }
                    else
                    {
                        f();
                    }
                }

            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            connector.DisconnectFromDB();
            loadingWindow.Invoke(new Action(() => loadingWindow.Close()));
        }

        public void DeleteTopologyAndWeightsMatrix(string NeuroNetName, LoadingWindow loadingWindow)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID FROM NeuroNet WHERE NAME = '" + NeuroNetName + "'";
            int NNID = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT ID FROM NetTopology WHERE NeuroNetID = '" + Convert.ToString(NNID) + "'";
            List<int> ls = new List<int>();

            double progress = 0.0;
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(Convert.ToInt32(reader[0]));
                }
                reader.Close();

                foreach(int item in ls)
                {
                    cmd.CommandText = "DELETE FROM WeightsMatrix WHERE NetTopologyID = '" + Convert.ToString(item) + "'";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DELETE FROM NetTopology WHERE ID = '" + Convert.ToString(item) + "'";
                    cmd.ExecuteNonQuery();

                    progress += 100.0 / Convert.ToDouble(ls.Count);
                    Action f = new Action(() => loadingWindow.LoadingBar.Value = Convert.ToInt32(progress));
                    if (loadingWindow.LoadingBar.InvokeRequired)
                    {
                        loadingWindow.LoadingBar.Invoke(f);
                    }
                    else
                    {
                        f();
                    }
                }

            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            connector.DisconnectFromDB();
            loadingWindow.Invoke(new Action(() => loadingWindow.Close()));
        }

        public void UpdateNeuroNet(string NewNameNeuroNet, string OldNameNeuroNet,
            string TopologyTypeName, string TaskName, int CountNeurons,
            int CountLayers, string NeuronsInLayers, string typeAF, string ParametersAF)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID FROM TASK WHERE NAME = '" + TaskName + "'";
            int TaskID = -1;
            TaskID = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "UPDATE NeuroNet SET TaskID ='" + Convert.ToString(TaskID) +
                "', TopologyTypeName = '" + TopologyTypeName +
                "', NeuronCount = '" + Convert.ToString(CountNeurons) +
                "', LayerCount ='" + Convert.ToString(CountLayers) +
                "', NeuronsInLayer ='" + NeuronsInLayers +
                "', ActivateFunctionType = '" + typeAF +
                "', AFParameters = '" + ParametersAF +
                "', Name = '" + NewNameNeuroNet + "' WHERE Name = '" + OldNameNeuroNet + "'";
            cmd.ExecuteNonQuery();
            connector.DisconnectFromDB();
        }

        public void InsertTopologyOfNeuroNet(string NameNN, string TopologyTypeName,
            int CountNeurons, int CountLayers, int[] NeuronsInLayers, LoadingWindow loadingWindow)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID FROM NeuroNet WHERE Name = '" + NameNN + "'";
            int NetID = Convert.ToInt32(cmd.ExecuteScalar());

            Topology topology = LibraryOfTopologies.GetTopology(TopologyTypeName, 
                LibraryOfTopologies.GetterParameter.TypeOfTopologyName);
            bool[,] connections = topology.CreateNet(CountNeurons, CountLayers, NeuronsInLayers);

            double progressValue = 0.0;
            for (int i = 0; i < CountNeurons; i++)
            {
                for (int j = 0; j < CountNeurons; j++)
                {
                    if (connections[i, j] == true)
                    {
                        cmd.CommandText = "INSERT INTO NetTopology (IndexInputNeuron, " +
                            "IndexOutputNeuron, NeuroNetID) values('" + Convert.ToString(j) +
                            "','" + Convert.ToString(i) + "','" + Convert.ToString(NetID) + "')";
                        cmd.ExecuteNonQuery();
                    }
                    progressValue += 100.0 / Convert.ToDouble(CountNeurons * CountNeurons);
                    Action f = new Action(() => loadingWindow.LoadingBar.Value = Convert.ToInt32(progressValue));
                    if (loadingWindow.LoadingBar.InvokeRequired)
                    {
                        loadingWindow.LoadingBar.Invoke(f);
                    }
                    else
                    {
                        f();
                    }
                }
            }
            connector.DisconnectFromDB();
            loadingWindow.Invoke(new Action(() => loadingWindow.Close()));
        }

        public void InsertNeuroNet(string NameNN, string TopologyTypeName, string TaskName, int CountNeurons,
            int CountLayers, string NeuronsInLayers, string typeAF, string ParametersAF)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID FROM TASK WHERE NAME = '" + TaskName + "'";
            int TaskID = -1;
            TaskID = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "INSERT INTO NeuroNet (TaskID, NeuronCount, " +
                "LayerCount, NeuronsInLayer, ActivateFunctionType, AFParameters, " +
                "Name, TopologyTypeName) values('" +
                Convert.ToString(TaskID) + "','" +
                Convert.ToString(CountNeurons) + "','" +
                Convert.ToString(CountLayers) + "','" +
                NeuronsInLayers + "','" +
                typeAF + "','" +
                ParametersAF + "','" + NameNN + 
                "','" + TopologyTypeName +  "')";
            cmd.ExecuteNonQuery();
            connector.DisconnectFromDB();
        }

        public bool IsNeuroNetExist(string name)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT COUNT(Name) FROM NeuroNet WHERE Name = '" + name + "'";
            int cNN = Convert.ToInt32(cmd.ExecuteScalar());
            connector.DisconnectFromDB();

            if (cNN > 0)
                return true;
            else
                return false;
        }

        public List<string> SelectCountParametersInTasks()
        {
            List<string> ls = new List<string>();

            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);

            cmd.CommandText = "SELECT PARAM_COUNT FROM TASK";
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(reader[0].ToString());
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            connector.DisconnectFromDB();
            return ls;
        }

        public List<string> SelectTasks()
        {
            List<string> ls = new List<string>();

            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT NAME FROM TASK";
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(reader[0].ToString());
                }
                reader.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            connector.DisconnectFromDB();
            return ls;
        }

        public void DeleteNeuroNet(string NeuroNetName)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);

            cmd.CommandText = "DELETE FROM NeuroNet WHERE Name = '" + NeuroNetName + "'";
            cmd.ExecuteNonQuery();

            connector.DisconnectFromDB();
        }

        public int[] SelectNeuronsInLayers(string NeuroNetName)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT LayerCount, NeuronsInLayer " +
            "FROM NeuroNet WHERE NeuroNet.Name = '" +NeuroNetName + "'";
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int layerCount = Convert.ToInt32(reader[0]);
                string line = Convert.ToString(reader[1]);

                int[] neurons_in_layer = new int[layerCount];
                int k = 0;
                for (int i = 0; i < layerCount; i++)
                {
                    string buf = "";
                    while (k < line.Length && line[k] != ' ')
                    {
                        buf += line[k];
                        k++;
                    }
                    k++;
                    if (buf.Length != 0)
                        neurons_in_layer[i] = Convert.ToInt32(buf);
                }

                connector.DisconnectFromDB();
                return neurons_in_layer;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public List<string[]> SelectNeuronsInLayers(string NeuroNetName, int countNeurons, string NameAF)
        {
            int IDAF = LibraryOfActivateFunctions.GetCountParametersOfAF(NameAF, 
                LibraryOfActivateFunctions.GetterParameter.ActivateFunctionName);
            if (IDAF != -1)
            {
                List<string[]> ls = new List<string[]>();

                connector.ConnectToDB();
                SQLiteCommand cmd = new SQLiteCommand(connector.connection);
                int[] neurons = new int[countNeurons];
                double[] valuesPars = new double[IDAF];
                cmd.CommandText = "SELECT NeuronsInLayer " +
                "FROM NeuroNet,TASK WHERE NeuroNet.TaskID = TASK.ID AND NeuroNet.Name ='" +
                NeuroNetName + "'";
                try
                {
                    string line = Convert.ToString(cmd.ExecuteScalar());

                    int k = 0;
                    int j = 0;
                    string buf = "";
                    string[] row;
                    while (j < line.Length)
                    {
                        row = new string[2];
                        if (line[j] != ' ')
                        {
                            buf += line[j];
                        }
                        else
                        {
                            row[0] = Convert.ToString(k + 1);
                            row[1] = buf;
                            ls.Add(row);
                            k++;
                            buf = "";
                        }
                        j++;
                    }

                    row = new string[2];
                    row[0] = Convert.ToString(k + 1);
                    row[1] = buf;
                    ls.Add(row);
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                connector.DisconnectFromDB();
                return ls;
            }
            else
                throw new Exception("No such name of Activate Function!");
        }

        public List<double> SelectValuesOfParametersOfAF(string NeuroNetName)
        {
            List<double> ls = new List<double>();

            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT AFParameters " +
            "FROM NeuroNet WHERE Name = '" + NeuroNetName + "'";
            string line = "";
            try
            {
                line = cmd.ExecuteScalar().ToString();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();

            int k = 0;
            while(k < line.Length)
            {
                string buf = "";
                while (k < line.Length && line[k] != ' ')
                {
                    buf += line[k];
                    k++;
                }
                k++;
                if (buf.Length != 0)
                    ls.Add(Convert.ToDouble(buf));
            }

            return ls;
        }

        public List<string[]> SelectParametersOfAF(string NeuroNetName, int countNeurons, string NameAF)
        {
            int IDAF = LibraryOfActivateFunctions.GetCountParametersOfAF(NameAF, 
                LibraryOfActivateFunctions.GetterParameter.ActivateFunctionName);
            ActivateFunction af = LibraryOfActivateFunctions.GetActivateFunction(NameAF, 
                LibraryOfActivateFunctions.GetterParameter.ActivateFunctionName);
            if (IDAF != -1)
            {
                List<string[]> ls = new List<string[]>();

                connector.ConnectToDB();
                SQLiteCommand cmd = new SQLiteCommand(connector.connection);
                int[] neurons = new int[countNeurons];
                double[] valuesPars = new double[IDAF];
                cmd.CommandText = "SELECT AFParameters " +
                "FROM NeuroNet,TASK WHERE NeuroNet.TaskID = TASK.ID AND NeuroNet.Name ='" +
                NeuroNetName + "'";
                try
                {
                    string line = Convert.ToString(cmd.ExecuteScalar());

                    int k = 0;
                    int j = 0;
                    string buf = "";
                    string[] row;

                    while (j < line.Length)
                    {
                        row = new string[2];
                        if (line[j] != ' ')
                        {
                            buf += line[j];
                        }
                        else
                        {
                            row[0] = af.GetNameOfParameter(k);
                            row[1] = buf;
                            ls.Add(row);
                            k++;
                            buf = "";
                        }
                        j++;
                    }

                    row = new string[2];
                    row[0] = af.GetNameOfParameter(k);
                    row[1] = buf;
                    ls.Add(row);
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                connector.DisconnectFromDB();
                return ls;
            }
            else
                throw new Exception("No such name of Activate Function!");
        }

        public string[] SelectNeuroNetDefinitionByName(string name)
        {
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT NeuroNet.Name, TopologyTypeName, TASK.NAME, NeuronCount, " +
            "LayerCount, NeuronsInLayer, ActivateFunctionType, AFParameters " +
            "FROM NeuroNet,TASK WHERE NeuroNet.TaskID = TASK.ID AND NeuroNet.Name ='"
            + name + "'";
            string[] line = new string[8];
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < 8; i++)
                    {
                        line[i] = reader[i].ToString();
                    }
                }
                line[1] = LibraryOfTopologies.GetTopologyName(line[1]);
                line[6] = LibraryOfActivateFunctions.GetActivateFunctionName(line[6]);
                reader.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();

            return line;
        }

        public void WriteLearnedWeights(string NeuroNetName, string SelectionName, string TypeOfLA, double[,] weights, bool[,] conns,
            LoadingWindow lw)
        {
            int selID = -1;
            int netID = -1;

            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ID FROM SELECTION WHERE NAME = '" + SelectionName + "'";
            try
            {
                selID = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SQLiteCommand(connector.connection);
                cmd.CommandText = "SELECT ID FROM NeuroNet WHERE Name = '" + NeuroNetName + "'";

                netID = Convert.ToInt32(cmd.ExecuteScalar());


                if (selID > 0)
                {
                    if (netID > 0)
                    {
                        double dim = conns.GetLength(0) * conns.GetLength(1);
                        double progress = 0.0;

                        for (int output = 0; output < conns.GetLength(0); output++)
                        {
                            for (int input = 0; input < conns.GetLength(1); input++)
                            {
                                if (conns[output, input] == true)
                                {
                                    cmd = new SQLiteCommand(connector.connection);
                                    cmd.CommandText = "SELECT ID FROM NetTopology WHERE " + 
                                        "NeuroNetID = " + netID 
                                        + " and IndexOutputNeuron = " + output + 
                                        " and IndexInputNeuron = " + input;
                                    
                                    int connID = Convert.ToInt32(cmd.ExecuteScalar());

                                    cmd.CommandText = "INSERT INTO WeightsMatrix (Weight, " +
                            "TypeLA, NetTopologyID, SELECTIONID) values('" + Convert.ToString(weights[output, input]) +
                            "','" + TypeOfLA + "','" + Convert.ToString(connID) + "','" + Convert.ToString(selID) + "')";
                                    cmd.ExecuteNonQuery();
                                }

                                progress += 100.0 / dim;
                                Action f = new Action(() => lw.LoadingBar.Value = Convert.ToInt32(progress));
                                if (lw.LoadingBar.InvokeRequired)
                                {
                                    lw.LoadingBar.Invoke(f);
                                }
                                else
                                {
                                    f();
                                }
                            }
                        }

                        lw.Invoke(new Action(() => lw.Close()));
                    }
                    else
                    {
                        MessageBox.Show("Данная нейронная сеть не найдена");
                    }
                }
                else
                {
                    MessageBox.Show("Данная выборка не найдена");
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();
        }

        public List<string[]> SelectNeuroNetDefinitions()
        {
            List<string[]> defs = new List<string[]>();

            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT NeuroNet.Name, TopologyTypeName, TASK.NAME, NeuronCount, " +
            "LayerCount, ActivateFunctionType " +
            "FROM NeuroNet,TASK WHERE NeuroNet.TaskID = TASK.ID";
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string[] line = new string[6];
                    for (int i = 0; i < 6; i++)
                    {
                        line[i] = reader[i].ToString();
                    }
                    line[1] = LibraryOfTopologies.GetTopologyName(line[1]);
                    line[5] = LibraryOfActivateFunctions.GetActivateFunctionName(line[5]);
                    defs.Add(line);
                }
                reader.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();

            return defs;
        }

        public List<string> SelectAllNeuroNetsByTask(string TaskName)
        {
            List<string> ls = new List<string>();

            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT NeuroNet.Name FROM NeuroNet,TASK WHERE NeuroNet.TaskID = TASK.ID AND TASK.NAME = '" + TaskName + "'";
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string line;
                    line = reader[0].ToString();
                    ls.Add(line);
                }
                reader.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();

            return ls;
        }

        public List<int> SelectTopologyID(string NeuroNetName)
        {
            List<int> ls = new List<int>();

            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT NetTopology.ID FROM NetTopology, NeuroNet WHERE NeuroNetID = NeuroNet.ID AND NeuroNet.Name = '" + NeuroNetName + "'";
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(Convert.ToInt32(reader[0]));
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();

            return ls;
        }

        public int SelectCountInputParametersInTask(string TaskName)
        {
            int res = 0;
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT PARAM_COUNT FROM TASK WHERE NAME = '" + TaskName + "'";
            try
            {
                res = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();
            return res;
        }
        public int SelectCountNeuronsInNet(string NeuroNetName)
        {
            int res = 0;
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT NeuronCount FROM NeuroNet WHERE Name = '" + NeuroNetName + "'";
            try
            {
                res = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();
            return res;
        }
        public string SelectActivateFunctionTypeByNeuroNet(string NeuroNetName)
        {
            string res = "";
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT ActivateFunctionType FROM NeuroNet WHERE Name = '" + NeuroNetName + "'";
            try
            {
                res = Convert.ToString(cmd.ExecuteScalar());
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();
            return res;
        }
        public List<Tuple<int, int>> SelectUnlearnedTopology(string NeuroNetName)
        {
            List<Tuple<int, int>> ls = new List<Tuple<int, int>>();
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT IndexInputNeuron, IndexOutputNeuron FROM NeuroNet, SELECTION, NetTopology WHERE NeuroNet.Name = '" + NeuroNetName
                + "' AND NeuroNetID = NeuroNet.ID";
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(new Tuple<int, int>(Convert.ToInt32(reader[0]),
                        Convert.ToInt32(reader[1])));
                }
                reader.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();
            return ls;
        }
        public List<Tuple<int,int,double>> SelectLearnedTopology(string NeuroNetName, string SelectionName,
            string LearningAlgorithmType)
        {
            List<Tuple<int, int, double>> ls = new List<Tuple<int, int, double>>();
            connector.ConnectToDB();
            SQLiteCommand cmd = new SQLiteCommand(connector.connection);
            cmd.CommandText = "SELECT IndexInputNeuron, IndexOutputNeuron, Weight FROM NeuroNet, SELECTION, NetTopology, WeightsMatrix WHERE NeuroNet.Name = '" + NeuroNetName 
                + "' AND SELECTION.NAME = '" + SelectionName + "' AND NeuroNetID = NeuroNet.ID AND SELECTIONID = SELECTION.ID AND NetTopologyID = NetTopology.ID AND TypeLA = '" + 
                LearningAlgorithmType + "'";
            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(new Tuple<int, int, double>(Convert.ToInt32(reader[0]),
                        Convert.ToInt32(reader[1]), Convert.ToDouble(reader[2])));
                }
                reader.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connector.DisconnectFromDB();
            return ls;
        }
    }
}
