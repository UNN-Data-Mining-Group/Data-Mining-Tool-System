namespace DesisionTrees
{
    partial class TreeUsingForm
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
            this.gbInputVal = new System.Windows.Forms.GroupBox();
            this.dgwInputVal = new System.Windows.Forms.DataGridView();
            this.btnSolve = new System.Windows.Forms.Button();
            this.lblAnswer = new System.Windows.Forms.Label();
            this.gbInputVal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwInputVal)).BeginInit();
            this.SuspendLayout();
            // 
            // gbInputVal
            // 
            this.gbInputVal.Controls.Add(this.dgwInputVal);
            this.gbInputVal.Location = new System.Drawing.Point(12, 12);
            this.gbInputVal.Name = "gbInputVal";
            this.gbInputVal.Size = new System.Drawing.Size(425, 124);
            this.gbInputVal.TabIndex = 0;
            this.gbInputVal.TabStop = false;
            this.gbInputVal.Text = "Входные значения";
            // 
            // dgwInputVal
            // 
            this.dgwInputVal.AllowUserToAddRows = false;
            this.dgwInputVal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwInputVal.Location = new System.Drawing.Point(6, 19);
            this.dgwInputVal.Name = "dgwInputVal";
            this.dgwInputVal.Size = new System.Drawing.Size(413, 93);
            this.dgwInputVal.TabIndex = 0;
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(490, 31);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(188, 37);
            this.btnSolve.TabIndex = 1;
            this.btnSolve.Text = "Получить ответ";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // lblAnswer
            // 
            this.lblAnswer.AutoSize = true;
            this.lblAnswer.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblAnswer.Location = new System.Drawing.Point(487, 84);
            this.lblAnswer.Name = "lblAnswer";
            this.lblAnswer.Size = new System.Drawing.Size(90, 29);
            this.lblAnswer.TabIndex = 2;
            this.lblAnswer.Text = "lsdadw";
            this.lblAnswer.Visible = false;
            // 
            // TreeUsingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 147);
            this.Controls.Add(this.lblAnswer);
            this.Controls.Add(this.btnSolve);
            this.Controls.Add(this.gbInputVal);
            this.Name = "TreeUsingForm";
            this.Text = "TreeUsingForm";
            this.gbInputVal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgwInputVal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbInputVal;
        private System.Windows.Forms.DataGridView dgwInputVal;
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.Label lblAnswer;
    }
}