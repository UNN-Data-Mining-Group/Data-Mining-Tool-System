namespace SII
{
    partial class SelectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.selectionsDataGridView = new System.Windows.Forms.DataGridView();
            this.NameSelection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountRows = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChangeButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.allSelectionLB = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.selectionsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // selectionsDataGridView
            // 
            this.selectionsDataGridView.AllowUserToAddRows = false;
            this.selectionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.selectionsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameSelection,
            this.CountRows,
            this.ChangeButton});
            this.selectionsDataGridView.Location = new System.Drawing.Point(26, 43);
            this.selectionsDataGridView.Name = "selectionsDataGridView";
            this.selectionsDataGridView.Size = new System.Drawing.Size(284, 177);
            this.selectionsDataGridView.TabIndex = 0;
            this.selectionsDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.selectionsDataGridView_CellClick);
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
            this.CountRows.Width = 70;
            // 
            // ChangeButton
            // 
            this.ChangeButton.HeaderText = "Изменить";
            this.ChangeButton.Name = "ChangeButton";
            this.ChangeButton.Width = 70;
            // 
            // allSelectionLB
            // 
            this.allSelectionLB.AutoSize = true;
            this.allSelectionLB.Location = new System.Drawing.Point(26, 24);
            this.allSelectionLB.Name = "allSelectionLB";
            this.allSelectionLB.Size = new System.Drawing.Size(73, 13);
            this.allSelectionLB.TabIndex = 1;
            this.allSelectionLB.Text = "Все выборки";
            // 
            // SelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 354);
            this.Controls.Add(this.allSelectionLB);
            this.Controls.Add(this.selectionsDataGridView);
            this.Name = "SelectionForm";
            this.Text = "Выборки";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SelectionForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.selectionsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView selectionsDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameSelection;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountRows;
        private System.Windows.Forms.DataGridViewButtonColumn ChangeButton;
        private System.Windows.Forms.Label allSelectionLB;
    }
}