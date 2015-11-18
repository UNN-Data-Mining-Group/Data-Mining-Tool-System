namespace MainProjectDMTS
{
    partial class MainWindow
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
            this.btnDataBase = new System.Windows.Forms.Button();
            this.btnNeuroWnd = new System.Windows.Forms.Button();
            this.btnDesisionTrees = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDataBase
            // 
            this.btnDataBase.Location = new System.Drawing.Point(12, 12);
            this.btnDataBase.Name = "btnDataBase";
            this.btnDataBase.Size = new System.Drawing.Size(143, 35);
            this.btnDataBase.TabIndex = 0;
            this.btnDataBase.Text = "Работа с БД";
            this.btnDataBase.UseVisualStyleBackColor = true;
            this.btnDataBase.Click += new System.EventHandler(this.btnDataBase_Click);
            // 
            // btnNeuroWnd
            // 
            this.btnNeuroWnd.Location = new System.Drawing.Point(12, 53);
            this.btnNeuroWnd.Name = "btnNeuroWnd";
            this.btnNeuroWnd.Size = new System.Drawing.Size(143, 35);
            this.btnNeuroWnd.TabIndex = 1;
            this.btnNeuroWnd.Text = "Нейронные сети";
            this.btnNeuroWnd.UseVisualStyleBackColor = true;
            this.btnNeuroWnd.Click += new System.EventHandler(this.btnNeuroWnd_Click);
            // 
            // btnDesisionTrees
            // 
            this.btnDesisionTrees.Location = new System.Drawing.Point(12, 94);
            this.btnDesisionTrees.Name = "btnDesisionTrees";
            this.btnDesisionTrees.Size = new System.Drawing.Size(143, 35);
            this.btnDesisionTrees.TabIndex = 2;
            this.btnDesisionTrees.Text = "Деревья решений";
            this.btnDesisionTrees.UseVisualStyleBackColor = true;
            this.btnDesisionTrees.Click += new System.EventHandler(this.btnDesisionTrees_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 140);
            this.Controls.Add(this.btnDesisionTrees);
            this.Controls.Add(this.btnNeuroWnd);
            this.Controls.Add(this.btnDataBase);
            this.Name = "MainWindow";
            this.Text = "Инструментальная система интеллектуального анализа данных";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDataBase;
        private System.Windows.Forms.Button btnNeuroWnd;
        private System.Windows.Forms.Button btnDesisionTrees;
    }
}

