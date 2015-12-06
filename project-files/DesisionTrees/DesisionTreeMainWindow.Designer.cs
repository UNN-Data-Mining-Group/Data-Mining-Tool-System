namespace DesisionTrees
{
    partial class DesisionTreeMainWindow
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
            this.gbTasksTree = new System.Windows.Forms.GroupBox();
            this.tvTaskSelections = new System.Windows.Forms.TreeView();
            this.gbUsingDesTrees = new System.Windows.Forms.GroupBox();
            this.btnBuildTree = new System.Windows.Forms.Button();
            this.lbLASelected = new System.Windows.Forms.Label();
            this.lbLAInfo = new System.Windows.Forms.Label();
            this.lbTreeSelected = new System.Windows.Forms.Label();
            this.lbTreeInfo = new System.Windows.Forms.Label();
            this.lbSelSelected = new System.Windows.Forms.Label();
            this.lbSelectionInfo = new System.Windows.Forms.Label();
            this.lbTaskSelected = new System.Windows.Forms.Label();
            this.lbTaskInfo = new System.Windows.Forms.Label();
            this.dgwLA = new System.Windows.Forms.DataGridView();
            this.btnLearn = new System.Windows.Forms.Button();
            this.btnUse = new System.Windows.Forms.Button();
            this.dgwTrees = new System.Windows.Forms.DataGridView();
            this.gbTasksTree.SuspendLayout();
            this.gbUsingDesTrees.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwLA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgwTrees)).BeginInit();
            this.SuspendLayout();
            // 
            // gbTasksTree
            // 
            this.gbTasksTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbTasksTree.Controls.Add(this.tvTaskSelections);
            this.gbTasksTree.Font = new System.Drawing.Font("Book Antiqua", 18F);
            this.gbTasksTree.Location = new System.Drawing.Point(12, 12);
            this.gbTasksTree.Name = "gbTasksTree";
            this.gbTasksTree.Size = new System.Drawing.Size(261, 446);
            this.gbTasksTree.TabIndex = 8;
            this.gbTasksTree.TabStop = false;
            this.gbTasksTree.Text = "Дерево задач";
            // 
            // tvTaskSelections
            // 
            this.tvTaskSelections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTaskSelections.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tvTaskSelections.Location = new System.Drawing.Point(3, 33);
            this.tvTaskSelections.Name = "tvTaskSelections";
            this.tvTaskSelections.Size = new System.Drawing.Size(255, 410);
            this.tvTaskSelections.TabIndex = 5;
            this.tvTaskSelections.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvTaskSelections_AfterSelect);
            // 
            // gbUsingDesTrees
            // 
            this.gbUsingDesTrees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbUsingDesTrees.Controls.Add(this.btnBuildTree);
            this.gbUsingDesTrees.Controls.Add(this.lbLASelected);
            this.gbUsingDesTrees.Controls.Add(this.lbLAInfo);
            this.gbUsingDesTrees.Controls.Add(this.lbTreeSelected);
            this.gbUsingDesTrees.Controls.Add(this.lbTreeInfo);
            this.gbUsingDesTrees.Controls.Add(this.lbSelSelected);
            this.gbUsingDesTrees.Controls.Add(this.lbSelectionInfo);
            this.gbUsingDesTrees.Controls.Add(this.lbTaskSelected);
            this.gbUsingDesTrees.Controls.Add(this.lbTaskInfo);
            this.gbUsingDesTrees.Controls.Add(this.dgwLA);
            this.gbUsingDesTrees.Controls.Add(this.btnLearn);
            this.gbUsingDesTrees.Controls.Add(this.btnUse);
            this.gbUsingDesTrees.Controls.Add(this.dgwTrees);
            this.gbUsingDesTrees.Font = new System.Drawing.Font("Book Antiqua", 18F);
            this.gbUsingDesTrees.Location = new System.Drawing.Point(279, 12);
            this.gbUsingDesTrees.Name = "gbUsingDesTrees";
            this.gbUsingDesTrees.Size = new System.Drawing.Size(758, 443);
            this.gbUsingDesTrees.TabIndex = 9;
            this.gbUsingDesTrees.TabStop = false;
            this.gbUsingDesTrees.Text = "Деревья решений";
            // 
            // btnBuildTree
            // 
            this.btnBuildTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuildTree.Font = new System.Drawing.Font("Book Antiqua", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBuildTree.Location = new System.Drawing.Point(276, 126);
            this.btnBuildTree.Name = "btnBuildTree";
            this.btnBuildTree.Size = new System.Drawing.Size(196, 35);
            this.btnBuildTree.TabIndex = 16;
            this.btnBuildTree.Text = "Построить Дерево";
            this.btnBuildTree.UseVisualStyleBackColor = true;
            this.btnBuildTree.Click += new System.EventHandler(this.btnBuildTree_Click);
            // 
            // lbLASelected
            // 
            this.lbLASelected.AutoSize = true;
            this.lbLASelected.Font = new System.Drawing.Font("Book Antiqua", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbLASelected.Location = new System.Drawing.Point(176, 146);
            this.lbLASelected.Name = "lbLASelected";
            this.lbLASelected.Size = new System.Drawing.Size(79, 20);
            this.lbLASelected.TabIndex = 15;
            this.lbLASelected.Text = "Не выбран";
            // 
            // lbLAInfo
            // 
            this.lbLAInfo.AutoSize = true;
            this.lbLAInfo.Font = new System.Drawing.Font("Book Antiqua", 12F);
            this.lbLAInfo.Location = new System.Drawing.Point(5, 146);
            this.lbLAInfo.Name = "lbLAInfo";
            this.lbLAInfo.Size = new System.Drawing.Size(165, 20);
            this.lbLAInfo.TabIndex = 14;
            this.lbLAInfo.Text = "Алгоритм обучения:";
            // 
            // lbTreeSelected
            // 
            this.lbTreeSelected.AutoSize = true;
            this.lbTreeSelected.Font = new System.Drawing.Font("Book Antiqua", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbTreeSelected.Location = new System.Drawing.Point(145, 126);
            this.lbTreeSelected.Name = "lbTreeSelected";
            this.lbTreeSelected.Size = new System.Drawing.Size(86, 20);
            this.lbTreeSelected.TabIndex = 13;
            this.lbTreeSelected.Text = "Не выбрано";
            // 
            // lbTreeInfo
            // 
            this.lbTreeInfo.AutoSize = true;
            this.lbTreeInfo.Font = new System.Drawing.Font("Book Antiqua", 12F);
            this.lbTreeInfo.Location = new System.Drawing.Point(5, 126);
            this.lbTreeInfo.Name = "lbTreeInfo";
            this.lbTreeInfo.Size = new System.Drawing.Size(142, 20);
            this.lbTreeInfo.TabIndex = 12;
            this.lbTreeInfo.Text = "Дерево решений:";
            // 
            // lbSelSelected
            // 
            this.lbSelSelected.AutoSize = true;
            this.lbSelSelected.Font = new System.Drawing.Font("Book Antiqua", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbSelSelected.Location = new System.Drawing.Point(91, 106);
            this.lbSelSelected.Name = "lbSelSelected";
            this.lbSelSelected.Size = new System.Drawing.Size(86, 20);
            this.lbSelSelected.TabIndex = 11;
            this.lbSelSelected.Text = "Не выбрана";
            // 
            // lbSelectionInfo
            // 
            this.lbSelectionInfo.AutoSize = true;
            this.lbSelectionInfo.Font = new System.Drawing.Font("Book Antiqua", 12F);
            this.lbSelectionInfo.Location = new System.Drawing.Point(5, 106);
            this.lbSelectionInfo.Name = "lbSelectionInfo";
            this.lbSelectionInfo.Size = new System.Drawing.Size(80, 20);
            this.lbSelectionInfo.TabIndex = 10;
            this.lbSelectionInfo.Text = "Выборка:";
            // 
            // lbTaskSelected
            // 
            this.lbTaskSelected.AutoSize = true;
            this.lbTaskSelected.Font = new System.Drawing.Font("Book Antiqua", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbTaskSelected.Location = new System.Drawing.Point(75, 86);
            this.lbTaskSelected.Name = "lbTaskSelected";
            this.lbTaskSelected.Size = new System.Drawing.Size(86, 20);
            this.lbTaskSelected.TabIndex = 9;
            this.lbTaskSelected.Text = "Не выбрана";
            // 
            // lbTaskInfo
            // 
            this.lbTaskInfo.AutoSize = true;
            this.lbTaskInfo.Font = new System.Drawing.Font("Book Antiqua", 12F);
            this.lbTaskInfo.Location = new System.Drawing.Point(5, 86);
            this.lbTaskInfo.Name = "lbTaskInfo";
            this.lbTaskInfo.Size = new System.Drawing.Size(64, 20);
            this.lbTaskInfo.TabIndex = 8;
            this.lbTaskInfo.Text = "Задача:";
            // 
            // dgwLA
            // 
            this.dgwLA.AllowUserToAddRows = false;
            this.dgwLA.AllowUserToDeleteRows = false;
            this.dgwLA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgwLA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwLA.Location = new System.Drawing.Point(478, 169);
            this.dgwLA.Name = "dgwLA";
            this.dgwLA.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Book Antiqua", 9.75F);
            this.dgwLA.Size = new System.Drawing.Size(273, 267);
            this.dgwLA.TabIndex = 7;
            // 
            // btnLearn
            // 
            this.btnLearn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLearn.Font = new System.Drawing.Font("Book Antiqua", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLearn.Location = new System.Drawing.Point(478, 126);
            this.btnLearn.Name = "btnLearn";
            this.btnLearn.Size = new System.Drawing.Size(133, 35);
            this.btnLearn.TabIndex = 3;
            this.btnLearn.Text = "Обучить";
            this.btnLearn.UseVisualStyleBackColor = true;
            this.btnLearn.Click += new System.EventHandler(this.btnLearn_Click);
            // 
            // btnUse
            // 
            this.btnUse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUse.Font = new System.Drawing.Font("Book Antiqua", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUse.Location = new System.Drawing.Point(617, 126);
            this.btnUse.Name = "btnUse";
            this.btnUse.Size = new System.Drawing.Size(134, 35);
            this.btnUse.TabIndex = 4;
            this.btnUse.Text = "Использовать";
            this.btnUse.UseVisualStyleBackColor = true;
            this.btnUse.Click += new System.EventHandler(this.btnUse_Click);
            // 
            // dgwTrees
            // 
            this.dgwTrees.AllowUserToAddRows = false;
            this.dgwTrees.AllowUserToDeleteRows = false;
            this.dgwTrees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgwTrees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwTrees.Location = new System.Drawing.Point(6, 169);
            this.dgwTrees.Name = "dgwTrees";
            this.dgwTrees.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dgwTrees.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Book Antiqua", 9.75F);
            this.dgwTrees.Size = new System.Drawing.Size(466, 268);
            this.dgwTrees.TabIndex = 2;
            this.dgwTrees.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgwTrees_CellClick);
            // 
            // DesisionTreeMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 470);
            this.Controls.Add(this.gbUsingDesTrees);
            this.Controls.Add(this.gbTasksTree);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "DesisionTreeMainWindow";
            this.Text = "Деревья решений";
            this.gbTasksTree.ResumeLayout(false);
            this.gbUsingDesTrees.ResumeLayout(false);
            this.gbUsingDesTrees.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwLA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgwTrees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTasksTree;
        private System.Windows.Forms.TreeView tvTaskSelections;
        private System.Windows.Forms.GroupBox gbUsingDesTrees;
        private System.Windows.Forms.Label lbLASelected;
        private System.Windows.Forms.Label lbLAInfo;
        private System.Windows.Forms.Label lbTreeSelected;
        private System.Windows.Forms.Label lbTreeInfo;
        private System.Windows.Forms.Label lbSelSelected;
        private System.Windows.Forms.Label lbSelectionInfo;
        private System.Windows.Forms.Label lbTaskSelected;
        private System.Windows.Forms.Label lbTaskInfo;
        private System.Windows.Forms.DataGridView dgwLA;
        private System.Windows.Forms.Button btnLearn;
        private System.Windows.Forms.Button btnUse;
        private System.Windows.Forms.DataGridView dgwTrees;
        private System.Windows.Forms.Button btnBuildTree;

    }
}

