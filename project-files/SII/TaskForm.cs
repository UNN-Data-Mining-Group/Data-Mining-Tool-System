using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;

namespace SII
{
    public partial class TaskForm : Form
    {
        private SQLManager sqlManager;

        private List<Task> arrTask;
        private List<Parametr> arrParams;
        private List<Selection> arrSelections;

        private bool creatingNewTask;

        private int curTaskID = -1;
        private int curSelectedSelectionRow;

        private int curSelectedTaskRow;

        private bool ShowOtherForm = false;//флаг показа других формъ
        private Form CurrentActiveForm;//текущая активная форма

        static int countTypes = 4;

        static ShowTypeParametr[] showTypeArr = new ShowTypeParametr[4]{
            new ShowTypeParametr(TypeParametr.Int, "Целое"),
            new ShowTypeParametr(TypeParametr.Real, "Вещественное"),
            new ShowTypeParametr(TypeParametr.Bool, "Булево"),
            new ShowTypeParametr(TypeParametr.Enum, "Перечисление")
        };

        public TaskForm()
        {
            InitializeComponent();


            sqlManager = SQLManager.MainSQLManager;
            //init column select
            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)parametersDataGridView.Columns[1];

            for (int i = 0; i < countTypes; i++)
                col.Items.Add(showTypeArr[i]);

            col.DisplayMember = "Name";
            col.ValueMember = "Type";
            ShowAllTasks();
        }

        private void ShowAllTasks()
        {
            List<Task> arr = sqlManager.GetTasksWithRequest("select ID, NAME,PARAM_COUNT,SELECTION_COUNT from TASK;");
            arrTask = arr;
            string[] row;
            DataGridViewRow lastRow;
            foreach (Task task in arr)
            {
                row = new string[] { task.Name, task.CountParameters.ToString(), task.CountSelections.ToString(), "Изменить", "Изменить", "Удалить" };
                tasksDataGridView.Rows.Add(row);
                lastRow = ((DataGridViewRow)tasksDataGridView.Rows[tasksDataGridView.Rows.Count - 1]);
                lastRow.ReadOnly = true;
            }
        }

        private void createNewTaskBtn_Click(object sender, EventArgs e)
        {
            createNewTaskBtn.Enabled = false;
            creationEndBtn.Enabled = true;
            addedSelectionEndBtn.Enabled = false;
            addSelectionBtn.Enabled = true;
            string[] row = new string[] { "", "", "", "", "Отмена", "Создать"};
            tasksDataGridView.Rows.Add(row);
            DataGridViewRow lastRow = ((DataGridViewRow)tasksDataGridView.Rows[tasksDataGridView.Rows.Count - 1]);
            creatingNewTask = true;
            parametersDataGridView.Rows.Clear();
            selectionsDataGridView.Rows.Clear();
        }

        private void createNewTask(String name, String countParams, String countSelections)
        {
            sqlManager.StartTransaction();
            String sqlReqStr = "INSERT INTO TASK (NAME, PARAM_COUNT, SELECTION_COUNT) " +
                "VALUES('" + name + "','" + countParams + "','" + countSelections + "');";
            int state = sqlManager.SendInsertRequest(sqlReqStr);
            if (state == 0)
                Console.WriteLine("error");
            curTaskID = state;
        }

        public void canselCreateTask(bool state)
        {
            if (state)
            {
                //cancel
                tasksDataGridView.Rows.RemoveAt(tasksDataGridView.RowCount - 1);
                creatingNewTask = false;
                sqlManager.EndTransaction(false);
                createNewTaskBtn.Enabled = true;
                ShowOtherForm = false;
                CurrentActiveForm = null;
                creationEndBtn.Enabled = false;
            }
            else
            {
                //продолжение создания
                if (CurrentActiveForm.GetType() == typeof(ParametersForm))
                {
                    DataGridViewRow row = tasksDataGridView.Rows[tasksDataGridView.RowCount - 1];

                    SelectionForm selectionForm = new SelectionForm();
                    selectionForm.CountSelections = int.Parse(row.Cells[2].Value.ToString());
                    selectionForm.SetCreateNewTask(true);
                    selectionForm.SetTaskID(curTaskID);
                    selectionForm.Show();

                    selectionForm.MainForm = this;
                    ShowOtherForm = true;

                    CurrentActiveForm = selectionForm;
                }
                else if (CurrentActiveForm.GetType() == typeof(SelectionForm))
                {
                    //окончание создания
                    CurrentActiveForm = null;
                    creatingNewTask = false;
                    sqlManager.EndTransaction(true);
                    createNewTaskBtn.Enabled = true;
                    ShowOtherForm = false;
                    tasksDataGridView.Rows.Clear();
                    ShowAllTasks();
                }
            }
        }

        private void UpdateTask(Task newTask)
        {
            String sqlReqStr = "UPDATE TASK SET NAME='" + newTask.Name + "',PARAM_COUNT='" + newTask.CountParameters + "',SELECTION_COUNT='" + newTask.CountSelections + "' "+
                "WHERE ID='" + newTask.ID + "';";
            sqlManager.SendUpdateRequest(sqlReqStr);
        }

        private void UpdateSelection(Selection newSelection)
        {
            String sqlReqStr = "UPDATE SELECTION SET NAME='" + newSelection.Name + "',COUNT_ROWS='" + newSelection.CountRows + "' " +
                "WHERE ID='" + newSelection.ID + "';";
            sqlManager.SendUpdateRequest(sqlReqStr);
        }

        private void DeleteTask(Task task)
        {
            String sqlReqStr = "DELETE FROM TASK WHERE ID='" + task.ID + "';";
            sqlManager.SendDeleteRequest(sqlReqStr);
            sqlReqStr = "DELETE FROM SELECTION WHERE TASK_ID='" + task.ID + "';";
            sqlManager.SendDeleteRequest(sqlReqStr);
            sqlReqStr = "DELETE FROM PARAM WHERE TASK_ID='" + task.ID + "';";
            sqlManager.SendDeleteRequest(sqlReqStr);
        }

        private void ShowAllParams()
        {
            arrParams = sqlManager.GetParamsWithRequest(String.Format("select * from PARAM where TASK_ID='{0}';", curTaskID));
            object[] row;
            DataGridViewRow lastRow;
            if (arrParams != null)
            {
                foreach (Parametr param in arrParams)
                {
                    row = new object[] { param.Name, param.Type, param.Range, param.Number.ToString(), "Изменить" };
                    parametersDataGridView.Rows.Add(row);
                    lastRow = ((DataGridViewRow)parametersDataGridView.Rows[parametersDataGridView.Rows.Count - 1]);
                    lastRow.ReadOnly = true;
                }
            }
        }

        private void ShowAllSelections()
        {
            arrSelections = sqlManager.GetSelectionsWithRequest(String.Format("select * from SELECTION where TASK_ID='{0}';", curTaskID));
            string[] row;
            DataGridViewRow lastRow;
            if (arrSelections != null)
            {
                foreach (Selection param in arrSelections)
                {
                    row = new string[] { param.Name, param.CountRows.ToString(), "Изменить" };
                    selectionsDataGridView.Rows.Add(row);
                    lastRow = ((DataGridViewRow)selectionsDataGridView.Rows[selectionsDataGridView.Rows.Count - 1]);
                    lastRow.ReadOnly = true;
                }
            }
        }

        private void tasksDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ShowOtherForm)
                return;
            if (creatingNewTask)
            {
                if (e.ColumnIndex == 4 && e.RowIndex == tasksDataGridView.RowCount - 1)
                {
                    //cancel
                    canselCreateTask(false);
                }
                if (e.ColumnIndex == 5 && e.RowIndex == tasksDataGridView.RowCount - 1)
                {
                    //create
                    DataGridViewRow row = tasksDataGridView.Rows[tasksDataGridView.RowCount - 1];
                    if (row.Cells[0].Value.ToString() != "" && row.Cells[1].Value.ToString() != "" && row.Cells[2].Value.ToString() != "")
                    {
                        createNewTask(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString()); //insert in db
                        ParametersForm paramForm = new ParametersForm();
                        paramForm.CountParams = int.Parse(row.Cells[1].Value.ToString());
                        paramForm.SetCreateNewTask(true);
                        paramForm.SetTaskID(curTaskID);
                        paramForm.Show();
                        
                        paramForm.MainForm = this;
                        ShowOtherForm = true;

                        CurrentActiveForm = paramForm;
                    }                    
                }
            }
            else
            {
                if (e.ColumnIndex == 3)
                {
                    //show parameters of task
                    Task task = arrTask[e.RowIndex];
                    ParametersForm paramForm = new ParametersForm();
                    paramForm.SetTaskID(task.ID);
                    paramForm.SetCreateNewTask(false);
                    paramForm.Show();

                    paramForm.MainForm = this;
                }
                if (e.ColumnIndex == 4)
                {
                    //show selections of task
                    Task task = arrTask[e.RowIndex];
                    SelectionForm paramForm = new SelectionForm();
                    paramForm.SetTaskID(task.ID);
                    paramForm.SetCreateNewTask(false);
                    paramForm.Show();

                    paramForm.MainForm = this;
                }
                if (e.ColumnIndex == 5)
                {
                    //delete current task
                    Task task = arrTask[e.RowIndex];
                    DeleteTask(task);
                    tasksDataGridView.Rows.Clear();
                    ShowAllTasks();
                }
            }            
        }

        private void createNewParam(String name, TypeParametr type, String range, String index)
        {
            String sqlReqStr = "INSERT INTO PARAM (TASK_ID, NAME, TYPE, RANGE, NUMBER) " +
                "VALUES('" + curTaskID + "','" + name + "','" + ((int)type).ToString() + "','" + range + "','" + index + "');";
            int state = sqlManager.SendInsertRequest(sqlReqStr);
            if (state == 0)
                Console.WriteLine("error");
        }

        private void createNewSelection(String name, String countRows, bool withRes)
        {
            String sqlReqStr = "INSERT INTO SELECTION (TASK_ID, NAME, COUNT_ROWS, WITH_RESULT) " +
                "VALUES('" + curTaskID + "','" + name + "','" + countRows + "','" + (withRes ? 1 : 0).ToString() + "');";
            int state = sqlManager.SendInsertRequest(sqlReqStr);
            if (state == 0)
                Console.WriteLine("error");
        }

        private void creationEndBtn_Click(object sender, EventArgs e)
        {
            bool fullContent = true;
            DataGridViewRow rowTask = tasksDataGridView.Rows[tasksDataGridView.RowCount - 1];
            if (rowTask.Cells[0].Value.ToString() == "" || rowTask.Cells[1].Value.ToString() == "" || rowTask.Cells[2].Value.ToString() == "")
                fullContent = false;
            foreach (DataGridViewRow row in parametersDataGridView.Rows)
            {
                for (int i = 0; i < 4; i++)
                    if (row.Cells[i].Value.ToString() == "")
                        fullContent = false;
            }
            foreach (DataGridViewRow row in selectionsDataGridView.Rows)
            {
                for (int i = 0; i < 2; i++)
                    if (row.Cells[i].Value.ToString() == "")
                        fullContent = false;
            }
            
            if (fullContent)
            {
                //создаем задачу
                createNewTask(rowTask.Cells[0].Value.ToString(), rowTask.Cells[1].Value.ToString(), rowTask.Cells[2].Value.ToString());
                //создаем параметры
                foreach (DataGridViewRow row in parametersDataGridView.Rows)
                {
                    createNewParam(row.Cells[0].Value.ToString(), (TypeParametr)Enum.Parse(typeof(TypeParametr), row.Cells[1].Value.ToString()), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString());
                }
                foreach (DataGridViewRow row in selectionsDataGridView.Rows)
                {
                    createNewSelection(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), true);
                }
                sqlManager.EndTransaction(true);
                tasksDataGridView.Rows.Clear();
                ShowAllTasks();
                createNewTaskBtn.Enabled = true;
                creationEndBtn.Enabled = false;
            }
        }

        private void tasksDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //show parameters and selections of task
            parametersDataGridView.Rows.Clear();
            selectionsDataGridView.Rows.Clear();
            if (creationEndBtn.Enabled == true)
                canselCreateTask(true);
            Task task = arrTask[e.RowIndex];
            curSelectedTaskRow = e.RowIndex; 
            curTaskID = task.ID;
            ShowAllParams();
            ShowAllSelections();
            addedSelectionEndBtn.Enabled = false;
            addSelectionBtn.Enabled = true;
        }

        private void tasksDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string[] addingRow;
            int countInDataGrid = 0;
            DataGridViewRow row = tasksDataGridView.Rows[tasksDataGridView.RowCount - 1];
            if (e.ColumnIndex == 1) 
            {
                parametersDataGridView.Rows.Clear();
                //params change
                countInDataGrid = int.Parse(row.Cells[1].Value.ToString()) + 1;
                for (int i = 0; i < countInDataGrid; i++)
                {
                    addingRow = new string[] { "", "", "", i.ToString(), "" };
                    parametersDataGridView.Rows.Add(addingRow);
                }
            }
            if (e.ColumnIndex == 2)
            {
                selectionsDataGridView.Rows.Clear();
                countInDataGrid = int.Parse(row.Cells[2].Value.ToString());
                //selections change
                for (int i = 0; i < countInDataGrid; i++)
                {
                    addingRow = new string[] { "", "", "" };
                    selectionsDataGridView.Rows.Add(addingRow);
                }
            }
            
        }

        private void deleteTaskBtn_Click(object sender, EventArgs e)
        {
            parametersDataGridView.Rows.Clear();
            selectionsDataGridView.Rows.Clear();
            Task task = arrTask[curSelectedTaskRow];
            DeleteTask(task);
            tasksDataGridView.Rows.Clear();
            ShowAllTasks();
        }

        private void addSelectionBtn_Click(object sender, EventArgs e)
        {
            string[] addingRow = new string[] { "", "", "" };
            addSelectionBtn.Enabled = false;
            addedSelectionEndBtn.Enabled = true;
            selectionsDataGridView.Rows.Add(addingRow);
        }

        private void addedEndBtn_Click(object sender, EventArgs e)
        {            
            DataGridViewRow row = selectionsDataGridView.Rows[selectionsDataGridView.RowCount - 1];
            if (row.Cells[0].Value.ToString() != "")
            {
                OpenFileDialog openValuesFile = new OpenFileDialog();

                openValuesFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
                openValuesFile.FilterIndex = 2 ;
                openValuesFile.RestoreDirectory = true ;

                if (openValuesFile.ShowDialog() == DialogResult.OK)
                {
                    createNewSelection(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), true);
                    selectionsDataGridView.Rows.Clear();
                    ShowAllSelections();
                    Task task = arrTask[curSelectedTaskRow];
                    task.CountSelections += 1;
                    UpdateTask(task);
                    tasksDataGridView.Rows.Clear();
                    ShowAllTasks();
                    addSelectionBtn.Enabled = true;
                    addedSelectionEndBtn.Enabled = false;
                    Selection curSelection = arrSelections.Last();
                    curSelection.WithRes = true;//test
                    curSelection.LoadArrValueParametersFromFile(openValuesFile.FileName, arrParams);
                    row = selectionsDataGridView.Rows[selectionsDataGridView.RowCount - 1];
                    row.Cells[1].Value = curSelection.CountRows;
                    UpdateSelection(curSelection);
                }                
            }            
        }

        private void selectionsDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            curSelectedSelectionRow = e.RowIndex;
            Selection curSelection = arrSelections.ElementAt(curSelectedSelectionRow);
            if (addedSelectionEndBtn.Enabled)
            {
                //идет создание выборки
                if (e.RowIndex != selectionsDataGridView.RowCount - 1)
                {
                    selectionsDataGridView.Rows.RemoveAt(selectionsDataGridView.RowCount - 1);
                    addedSelectionEndBtn.Enabled = false;
                    addSelectionBtn.Enabled = true;
                    ShowAllValuesOfSelection(curSelection);
                }
            }
            else
            {
                valuesDataGridView.Rows.Clear();
                ShowAllValuesOfSelection(curSelection);
            }
            
        }

        private void ShowAllValuesOfSelection(Selection selection)
        {
            valuesGroupBox.Text = "Значения выборки " + selection.Name;
            Task curTask = arrTask[curSelectedTaskRow];
            List<ValueParametr> arrValues = sqlManager.GetValuesWithRequest(String.Format("select * from VALUE_PARAM where SELECTION_ID='{0}';", selection.ID));
            List<string> row;
            valuesDataGridView.Rows.Clear();
            valuesDataGridView.Columns.Clear();
            foreach (Parametr param in arrParams)
            {
                valuesDataGridView.Columns.Add(param.Name + "column", param.Name);
            }
            if (arrValues != null)
            {
                int prevIndex = arrValues[0].RowIndex;
                row = new List<string>();
                if (!selection.WithRes)//добавляем пустую строку вместо параметра 0
                    row.Add("");
                foreach (ValueParametr valueParam in arrValues)
                {
                    if (valueParam.RowIndex != prevIndex)
                    {
                        valuesDataGridView.Rows.Add(row.ToArray());
                        row = new List<string>();
                        if (!selection.WithRes)//добавляем пустую строку вместо параметра 0
                            row.Add("");
                    }
                    row.Add(valueParam.Value);
                    prevIndex = valueParam.RowIndex;
                }
            }
        }

        private void removeSelectionBtn_Click(object sender, EventArgs e)
        {
            Selection curSelection = arrSelections.ElementAt(curSelectedSelectionRow);
            String sqlReqStr = "DELETE FROM VALUE_PARAM WHERE SELECTION_ID='" + curSelection.ID + "';";
            sqlManager.SendDeleteRequest(sqlReqStr);
            sqlReqStr = "DELETE FROM SELECTION WHERE ID='" + curSelection.ID + "';";
            sqlManager.SendDeleteRequest(sqlReqStr);
            selectionsDataGridView.Rows.Clear();
            ShowAllSelections();
            Task curTask = arrTask[curSelectedTaskRow];
            curTask.CountSelections--;
            UpdateTask(curTask);
            tasksDataGridView.Rows.Clear();
            ShowAllTasks();
            valuesDataGridView.Rows.Clear();
        }

        private void btnNeuroWnd_Click(object sender, EventArgs e)
        {
            
        }
    }
}
