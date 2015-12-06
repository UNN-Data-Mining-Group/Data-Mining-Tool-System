namespace NeuroWnd
{
    partial class NeuroNetSolvingWindow
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
            this.gbInputs = new System.Windows.Forms.GroupBox();
            this.gbOutputs = new System.Windows.Forms.GroupBox();
            this.btnSolve = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gbInputs
            // 
            this.gbInputs.Location = new System.Drawing.Point(12, 12);
            this.gbInputs.Name = "gbInputs";
            this.gbInputs.Size = new System.Drawing.Size(246, 128);
            this.gbInputs.TabIndex = 0;
            this.gbInputs.TabStop = false;
            this.gbInputs.Text = "Входные значения";
            // 
            // gbOutputs
            // 
            this.gbOutputs.Location = new System.Drawing.Point(264, 12);
            this.gbOutputs.Name = "gbOutputs";
            this.gbOutputs.Size = new System.Drawing.Size(246, 128);
            this.gbOutputs.TabIndex = 1;
            this.gbOutputs.TabStop = false;
            this.gbOutputs.Text = "Выходные значения";
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(516, 12);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(131, 36);
            this.btnSolve.TabIndex = 2;
            this.btnSolve.Text = "Решить";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // NeuroNetSolvingWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 151);
            this.Controls.Add(this.btnSolve);
            this.Controls.Add(this.gbOutputs);
            this.Controls.Add(this.gbInputs);
            this.Name = "NeuroNetSolvingWindow";
            this.ShowIcon = false;
            this.Text = "Решение задачи";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbInputs;
        private System.Windows.Forms.GroupBox gbOutputs;
        private System.Windows.Forms.Button btnSolve;
    }
}