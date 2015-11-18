namespace NeuroWnd
{
    partial class AddChangeNeuroNetDialog
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
            this.tbNameNeuroNet = new System.Windows.Forms.TextBox();
            this.lbNameNeuroNet = new System.Windows.Forms.Label();
            this.cbTask = new System.Windows.Forms.ComboBox();
            this.lbTask = new System.Windows.Forms.Label();
            this.numNeuronsNumber = new System.Windows.Forms.NumericUpDown();
            this.lbNeuronsNumber = new System.Windows.Forms.Label();
            this.lbLayersNumber = new System.Windows.Forms.Label();
            this.numLayersNumber = new System.Windows.Forms.NumericUpDown();
            this.lbActivateFunction = new System.Windows.Forms.Label();
            this.cbActivateFunction = new System.Windows.Forms.ComboBox();
            this.gbNeuroNetDefinition = new System.Windows.Forms.GroupBox();
            this.cbTopology = new System.Windows.Forms.ComboBox();
            this.lbTopology = new System.Windows.Forms.Label();
            this.gbNeuronsInLayers = new System.Windows.Forms.GroupBox();
            this.dgwEditNeuronsInLayers = new System.Windows.Forms.DataGridView();
            this.gbParametersAF = new System.Windows.Forms.GroupBox();
            this.dgwEditParametersAF = new System.Windows.Forms.DataGridView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numNeuronsNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLayersNumber)).BeginInit();
            this.gbNeuroNetDefinition.SuspendLayout();
            this.gbNeuronsInLayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwEditNeuronsInLayers)).BeginInit();
            this.gbParametersAF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwEditParametersAF)).BeginInit();
            this.SuspendLayout();
            // 
            // tbNameNeuroNet
            // 
            this.tbNameNeuroNet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNameNeuroNet.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbNameNeuroNet.Location = new System.Drawing.Point(178, 27);
            this.tbNameNeuroNet.Name = "tbNameNeuroNet";
            this.tbNameNeuroNet.Size = new System.Drawing.Size(232, 24);
            this.tbNameNeuroNet.TabIndex = 0;
            // 
            // lbNameNeuroNet
            // 
            this.lbNameNeuroNet.AutoSize = true;
            this.lbNameNeuroNet.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbNameNeuroNet.Location = new System.Drawing.Point(8, 31);
            this.lbNameNeuroNet.Name = "lbNameNeuroNet";
            this.lbNameNeuroNet.Size = new System.Drawing.Size(141, 17);
            this.lbNameNeuroNet.TabIndex = 1;
            this.lbNameNeuroNet.Text = "Имя Нейронной Сети";
            // 
            // cbTask
            // 
            this.cbTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTask.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbTask.FormattingEnabled = true;
            this.cbTask.Location = new System.Drawing.Point(178, 57);
            this.cbTask.Name = "cbTask";
            this.cbTask.Size = new System.Drawing.Size(232, 25);
            this.cbTask.TabIndex = 2;
            this.cbTask.SelectedIndexChanged += new System.EventHandler(this.cbTask_SelectedIndexChanged);
            // 
            // lbTask
            // 
            this.lbTask.AutoSize = true;
            this.lbTask.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbTask.Location = new System.Drawing.Point(8, 60);
            this.lbTask.Name = "lbTask";
            this.lbTask.Size = new System.Drawing.Size(50, 17);
            this.lbTask.TabIndex = 3;
            this.lbTask.Text = "Задача";
            // 
            // numNeuronsNumber
            // 
            this.numNeuronsNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numNeuronsNumber.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numNeuronsNumber.Location = new System.Drawing.Point(178, 119);
            this.numNeuronsNumber.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numNeuronsNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNeuronsNumber.Name = "numNeuronsNumber";
            this.numNeuronsNumber.Size = new System.Drawing.Size(232, 24);
            this.numNeuronsNumber.TabIndex = 4;
            this.numNeuronsNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbNeuronsNumber
            // 
            this.lbNeuronsNumber.AutoSize = true;
            this.lbNeuronsNumber.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbNeuronsNumber.Location = new System.Drawing.Point(8, 121);
            this.lbNeuronsNumber.Name = "lbNeuronsNumber";
            this.lbNeuronsNumber.Size = new System.Drawing.Size(139, 17);
            this.lbNeuronsNumber.TabIndex = 5;
            this.lbNeuronsNumber.Text = "Число нейронов в НС";
            // 
            // lbLayersNumber
            // 
            this.lbLayersNumber.AutoSize = true;
            this.lbLayersNumber.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbLayersNumber.Location = new System.Drawing.Point(7, 151);
            this.lbLayersNumber.Name = "lbLayersNumber";
            this.lbLayersNumber.Size = new System.Drawing.Size(113, 17);
            this.lbLayersNumber.TabIndex = 7;
            this.lbLayersNumber.Text = "Число слоев в НС";
            // 
            // numLayersNumber
            // 
            this.numLayersNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numLayersNumber.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numLayersNumber.Location = new System.Drawing.Point(178, 149);
            this.numLayersNumber.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numLayersNumber.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numLayersNumber.Name = "numLayersNumber";
            this.numLayersNumber.Size = new System.Drawing.Size(232, 24);
            this.numLayersNumber.TabIndex = 6;
            this.numLayersNumber.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numLayersNumber.ValueChanged += new System.EventHandler(this.numLayersNumber_ValueChanged);
            // 
            // lbActivateFunction
            // 
            this.lbActivateFunction.AutoSize = true;
            this.lbActivateFunction.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbActivateFunction.Location = new System.Drawing.Point(6, 182);
            this.lbActivateFunction.Name = "lbActivateFunction";
            this.lbActivateFunction.Size = new System.Drawing.Size(164, 17);
            this.lbActivateFunction.TabIndex = 9;
            this.lbActivateFunction.Text = "Активационная функция";
            // 
            // cbActivateFunction
            // 
            this.cbActivateFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbActivateFunction.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbActivateFunction.FormattingEnabled = true;
            this.cbActivateFunction.Location = new System.Drawing.Point(178, 179);
            this.cbActivateFunction.Name = "cbActivateFunction";
            this.cbActivateFunction.Size = new System.Drawing.Size(232, 25);
            this.cbActivateFunction.TabIndex = 8;
            this.cbActivateFunction.SelectedIndexChanged += new System.EventHandler(this.cbActivateFunction_SelectedIndexChanged);
            // 
            // gbNeuroNetDefinition
            // 
            this.gbNeuroNetDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbNeuroNetDefinition.Controls.Add(this.cbTopology);
            this.gbNeuroNetDefinition.Controls.Add(this.lbTopology);
            this.gbNeuroNetDefinition.Controls.Add(this.lbActivateFunction);
            this.gbNeuroNetDefinition.Controls.Add(this.cbActivateFunction);
            this.gbNeuroNetDefinition.Controls.Add(this.lbLayersNumber);
            this.gbNeuroNetDefinition.Controls.Add(this.numLayersNumber);
            this.gbNeuroNetDefinition.Controls.Add(this.lbNeuronsNumber);
            this.gbNeuroNetDefinition.Controls.Add(this.numNeuronsNumber);
            this.gbNeuroNetDefinition.Controls.Add(this.lbTask);
            this.gbNeuroNetDefinition.Controls.Add(this.cbTask);
            this.gbNeuroNetDefinition.Controls.Add(this.lbNameNeuroNet);
            this.gbNeuroNetDefinition.Controls.Add(this.tbNameNeuroNet);
            this.gbNeuroNetDefinition.Font = new System.Drawing.Font("Book Antiqua", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gbNeuroNetDefinition.Location = new System.Drawing.Point(5, 12);
            this.gbNeuroNetDefinition.Name = "gbNeuroNetDefinition";
            this.gbNeuroNetDefinition.Size = new System.Drawing.Size(416, 221);
            this.gbNeuroNetDefinition.TabIndex = 10;
            this.gbNeuroNetDefinition.TabStop = false;
            this.gbNeuroNetDefinition.Text = "Определение Нейронной Сети";
            // 
            // cbTopology
            // 
            this.cbTopology.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTopology.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbTopology.FormattingEnabled = true;
            this.cbTopology.Location = new System.Drawing.Point(178, 88);
            this.cbTopology.Name = "cbTopology";
            this.cbTopology.Size = new System.Drawing.Size(232, 25);
            this.cbTopology.TabIndex = 11;
            // 
            // lbTopology
            // 
            this.lbTopology.AutoSize = true;
            this.lbTopology.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbTopology.Location = new System.Drawing.Point(8, 91);
            this.lbTopology.Name = "lbTopology";
            this.lbTopology.Size = new System.Drawing.Size(73, 17);
            this.lbTopology.TabIndex = 10;
            this.lbTopology.Text = "Топология";
            // 
            // gbNeuronsInLayers
            // 
            this.gbNeuronsInLayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbNeuronsInLayers.Controls.Add(this.dgwEditNeuronsInLayers);
            this.gbNeuronsInLayers.Font = new System.Drawing.Font("Book Antiqua", 14F);
            this.gbNeuronsInLayers.Location = new System.Drawing.Point(5, 239);
            this.gbNeuronsInLayers.Name = "gbNeuronsInLayers";
            this.gbNeuronsInLayers.Size = new System.Drawing.Size(416, 155);
            this.gbNeuronsInLayers.TabIndex = 11;
            this.gbNeuronsInLayers.TabStop = false;
            this.gbNeuronsInLayers.Text = "Распределение нейронов по слоям";
            // 
            // dgwEditNeuronsInLayers
            // 
            this.dgwEditNeuronsInLayers.AllowUserToAddRows = false;
            this.dgwEditNeuronsInLayers.AllowUserToDeleteRows = false;
            this.dgwEditNeuronsInLayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwEditNeuronsInLayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgwEditNeuronsInLayers.Location = new System.Drawing.Point(3, 27);
            this.dgwEditNeuronsInLayers.Name = "dgwEditNeuronsInLayers";
            this.dgwEditNeuronsInLayers.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Book Antiqua", 9.75F);
            this.dgwEditNeuronsInLayers.Size = new System.Drawing.Size(410, 125);
            this.dgwEditNeuronsInLayers.TabIndex = 0;
            // 
            // gbParametersAF
            // 
            this.gbParametersAF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbParametersAF.Controls.Add(this.dgwEditParametersAF);
            this.gbParametersAF.Font = new System.Drawing.Font("Book Antiqua", 14F);
            this.gbParametersAF.Location = new System.Drawing.Point(5, 400);
            this.gbParametersAF.Name = "gbParametersAF";
            this.gbParametersAF.Size = new System.Drawing.Size(416, 155);
            this.gbParametersAF.TabIndex = 12;
            this.gbParametersAF.TabStop = false;
            this.gbParametersAF.Text = "Параметры активационной функции";
            // 
            // dgwEditParametersAF
            // 
            this.dgwEditParametersAF.AllowUserToAddRows = false;
            this.dgwEditParametersAF.AllowUserToDeleteRows = false;
            this.dgwEditParametersAF.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwEditParametersAF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgwEditParametersAF.Location = new System.Drawing.Point(3, 27);
            this.dgwEditParametersAF.Name = "dgwEditParametersAF";
            this.dgwEditParametersAF.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Book Antiqua", 9.75F);
            this.dgwEditParametersAF.Size = new System.Drawing.Size(410, 125);
            this.dgwEditParametersAF.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("Book Antiqua", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCancel.Location = new System.Drawing.Point(319, 561);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(99, 30);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.Font = new System.Drawing.Font("Book Antiqua", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAccept.Location = new System.Drawing.Point(214, 561);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(99, 30);
            this.btnAccept.TabIndex = 14;
            this.btnAccept.Text = "Применить";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // AddChangeNeuroNetDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 603);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbParametersAF);
            this.Controls.Add(this.gbNeuronsInLayers);
            this.Controls.Add(this.gbNeuroNetDefinition);
            this.Name = "AddChangeNeuroNetDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ND";
            ((System.ComponentModel.ISupportInitialize)(this.numNeuronsNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLayersNumber)).EndInit();
            this.gbNeuroNetDefinition.ResumeLayout(false);
            this.gbNeuroNetDefinition.PerformLayout();
            this.gbNeuronsInLayers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgwEditNeuronsInLayers)).EndInit();
            this.gbParametersAF.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgwEditParametersAF)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbNameNeuroNet;
        private System.Windows.Forms.Label lbNameNeuroNet;
        private System.Windows.Forms.ComboBox cbTask;
        private System.Windows.Forms.Label lbTask;
        private System.Windows.Forms.NumericUpDown numNeuronsNumber;
        private System.Windows.Forms.Label lbNeuronsNumber;
        private System.Windows.Forms.Label lbLayersNumber;
        private System.Windows.Forms.NumericUpDown numLayersNumber;
        private System.Windows.Forms.Label lbActivateFunction;
        private System.Windows.Forms.ComboBox cbActivateFunction;
        private System.Windows.Forms.GroupBox gbNeuroNetDefinition;
        private System.Windows.Forms.GroupBox gbNeuronsInLayers;
        private System.Windows.Forms.DataGridView dgwEditNeuronsInLayers;
        private System.Windows.Forms.GroupBox gbParametersAF;
        private System.Windows.Forms.DataGridView dgwEditParametersAF;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.ComboBox cbTopology;
        private System.Windows.Forms.Label lbTopology;
    }
}