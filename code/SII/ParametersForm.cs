using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SII
{
    public partial class ParametersForm : Form
    {
        static int countTypes = 4;

        static ShowTypeParametr[] showTypeArr = new ShowTypeParametr[4]{
            new ShowTypeParametr(TypeParametr.Int, "Целое"),
            new ShowTypeParametr(TypeParametr.Real, "Вещественное"),
            new ShowTypeParametr(TypeParametr.Bool, "Булево"),
            new ShowTypeParametr(TypeParametr.Enum, "Перечисление")
        };

        public TaskForm MainForm;

        private SQLManager sqlManager;

        int TaskID;
        bool CreateNewTask;
        public int CountParams;

        bool ChangeParam = false;
        int CurChangeParamRow;

        List<Parametr> arrParams;
        bool SuccessCreate = false;

        public ParametersForm()
        {
            InitializeComponent();
            sqlManager = SQLManager.MainSQLManager;

            //init column select
            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)parametersDataGridView.Columns[1];

            for (int i = 0; i < countTypes; i++)
                col.Items.Add(showTypeArr[i]);

            col.DisplayMember = "Name";
            col.ValueMember = "Type";
        }

        public void SetTaskID(int _taskID)
        {
            TaskID = _taskID;
        }

        public void SetCreateNewTask(bool _createNewTask)
        {
            CreateNewTask = _createNewTask;
            if (CreateNewTask)
            {
                string[] row;
                string lastColumnValue = "";
                DataGridViewRow lastRow;
                for (int i = 0; i < CountParams + 1; i++)
                {
                    if (i == CountParams - 1)
                        lastColumnValue = "Создать!";
                    if (i == CountParams - 2)
                        lastColumnValue = "Отмена";
                    row = new string[] { "", "", "", i.ToString(), lastColumnValue };
                    parametersDataGridView.Rows.Add(row);
                    lastRow = lastRow = ((DataGridViewRow)parametersDataGridView.Rows[parametersDataGridView.Rows.Count - 1]);
                    lastColumnValue = "";
                }
            }
            else
            {
                ShowAllParams();           
            }
        }

        private void ShowAllParams()
        {
            arrParams = sqlManager.GetParamsWithRequest(String.Format("select * from PARAM where TASK_ID='{0}';", TaskID));
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

        private void createNewParam(String name, TypeParametr type, String range, String index)
        {
            String sqlReqStr = "INSERT INTO PARAM (TASK_ID, NAME, TYPE, RANGE, NUMBER) " +
                "VALUES('" + TaskID + "','" + name + "','" + ((int)type).ToString() + "','" + range + "','" + index + "');";
            int state = sqlManager.SendInsertRequest(sqlReqStr);
            if (state == 0)
                Console.WriteLine("error");
        }

        private void UpdateParam(Parametr newParam)
        {
            String sqlReqStr = "UPDATE PARAM SET NAME='" + newParam.Name + "',TYPE='" + ((int)newParam.Type).ToString() + "', " +
                "RANGE='" + newParam.Range + "', NUMBER='" + newParam.Number + "' " +
                "WHERE TASK_ID='" + newParam.TaskID + "' and ID='" + newParam.ID + "';";
            sqlManager.SendUpdateRequest(sqlReqStr);
        }

        private void parametersDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CreateNewTask)
            {
                //если создается новая задача
                if (e.ColumnIndex == 4 && e.RowIndex == CountParams - 1)
                {
                    bool fullContent = true;
                    foreach (DataGridViewRow row in parametersDataGridView.Rows)
                    {
                        for (int i = 0; i < 4; i++)
                            if (row.Cells[i].Value.ToString() == "")
                                fullContent = false;
                    }
                    if (fullContent)
                    {
                        //отсылаем в бд, закрываем форму, уведомляем о успешном создании
                        foreach (DataGridViewRow row in parametersDataGridView.Rows)
                        {
                            createNewParam(row.Cells[0].Value.ToString(), (TypeParametr)Enum.Parse(typeof(TypeParametr), row.Cells[1].Value.ToString()), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString());
                        }
                        SuccessCreate = true;
                        this.Close();
                    }
                }
                if (e.ColumnIndex == 4 && e.RowIndex == CountParams - 2)
                {
                    this.Close();
                }
            }
            else
            {
                //если хотим обновить
                if (!ChangeParam)
                {
                    if (e.ColumnIndex == 4)
                    {
                        Parametr param = arrParams[e.RowIndex];
                        parametersDataGridView.Rows[e.RowIndex].ReadOnly = false;
                        parametersDataGridView.Rows[e.RowIndex].Cells[4].Value = "Сохранить";
                        ChangeParam = true;
                        CurChangeParamRow = e.RowIndex;
                    }
                }
                else
                {
                    if (e.ColumnIndex == 4 && e.RowIndex == CurChangeParamRow)
                    {
                        //Save changes
                        bool fullContent = true;
                        for (int i = 0; i < 3; i++)
                            if (parametersDataGridView.Rows[CurChangeParamRow].Cells[i].Value.ToString() == "")
                                fullContent = false;
                        if (fullContent)
                        {
                            ChangeParam = false;
                            Parametr curParam = arrParams[CurChangeParamRow];
                            curParam.Name = parametersDataGridView.Rows[CurChangeParamRow].Cells[0].Value.ToString();
                            curParam.Range = parametersDataGridView.Rows[CurChangeParamRow].Cells[2].Value.ToString();
                            curParam.Number = int.Parse(parametersDataGridView.Rows[CurChangeParamRow].Cells[3].Value.ToString());
                            curParam.Type = (TypeParametr)Enum.Parse(typeof(TypeParametr), (parametersDataGridView.Rows[CurChangeParamRow].Cells[1].Value.ToString()));
                            UpdateParam(curParam);
                            parametersDataGridView.Rows.Clear();
                            ShowAllParams();
                        }
                    }
                }
            }
        }

        private void ParametersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CreateNewTask)
            {
                MainForm.canselCreateTask(SuccessCreate);
            }            
        }
    }
}
