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
    public partial class SelectionForm : Form
    {
        public TaskForm MainForm;

        private SQLManager sqlManager;

        int TaskID;
        bool CreateNewTask;
        public int CountSelections;

        List<Selection> arrSelections;

        bool ChangeSelection = false;
        int CurChangeSelectionRow;

        bool SuccessCreate = false;

        public SelectionForm()
        {
            InitializeComponent();
            sqlManager = SQLManager.MainSQLManager;
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
                for (int i = 0; i < CountSelections; i++)
                {
                    if (i == CountSelections - 1)
                        lastColumnValue = "Создать!";
                    if (i == CountSelections - 2)
                        lastColumnValue = "Отмена";
                    row = new string[] { "", "", lastColumnValue };
                    selectionsDataGridView.Rows.Add(row);
                }
            }
            else
            {
                ShowAllSelections();
            }
        }

        private void ShowAllSelections()
        {
            arrSelections = sqlManager.GetSelectionsWithRequest(String.Format("select * from SELECTION where TASK_ID='{0}';", TaskID));
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

        private void UpdateSelection(Selection newSelection)
        {
            String sqlReqStr = "UPDATE SELECTION SET NAME='"+newSelection.Name+"',COUNT_ROWS='"+ newSelection.CountRows + "' " +
                "WHERE TASK_ID='" + newSelection.TaskID + "' and ID='" + newSelection.ID +"';";
            sqlManager.SendUpdateRequest(sqlReqStr);
        }

        private void createNewSelection(String name, String countRows)
        {
            String sqlReqStr = "INSERT INTO SELECTION (TASK_ID, NAME, COUNT_ROWS) " +
                "VALUES('" + TaskID + "','" + name + "','" + countRows + "');";
            int state = sqlManager.SendInsertRequest(sqlReqStr);
            if (state == 0)
                Console.WriteLine("error");
        }

        private void selectionsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CreateNewTask)
            {
                //если создается новая задача
                if (e.ColumnIndex == 2 && e.RowIndex == CountSelections - 1)
                {
                    bool fullContent = true;
                    foreach (DataGridViewRow row in selectionsDataGridView.Rows)
                    {
                        for (int i = 0; i < 2; i++)
                            if (row.Cells[i].Value.ToString() == "")
                                fullContent = false;
                    }
                    if (fullContent)
                    {
                        //отсылаем в бд, закрываем форму, уведомляем о успешном создании
                        foreach (DataGridViewRow row in selectionsDataGridView.Rows)
                        {
                            createNewSelection(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
                        }
                        SuccessCreate = true;
                        this.Close();
                    }
                }
                if (e.ColumnIndex == 2 && e.RowIndex == CountSelections - 2)
                {
                    this.Close();
                }
            }
            else
            {
                //если хотим обновить
                if (!ChangeSelection)
                {
                    if (e.ColumnIndex == 2)
                    {
                        Selection selection = arrSelections[e.RowIndex];
                        selectionsDataGridView.Rows[e.RowIndex].ReadOnly = false;
                        selectionsDataGridView.Rows[e.RowIndex].Cells[2].Value = "Сохранить";
                        ChangeSelection = true;
                        CurChangeSelectionRow = e.RowIndex;
                    }
                }
                else
                {
                    if (e.ColumnIndex == 2 && e.RowIndex == CurChangeSelectionRow)
                    {
                        //Save changes
                        bool fullContent = true;
                        for (int i = 0; i < 2; i++)
                            if (selectionsDataGridView.Rows[CurChangeSelectionRow].Cells[i].Value.ToString() == "")
                                fullContent = false;
                        if (fullContent)
                        {
                            ChangeSelection = false;
                            Selection curSelection = arrSelections[CurChangeSelectionRow];
                            curSelection.Name = selectionsDataGridView.Rows[CurChangeSelectionRow].Cells[0].Value.ToString();
                            curSelection.CountRows = Int32.Parse(selectionsDataGridView.Rows[CurChangeSelectionRow].Cells[1].Value.ToString());
                            UpdateSelection(curSelection);
                            selectionsDataGridView.Rows.Clear();
                            ShowAllSelections();
                        }                        
                    }
                }                
            }
        }

        private void SelectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CreateNewTask)
            {
                MainForm.canselCreateTask(SuccessCreate);
            }
        }
    }
}
