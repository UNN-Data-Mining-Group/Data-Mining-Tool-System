using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using NeuroWnd.Neuro_Nets;
using NeuroWnd.Activate_functions;

namespace NeuroWnd
{
    public partial class AddChangeNeuroNetDialog : Form
    {
        private DataBaseHandler dbHandler;
        private int[] countInputParamsInTask;
        private NeuroNetDefinition definition_of_editing_net; //Для режима изменения
        private enum form_destination { Editing, Creating };
        private form_destination mode;

        private void RefreshDgwParametersAF()
        {
            dgwEditParametersAF.Rows.Clear();
            int countParameters = LibraryOfActivateFunctions.GetCountParametersOfAF(cbActivateFunction.SelectedItem.ToString(), 
                LibraryOfActivateFunctions.GetterParameter.ActivateFunctionName);
            ActivateFunction af = LibraryOfActivateFunctions.GetActivateFunction(cbActivateFunction.SelectedItem.ToString(),
                LibraryOfActivateFunctions.GetterParameter.ActivateFunctionName);
            

            for (int i = 0; i < countParameters; i++)
            {
                string[] row = new string[2];
                row[0] = Convert.ToString(af.GetNameOfParameter(i));
                row[1] = Convert.ToString(af.GetDefaultValueOfParameter(i));
                dgwEditParametersAF.Rows.Add(row);
                dgwEditParametersAF.Rows[i].Cells[0].ReadOnly = true;
            }
        }

        private void RefreshDgwNeuronsInLayers()
        {
            dgwEditNeuronsInLayers.Rows.Clear();
            int countRows = Convert.ToInt32(numLayersNumber.Value);
            string[] row = new string[2];
            row[0] = Convert.ToString(1);
            if(cbTask.SelectedIndex != -1)
                row[1] = Convert.ToString(countInputParamsInTask[cbTask.SelectedIndex]);
            else
                row[1] = Convert.ToString(0);
            dgwEditNeuronsInLayers.Rows.Add(row);
            dgwEditNeuronsInLayers.Rows[0].Cells[0].ReadOnly = true;
            dgwEditNeuronsInLayers.Rows[0].Cells[1].ReadOnly = true;
            for (int i = 1; i < countRows - 1; i++)
            {
                row = new string[2];
                row[0] = Convert.ToString(i + 1);
                row[1] = Convert.ToString(0);
                dgwEditNeuronsInLayers.Rows.Add(row);
                dgwEditNeuronsInLayers.Rows[i].Cells[0].ReadOnly = true;
            }
            row = new string[2];
            row[0] = Convert.ToString(countRows);
            row[1] = Convert.ToString(1);
            dgwEditNeuronsInLayers.Rows.Add(row);
            dgwEditNeuronsInLayers.Rows[countRows - 1].Cells[0].ReadOnly = true;
            dgwEditNeuronsInLayers.Rows[countRows - 1].Cells[1].ReadOnly = true;
        }

        public AddChangeNeuroNetDialog(DataBaseHandler _dbHandler, NeuroNetDefinition neuro_def = null)
        {
            InitializeComponent();

            if (neuro_def == null)
                mode = form_destination.Creating;
            else
            {
                mode = form_destination.Editing;
                definition_of_editing_net = neuro_def;
            }

            dbHandler = _dbHandler;

            List<string> ls = dbHandler.SelectTasks();
            foreach (string par in ls)
            {
                cbTask.Items.Add(par);
            }
            
            ls = LibraryOfTopologies.GetAllTopologyTypeNames();
            foreach (string item in ls)
            {
                cbTopology.Items.Add(item);
            }

            string[] namesAF = LibraryOfActivateFunctions.GetAllActivateFunctionNames();
            for (int i = 0; i < LibraryOfActivateFunctions.GetCountActivateFunctions(); i++)
            {
                cbActivateFunction.Items.Add(namesAF[i]);
            }

            ls = dbHandler.SelectCountParametersInTasks();
            countInputParamsInTask = new int[ls.Count];
            for (int i = 0; i < ls.Count; i++)
            {
                countInputParamsInTask[i] = Convert.ToInt32(ls[i]);
            }

            dgwEditNeuronsInLayers.ColumnHeadersDefaultCellStyle.Font = new Font("Book Antiqua", 9);
            dgwEditNeuronsInLayers.Columns.Add("Layer", "Слой");
            dgwEditNeuronsInLayers.Columns.Add("Neurons", "Число нейронов");
            RefreshDgwNeuronsInLayers();

            dgwEditParametersAF.ColumnHeadersDefaultCellStyle.Font = new Font("Book Antiqua", 9);
            dgwEditParametersAF.Columns.Add("Parameter", "Параметр");
            dgwEditParametersAF.Columns.Add("Value", "Значение");

            switch (mode)
            {
                case form_destination.Creating:
                    Text = "Создание нейронной сети";
                    break;
                case form_destination.Editing:
                    Text = "Изменение Нейронной Сети";
                    tbNameNeuroNet.Text = definition_of_editing_net.Name;
                    for (int i = 0; i < cbTask.Items.Count; i++)
                    {
                        if (String.Compare(cbTask.Items[i].ToString(), definition_of_editing_net.TaskName) == 0)
                        {
                            cbTask.SelectedIndex = i;
                            break;
                        }
                    }
                    for (int i = 0; i < cbTopology.Items.Count; i++)
                    {
                        if (String.Compare(cbTopology.Items[i].ToString(), definition_of_editing_net.TopologyName) == 0)
                        {
                            cbTopology.SelectedIndex = i;
                            break;
                        }
                    }
                    if (cbTopology.SelectedIndex == -1)
                    {
                        MessageBox.Show("Топология, примененная в данной нейронной сети, " +
                        "не найдена. Она будет недоступна при изменении сети.");
                    }

                    numNeuronsNumber.Value = definition_of_editing_net.NeuronsCount;
                    numLayersNumber.Value = definition_of_editing_net.LayerCount;

                    for (int i = 0; i < definition_of_editing_net.LayerCount; i++)
                    {
                        dgwEditNeuronsInLayers.Rows[i].Cells[1].Value = definition_of_editing_net.NeuronsInLayer[i];
                    }

                    for (int i = 0; i < cbActivateFunction.Items.Count; i++)
                    {
                        if (String.Compare(cbActivateFunction.Items[i].ToString(), definition_of_editing_net.ActivateFunction) == 0)
                        {
                            cbActivateFunction.SelectedIndex = i;
                            break;
                        }
                    }

                    for (int i = 0; i < definition_of_editing_net.AFParameters.Length; i++)
                    {
                        dgwEditParametersAF.Rows[i].Cells[1].Value = definition_of_editing_net.AFParameters[i];
                    }
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbActivateFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDgwParametersAF();
        }

        private void numLayersNumber_ValueChanged(object sender, EventArgs e)
        {
            RefreshDgwNeuronsInLayers();
        }

        private void LoadNeuroNetIntoDB()
        {
            if (mode == form_destination.Creating)
            {
                try
                {
                    string NameNN = tbNameNeuroNet.Text;
                    if (NameNN == "")
                        throw new Exception("Имя НС не может быть пустым");

                    if (dbHandler.IsNeuroNetExist(NameNN))
                        throw new Exception("Такое имя НС уже используется");

                    if (cbTopology.SelectedItem == null)
                        throw new Exception("Необходимо выбрать топологию");
                    string TopologyTypeName = LibraryOfTopologies.GetTopologyTypeName(cbTopology.SelectedItem.ToString());

                    if (cbTask.SelectedItem == null)
                        throw new Exception("Необходимо выбрать задачу");
                    string TaskName = cbTask.SelectedItem.ToString();

                    int CountNeurons = Convert.ToInt32(numNeuronsNumber.Value);
                    int CountLayers = Convert.ToInt32(numLayersNumber.Value);
                    string typeAF = LibraryOfActivateFunctions.GetActivateFunctionTypeName(cbActivateFunction.SelectedItem.ToString());
                    if (cbActivateFunction.SelectedIndex < 0)
                        throw new Exception("Необходимо выбрать активационную функцию");

                    string NeuronsInLayers = "";
                    int[] NeuInL = new int[CountLayers];
                    string ParametersAF = "";

                    int sumNeurons = 0;
                    for (int i = 0; i < CountLayers; i++)
                    {
                        int neurs = Convert.ToInt32(dgwEditNeuronsInLayers.Rows[i].Cells[1].Value);
                        if (neurs <= 0)
                            throw new Exception("В каждом слое должен быть по крайней мере 1 нейрон");
                        sumNeurons += neurs;

                        NeuInL[i] = neurs;

                        NeuronsInLayers += Convert.ToString(dgwEditNeuronsInLayers.Rows[i].Cells[1].Value);
                        if (i != CountLayers - 1)
                            NeuronsInLayers += " ";
                    }
                    if (sumNeurons != CountNeurons)
                        throw new Exception("Сумма нейронов по слоям должна равняться общему числу нейронов");

                    for (int i = 0; i < LibraryOfActivateFunctions.GetCountParametersOfAF(typeAF, 
                        LibraryOfActivateFunctions.GetterParameter.TypeOfActivateFunctionName); i++)
                    {
                        double par = Convert.ToDouble(dgwEditParametersAF.Rows[i].Cells[1].Value);
                        ParametersAF += Convert.ToString(par);
                        if (i != LibraryOfActivateFunctions.GetCountParametersOfAF(typeAF,
                        LibraryOfActivateFunctions.GetterParameter.TypeOfActivateFunctionName) - 1)
                            ParametersAF += " ";
                    }

                    dbHandler.InsertNeuroNet(NameNN, TopologyTypeName, TaskName, CountNeurons, CountLayers, NeuronsInLayers, typeAF, ParametersAF);

                    LoadingWindow loadingWindow = new LoadingWindow();
                    loadingWindow.MakeLoading(
                        () => dbHandler.InsertTopologyOfNeuroNet(NameNN, TopologyTypeName, CountNeurons, CountLayers, NeuInL, loadingWindow),
                        "Запись нейронной сети в БД");
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (mode == form_destination.Editing)
            {
                try
                {
                    string NameNN = tbNameNeuroNet.Text;
                    if (NameNN == "")
                        throw new Exception("Имя НС не может быть пустым");

                    if (cbTopology.SelectedItem == null)
                        throw new Exception("Необходимо выбрать топологию");
                    string TopologyTypeName = LibraryOfTopologies.GetTopologyTypeName(cbTopology.SelectedItem.ToString());

                    string TaskName = cbTask.SelectedItem.ToString();

                    int CountNeurons = Convert.ToInt32(numNeuronsNumber.Value);
                    int CountLayers = Convert.ToInt32(numLayersNumber.Value);
                    string typeAF = LibraryOfActivateFunctions.GetActivateFunctionTypeName(cbActivateFunction.SelectedItem.ToString());

                    string NeuronsInLayers = "";
                    int[] NeuInL = new int[CountLayers];
                    double[] ParAF = new double[LibraryOfActivateFunctions.GetCountParametersOfAF(typeAF,
                        LibraryOfActivateFunctions.GetterParameter.TypeOfActivateFunctionName)];
                    string ParametersAF = "";

                    int sumNeurons = 0;
                    for (int i = 0; i < CountLayers; i++)
                    {
                        int neurs = Convert.ToInt32(dgwEditNeuronsInLayers.Rows[i].Cells[1].Value);
                        if (neurs <= 0)
                            throw new Exception("В каждом слое должен быть по крайней мере 1 нейрон");
                        sumNeurons += neurs;

                        NeuInL[i] = neurs;

                        NeuronsInLayers += Convert.ToString(dgwEditNeuronsInLayers.Rows[i].Cells[1].Value);
                        if (i != CountLayers - 1)
                            NeuronsInLayers += " ";
                    }
                    if (sumNeurons != CountNeurons)
                        throw new Exception("Сумма нейронов по слоям должна равняться общему числу нейронов");

                    for (int i = 0; i < LibraryOfActivateFunctions.GetCountParametersOfAF(typeAF,
                        LibraryOfActivateFunctions.GetterParameter.TypeOfActivateFunctionName); i++)
                    {
                        ParAF[i] = Convert.ToDouble(dgwEditParametersAF.Rows[i].Cells[1].Value);
                        ParametersAF += Convert.ToString(dgwEditParametersAF.Rows[i].Cells[1].Value);
                        if (i != LibraryOfActivateFunctions.GetCountParametersOfAF(typeAF,
                            LibraryOfActivateFunctions.GetterParameter.TypeOfActivateFunctionName) - 1)
                            ParametersAF += " ";
                    }

                    bool isContining = true;
                    bool isNeuronsInLayersSame = false;
                    bool isParametersAFSame = false;
                    bool isNeedToDelete = false;

                    if (CountLayers == definition_of_editing_net.LayerCount)
                    {
                        int k = 0;
                        for (int i = 0; i < CountLayers; i++)
                        {
                            if (NeuInL[i] == definition_of_editing_net.NeuronsInLayer[i])
                            {
                                k++;
                            }
                        }
                        if (k == CountLayers)
                            isNeuronsInLayersSame = true;
                    }
                    if (String.Compare(cbActivateFunction.SelectedItem.ToString(), definition_of_editing_net.ActivateFunction) == 0)
                    {
                        int k = 0;
                        for (int i = 0; i < definition_of_editing_net.AFParameters.Length; i++)
                        {
                            if (ParAF[i] == definition_of_editing_net.AFParameters[i])
                            {
                                k++;
                            }
                        }
                        if (k == definition_of_editing_net.AFParameters.Length)
                            isParametersAFSame = true;
                    }
                    if (String.Compare(TaskName, definition_of_editing_net.TaskName) != 0 ||
                        String.Compare(TopologyTypeName, definition_of_editing_net.TopologyName) != 0 ||
                        CountNeurons != definition_of_editing_net.NeuronsCount ||
                        isNeuronsInLayersSame == false || isParametersAFSame == false)
                    {
                        DialogResult res = MessageBox.Show("Данные изменения влекут удаление обученных весов сети. " +
                        "Нейронную сеть необходимо будет обучать заново.", "Предупреждение потери данных", MessageBoxButtons.OKCancel);

                        isNeedToDelete = true;
                        if (res == DialogResult.Cancel)
                            isContining = false;
                    }
                    
                    if (isContining == true)
                    {
                        if (isNeedToDelete == true)
                        {
                            LoadingWindow loadingWindow = new LoadingWindow();

                            if (isNeuronsInLayersSame && (String.Compare(cbTopology.SelectedItem.ToString(), definition_of_editing_net.TopologyName) == 0))
                            {
                                loadingWindow.MakeLoading(
                                    ()=>dbHandler.DeleteWeightsMatrix(definition_of_editing_net.Name, loadingWindow),
                                    "Удаление устаревших весовых коэффициентов");
                            }
                            else
                            {
                                loadingWindow.MakeLoading(
                                () => dbHandler.DeleteTopologyAndWeightsMatrix(definition_of_editing_net.Name, loadingWindow),
                                "Удаление старой топологии нейронной сети");
                                loadingWindow.MakeLoading(
                                    () => dbHandler.InsertTopologyOfNeuroNet(definition_of_editing_net.Name, TopologyTypeName, CountNeurons, CountLayers, NeuInL, loadingWindow),
                                    "Запись измененной нейронной сети в БД");
                            }
                        }

                        dbHandler.UpdateNeuroNet(NameNN, definition_of_editing_net.Name, TopologyTypeName, TaskName, CountNeurons, CountLayers,
                           NeuronsInLayers, typeAF, ParametersAF);
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            LoadNeuroNetIntoDB();
        }

        private void cbTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            numNeuronsNumber.Minimum = countInputParamsInTask[cbTask.SelectedIndex];
            RefreshDgwNeuronsInLayers();
        }
    }
}
