namespace SII
{
    partial class ParametersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParametersForm));
            this.parametersDataGridView = new System.Windows.Forms.DataGridView();
            this.allParamsLB = new System.Windows.Forms.Label();
            this.NameParametr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Range = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Change = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.parametersDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // parametersDataGridView
            // 
            resources.ApplyResources(this.parametersDataGridView, "parametersDataGridView");
            this.parametersDataGridView.AllowUserToAddRows = false;
            this.parametersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.parametersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameParametr,
            this.Type,
            this.Range,
            this.Index,
            this.Change});
            this.parametersDataGridView.Name = "parametersDataGridView";
            this.parametersDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.parametersDataGridView_CellClick);
            // 
            // allParamsLB
            // 
            resources.ApplyResources(this.allParamsLB, "allParamsLB");
            this.allParamsLB.Name = "allParamsLB";
            // 
            // NameParametr
            // 
            resources.ApplyResources(this.NameParametr, "NameParametr");
            this.NameParametr.Name = "NameParametr";
            // 
            // Type
            // 
            resources.ApplyResources(this.Type, "Type");
            this.Type.Name = "Type";
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Range
            // 
            resources.ApplyResources(this.Range, "Range");
            this.Range.Name = "Range";
            // 
            // Index
            // 
            resources.ApplyResources(this.Index, "Index");
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            // 
            // Change
            // 
            resources.ApplyResources(this.Change, "Change");
            this.Change.Name = "Change";
            this.Change.Text = "Изменить";
            // 
            // ParametersForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.allParamsLB);
            this.Controls.Add(this.parametersDataGridView);
            this.Name = "ParametersForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ParametersForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.parametersDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView parametersDataGridView;
        private System.Windows.Forms.Label allParamsLB;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameParametr;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Range;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewButtonColumn Change;
    }
}