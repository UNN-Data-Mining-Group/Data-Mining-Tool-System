namespace SII
{
    partial class TaskForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.createNewTaskBtn = new System.Windows.Forms.Button();
            this.tasksDataGridView = new System.Windows.Forms.DataGridView();
            this.NameTask = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountParameters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountSelection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChangeParameters = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Selections = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Change = new System.Windows.Forms.DataGridViewButtonColumn();
            this.parametersGroupBox = new System.Windows.Forms.GroupBox();
            this.parametersDataGridView = new System.Windows.Forms.DataGridView();
            this.NameParametr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Range = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.creationEndBtn = new System.Windows.Forms.Button();
            this.selectionsGroupBox = new System.Windows.Forms.GroupBox();
            this.addedSelectionEndBtn = new System.Windows.Forms.Button();
            this.removeSelectionBtn = new System.Windows.Forms.Button();
            this.addSelectionBtn = new System.Windows.Forms.Button();
            this.selectionsDataGridView = new System.Windows.Forms.DataGridView();
            this.NameSelection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountRows = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChangeButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.deleteTaskBtn = new System.Windows.Forms.Button();
            this.tasksGroupBox = new System.Windows.Forms.GroupBox();
            this.valuesGroupBox = new System.Windows.Forms.GroupBox();
            this.valuesDataGridView = new System.Windows.Forms.DataGridView();
            this.ValueNameParametr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueNameSelection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.tasksDataGridView)).BeginInit();
            this.parametersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parametersDataGridView)).BeginInit();
            this.selectionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectionsDataGridView)).BeginInit();
            this.tasksGroupBox.SuspendLayout();
            this.valuesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valuesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // createNewTaskBtn
            // 
            this.createNewTaskBtn.Location = new System.Drawing.Point(354, 19);
            this.createNewTaskBtn.Name = "createNewTaskBtn";
            this.createNewTaskBtn.Size = new System.Drawing.Size(72, 23);
            this.createNewTaskBtn.TabIndex = 0;
            this.createNewTaskBtn.Text = "Добавить";
            this.createNewTaskBtn.UseVisualStyleBackColor = true;
            this.createNewTaskBtn.Click += new System.EventHandler(this.createNewTaskBtn_Click);
            // 
            // tasksDataGridView
            // 
            this.tasksDataGridView.AllowUserToAddRows = false;
            this.tasksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tasksDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameTask,
            this.CountParameters,
            this.CountSelection,
            this.ChangeParameters,
            this.Selections,
            this.Change});
            this.tasksDataGridView.Location = new System.Drawing.Point(19, 19);
            this.tasksDataGridView.Name = "tasksDataGridView";
            this.tasksDataGridView.Size = new System.Drawing.Size(324, 269);
            this.tasksDataGridView.TabIndex = 10;
            this.tasksDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tasksDataGridView_CellClick);
            this.tasksDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.tasksDataGridView_CellEndEdit);
            this.tasksDataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.tasksDataGridView_RowHeaderMouseClick);
            // 
            // NameTask
            // 
            this.NameTask.HeaderText = "Имя";
            this.NameTask.Name = "NameTask";
            // 
            // CountParameters
            // 
            this.CountParameters.HeaderText = "Количество параметров";
            this.CountParameters.Name = "CountParameters";
            this.CountParameters.Width = 70;
            // 
            // CountSelection
            // 
            this.CountSelection.HeaderText = "Количество выборок";
            this.CountSelection.Name = "CountSelection";
            this.CountSelection.Width = 70;
            // 
            // ChangeParameters
            // 
            this.ChangeParameters.HeaderText = "Параметры";
            this.ChangeParameters.Name = "ChangeParameters";
            this.ChangeParameters.Text = "Изменить";
            this.ChangeParameters.Visible = false;
            this.ChangeParameters.Width = 65;
            // 
            // Selections
            // 
            this.Selections.HeaderText = "Выборки";
            this.Selections.Name = "Selections";
            this.Selections.Text = "Изменить";
            this.Selections.Visible = false;
            this.Selections.Width = 55;
            // 
            // Change
            // 
            this.Change.HeaderText = "Изменение";
            this.Change.Name = "Change";
            this.Change.Text = "Изменение";
            this.Change.Visible = false;
            this.Change.Width = 65;
            // 
            // parametersGroupBox
            // 
            this.parametersGroupBox.Controls.Add(this.parametersDataGridView);
            this.parametersGroupBox.Location = new System.Drawing.Point(5, 338);
            this.parametersGroupBox.Name = "parametersGroupBox";
            this.parametersGroupBox.Size = new System.Drawing.Size(432, 213);
            this.parametersGroupBox.TabIndex = 12;
            this.parametersGroupBox.TabStop = false;
            this.parametersGroupBox.Text = "Параметры";
            // 
            // parametersDataGridView
            // 
            this.parametersDataGridView.AllowUserToAddRows = false;
            this.parametersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.parametersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameParametr,
            this.Type,
            this.Range,
            this.Index,
            this.dataGridViewButtonColumn1});
            this.parametersDataGridView.Location = new System.Drawing.Point(6, 19);
            this.parametersDataGridView.Name = "parametersDataGridView";
            this.parametersDataGridView.Size = new System.Drawing.Size(335, 188);
            this.parametersDataGridView.TabIndex = 1;
            // 
            // NameParametr
            // 
            this.NameParametr.HeaderText = "Имя";
            this.NameParametr.Name = "NameParametr";
            // 
            // Type
            // 
            this.Type.HeaderText = "Тип";
            this.Type.Name = "Type";
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Type.Width = 70;
            // 
            // Range
            // 
            this.Range.HeaderText = "Границы";
            this.Range.Name = "Range";
            this.Range.Width = 70;
            // 
            // Index
            // 
            this.Index.HeaderText = "Индекс";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Width = 50;
            // 
            // dataGridViewButtonColumn1
            // 
            this.dataGridViewButtonColumn1.HeaderText = "Change";
            this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            this.dataGridViewButtonColumn1.Text = "Изменить";
            this.dataGridViewButtonColumn1.Visible = false;
            this.dataGridViewButtonColumn1.Width = 50;
            // 
            // creationEndBtn
            // 
            this.creationEndBtn.Enabled = false;
            this.creationEndBtn.Location = new System.Drawing.Point(355, 77);
            this.creationEndBtn.Name = "creationEndBtn";
            this.creationEndBtn.Size = new System.Drawing.Size(71, 23);
            this.creationEndBtn.TabIndex = 13;
            this.creationEndBtn.Text = "ОК";
            this.creationEndBtn.UseVisualStyleBackColor = true;
            this.creationEndBtn.Click += new System.EventHandler(this.creationEndBtn_Click);
            // 
            // selectionsGroupBox
            // 
            this.selectionsGroupBox.Controls.Add(this.addedSelectionEndBtn);
            this.selectionsGroupBox.Controls.Add(this.removeSelectionBtn);
            this.selectionsGroupBox.Controls.Add(this.addSelectionBtn);
            this.selectionsGroupBox.Controls.Add(this.selectionsDataGridView);
            this.selectionsGroupBox.Location = new System.Drawing.Point(469, 12);
            this.selectionsGroupBox.Name = "selectionsGroupBox";
            this.selectionsGroupBox.Size = new System.Drawing.Size(347, 232);
            this.selectionsGroupBox.TabIndex = 14;
            this.selectionsGroupBox.TabStop = false;
            this.selectionsGroupBox.Text = "Выборки";
            // 
            // addedSelectionEndBtn
            // 
            this.addedSelectionEndBtn.Enabled = false;
            this.addedSelectionEndBtn.Location = new System.Drawing.Point(240, 77);
            this.addedSelectionEndBtn.Name = "addedSelectionEndBtn";
            this.addedSelectionEndBtn.Size = new System.Drawing.Size(75, 23);
            this.addedSelectionEndBtn.TabIndex = 4;
            this.addedSelectionEndBtn.Text = "ОК";
            this.addedSelectionEndBtn.UseVisualStyleBackColor = true;
            this.addedSelectionEndBtn.Click += new System.EventHandler(this.addedEndBtn_Click);
            // 
            // removeSelectionBtn
            // 
            this.removeSelectionBtn.Location = new System.Drawing.Point(240, 48);
            this.removeSelectionBtn.Name = "removeSelectionBtn";
            this.removeSelectionBtn.Size = new System.Drawing.Size(75, 23);
            this.removeSelectionBtn.TabIndex = 3;
            this.removeSelectionBtn.Text = "Удалить";
            this.removeSelectionBtn.UseVisualStyleBackColor = true;
            this.removeSelectionBtn.Click += new System.EventHandler(this.removeSelectionBtn_Click);
            // 
            // addSelectionBtn
            // 
            this.addSelectionBtn.Location = new System.Drawing.Point(240, 19);
            this.addSelectionBtn.Name = "addSelectionBtn";
            this.addSelectionBtn.Size = new System.Drawing.Size(75, 23);
            this.addSelectionBtn.TabIndex = 2;
            this.addSelectionBtn.Text = "Добавить";
            this.addSelectionBtn.UseVisualStyleBackColor = true;
            this.addSelectionBtn.Click += new System.EventHandler(this.addSelectionBtn_Click);
            // 
            // selectionsDataGridView
            // 
            this.selectionsDataGridView.AllowUserToAddRows = false;
            this.selectionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.selectionsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameSelection,
            this.CountRows,
            this.ChangeButton});
            this.selectionsDataGridView.Location = new System.Drawing.Point(19, 15);
            this.selectionsDataGridView.Name = "selectionsDataGridView";
            this.selectionsDataGridView.Size = new System.Drawing.Size(215, 177);
            this.selectionsDataGridView.TabIndex = 1;
            this.selectionsDataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.selectionsDataGridView_RowHeaderMouseClick);
            // 
            // NameSelection
            // 
            this.NameSelection.HeaderText = "Имя";
            this.NameSelection.Name = "NameSelection";
            // 
            // CountRows
            // 
            this.CountRows.HeaderText = "Количество записей";
            this.CountRows.Name = "CountRows";
            this.CountRows.ReadOnly = true;
            this.CountRows.Width = 70;
            // 
            // ChangeButton
            // 
            this.ChangeButton.HeaderText = "Изменить";
            this.ChangeButton.Name = "ChangeButton";
            this.ChangeButton.Visible = false;
            this.ChangeButton.Width = 70;
            // 
            // deleteTaskBtn
            // 
            this.deleteTaskBtn.Location = new System.Drawing.Point(354, 48);
            this.deleteTaskBtn.Name = "deleteTaskBtn";
            this.deleteTaskBtn.Size = new System.Drawing.Size(71, 23);
            this.deleteTaskBtn.TabIndex = 15;
            this.deleteTaskBtn.Text = "Удалить";
            this.deleteTaskBtn.UseVisualStyleBackColor = true;
            this.deleteTaskBtn.Click += new System.EventHandler(this.deleteTaskBtn_Click);
            // 
            // tasksGroupBox
            // 
            this.tasksGroupBox.Controls.Add(this.tasksDataGridView);
            this.tasksGroupBox.Controls.Add(this.deleteTaskBtn);
            this.tasksGroupBox.Controls.Add(this.creationEndBtn);
            this.tasksGroupBox.Controls.Add(this.createNewTaskBtn);
            this.tasksGroupBox.Location = new System.Drawing.Point(5, 12);
            this.tasksGroupBox.Name = "tasksGroupBox";
            this.tasksGroupBox.Size = new System.Drawing.Size(432, 308);
            this.tasksGroupBox.TabIndex = 16;
            this.tasksGroupBox.TabStop = false;
            this.tasksGroupBox.Text = "Задачи";
            // 
            // valuesGroupBox
            // 
            this.valuesGroupBox.Controls.Add(this.valuesDataGridView);
            this.valuesGroupBox.Location = new System.Drawing.Point(469, 250);
            this.valuesGroupBox.Name = "valuesGroupBox";
            this.valuesGroupBox.Size = new System.Drawing.Size(347, 320);
            this.valuesGroupBox.TabIndex = 17;
            this.valuesGroupBox.TabStop = false;
            this.valuesGroupBox.Text = "Значения выборки";
            // 
            // valuesDataGridView
            // 
            this.valuesDataGridView.AllowUserToAddRows = false;
            this.valuesDataGridView.AllowUserToDeleteRows = false;
            this.valuesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.valuesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ValueNameParametr,
            this.Value,
            this.ValueNameSelection});
            this.valuesDataGridView.Location = new System.Drawing.Point(6, 29);
            this.valuesDataGridView.Name = "valuesDataGridView";
            this.valuesDataGridView.ReadOnly = true;
            this.valuesDataGridView.Size = new System.Drawing.Size(335, 272);
            this.valuesDataGridView.TabIndex = 0;
            // 
            // ValueNameParametr
            // 
            this.ValueNameParametr.HeaderText = "Название параметра";
            this.ValueNameParametr.Name = "ValueNameParametr";
            this.ValueNameParametr.ReadOnly = true;
            this.ValueNameParametr.Visible = false;
            this.ValueNameParametr.Width = 80;
            // 
            // Value
            // 
            this.Value.HeaderText = "Значение";
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            this.Value.Visible = false;
            this.Value.Width = 80;
            // 
            // ValueNameSelection
            // 
            this.ValueNameSelection.HeaderText = "Название выборки";
            this.ValueNameSelection.Name = "ValueNameSelection";
            this.ValueNameSelection.ReadOnly = true;
            this.ValueNameSelection.Visible = false;
            this.ValueNameSelection.Width = 80;
            // 
            // TaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 588);
            this.Controls.Add(this.valuesGroupBox);
            this.Controls.Add(this.selectionsGroupBox);
            this.Controls.Add(this.tasksGroupBox);
            this.Controls.Add(this.parametersGroupBox);
            this.Name = "TaskForm";
            this.Text = "Задача";
            ((System.ComponentModel.ISupportInitialize)(this.tasksDataGridView)).EndInit();
            this.parametersGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.parametersDataGridView)).EndInit();
            this.selectionsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.selectionsDataGridView)).EndInit();
            this.tasksGroupBox.ResumeLayout(false);
            this.valuesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.valuesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button createNewTaskBtn;
        private System.Windows.Forms.DataGridView tasksDataGridView;
        private System.Windows.Forms.GroupBox parametersGroupBox;
        private System.Windows.Forms.DataGridView parametersDataGridView;
        private System.Windows.Forms.Button creationEndBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameTask;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountSelection;
        private System.Windows.Forms.DataGridViewButtonColumn ChangeParameters;
        private System.Windows.Forms.DataGridViewButtonColumn Selections;
        private System.Windows.Forms.DataGridViewButtonColumn Change;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameParametr;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Range;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn1;
        private System.Windows.Forms.GroupBox selectionsGroupBox;
        private System.Windows.Forms.DataGridView selectionsDataGridView;
        private System.Windows.Forms.Button deleteTaskBtn;
        private System.Windows.Forms.Button removeSelectionBtn;
        private System.Windows.Forms.Button addSelectionBtn;
        private System.Windows.Forms.Button addedSelectionEndBtn;
        private System.Windows.Forms.GroupBox tasksGroupBox;
        private System.Windows.Forms.GroupBox valuesGroupBox;
        private System.Windows.Forms.DataGridView valuesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameSelection;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountRows;
        private System.Windows.Forms.DataGridViewButtonColumn ChangeButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueNameParametr;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueNameSelection;
    }
}

