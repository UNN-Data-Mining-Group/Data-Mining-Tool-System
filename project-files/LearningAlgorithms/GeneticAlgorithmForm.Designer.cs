namespace LearningAlgorithms
{
    partial class GeneticAlgorithmForm
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
            this.GB_gen_par = new System.Windows.Forms.GroupBox();
            this.CB_lin_repr = new System.Windows.Forms.CheckBox();
            this.TB_max_step = new System.Windows.Forms.TextBox();
            this.LB_max_step = new System.Windows.Forms.Label();
            this.TB_selection_percent = new System.Windows.Forms.TextBox();
            this.LB_selection_percent = new System.Windows.Forms.Label();
            this.TB_train_percent = new System.Windows.Forms.TextBox();
            this.LB_train_pecent = new System.Windows.Forms.Label();
            this.TB_coef_mut = new System.Windows.Forms.TextBox();
            this.LB_coef_mut = new System.Windows.Forms.Label();
            this.TB_count_popul = new System.Windows.Forms.TextBox();
            this.LB_count_popul = new System.Windows.Forms.Label();
            this.TB_eps = new System.Windows.Forms.TextBox();
            this.LB_eps = new System.Windows.Forms.Label();
            this.BT_learn = new System.Windows.Forms.Button();
            this.GB_res = new System.Windows.Forms.GroupBox();
            this.TB_res_count_step = new System.Windows.Forms.TextBox();
            this.LB_res_count_step = new System.Windows.Forms.Label();
            this.TB_res_eps = new System.Windows.Forms.TextBox();
            this.LB_res_eps = new System.Windows.Forms.Label();
            this.LB_err = new System.Windows.Forms.Label();
            this.LB_max_err = new System.Windows.Forms.Label();
            this.LB_min_err = new System.Windows.Forms.Label();
            this.BT_write = new System.Windows.Forms.Button();
            this.GB_gen_par.SuspendLayout();
            this.GB_res.SuspendLayout();
            this.SuspendLayout();
            // 
            // GB_gen_par
            // 
            this.GB_gen_par.Controls.Add(this.CB_lin_repr);
            this.GB_gen_par.Controls.Add(this.TB_max_step);
            this.GB_gen_par.Controls.Add(this.LB_max_step);
            this.GB_gen_par.Controls.Add(this.TB_selection_percent);
            this.GB_gen_par.Controls.Add(this.LB_selection_percent);
            this.GB_gen_par.Controls.Add(this.TB_train_percent);
            this.GB_gen_par.Controls.Add(this.LB_train_pecent);
            this.GB_gen_par.Controls.Add(this.TB_coef_mut);
            this.GB_gen_par.Controls.Add(this.LB_coef_mut);
            this.GB_gen_par.Controls.Add(this.TB_count_popul);
            this.GB_gen_par.Controls.Add(this.LB_count_popul);
            this.GB_gen_par.Controls.Add(this.TB_eps);
            this.GB_gen_par.Controls.Add(this.LB_eps);
            this.GB_gen_par.Location = new System.Drawing.Point(12, 12);
            this.GB_gen_par.Name = "GB_gen_par";
            this.GB_gen_par.Size = new System.Drawing.Size(340, 202);
            this.GB_gen_par.TabIndex = 0;
            this.GB_gen_par.TabStop = false;
            this.GB_gen_par.Text = "Параметры Генетического алгоритма";
            // 
            // CB_lin_repr
            // 
            this.CB_lin_repr.AutoSize = true;
            this.CB_lin_repr.Location = new System.Drawing.Point(13, 179);
            this.CB_lin_repr.Name = "CB_lin_repr";
            this.CB_lin_repr.Size = new System.Drawing.Size(144, 17);
            this.CB_lin_repr.TabIndex = 12;
            this.CB_lin_repr.Text = "Линейная репродукция";
            this.CB_lin_repr.UseVisualStyleBackColor = true;
            // 
            // TB_max_step
            // 
            this.TB_max_step.Location = new System.Drawing.Point(209, 149);
            this.TB_max_step.Name = "TB_max_step";
            this.TB_max_step.Size = new System.Drawing.Size(100, 20);
            this.TB_max_step.TabIndex = 11;
            this.TB_max_step.Text = "100";
            // 
            // LB_max_step
            // 
            this.LB_max_step.AutoSize = true;
            this.LB_max_step.Location = new System.Drawing.Point(9, 152);
            this.LB_max_step.Name = "LB_max_step";
            this.LB_max_step.Size = new System.Drawing.Size(150, 13);
            this.LB_max_step.TabIndex = 10;
            this.LB_max_step.Text = "Максимальное число шагов";
            // 
            // TB_selection_percent
            // 
            this.TB_selection_percent.Location = new System.Drawing.Point(209, 123);
            this.TB_selection_percent.Name = "TB_selection_percent";
            this.TB_selection_percent.Size = new System.Drawing.Size(100, 20);
            this.TB_selection_percent.TabIndex = 9;
            this.TB_selection_percent.Text = "3E-001";
            // 
            // LB_selection_percent
            // 
            this.LB_selection_percent.AutoSize = true;
            this.LB_selection_percent.Location = new System.Drawing.Point(9, 126);
            this.LB_selection_percent.Name = "LB_selection_percent";
            this.LB_selection_percent.Size = new System.Drawing.Size(122, 13);
            this.LB_selection_percent.TabIndex = 8;
            this.LB_selection_percent.Text = "Процент скрещивания";
            // 
            // TB_train_percent
            // 
            this.TB_train_percent.Location = new System.Drawing.Point(209, 97);
            this.TB_train_percent.Name = "TB_train_percent";
            this.TB_train_percent.Size = new System.Drawing.Size(100, 20);
            this.TB_train_percent.TabIndex = 7;
            this.TB_train_percent.Text = "1E-001";
            // 
            // LB_train_pecent
            // 
            this.LB_train_pecent.AutoSize = true;
            this.LB_train_pecent.Location = new System.Drawing.Point(9, 100);
            this.LB_train_pecent.Name = "LB_train_pecent";
            this.LB_train_pecent.Size = new System.Drawing.Size(194, 13);
            this.LB_train_pecent.TabIndex = 6;
            this.LB_train_pecent.Text = "Процент обучающей выборки на шаг";
            // 
            // TB_coef_mut
            // 
            this.TB_coef_mut.Location = new System.Drawing.Point(209, 71);
            this.TB_coef_mut.Name = "TB_coef_mut";
            this.TB_coef_mut.Size = new System.Drawing.Size(100, 20);
            this.TB_coef_mut.TabIndex = 5;
            this.TB_coef_mut.Text = "2E-001";
            // 
            // LB_coef_mut
            // 
            this.LB_coef_mut.AutoSize = true;
            this.LB_coef_mut.Location = new System.Drawing.Point(9, 74);
            this.LB_coef_mut.Name = "LB_coef_mut";
            this.LB_coef_mut.Size = new System.Drawing.Size(122, 13);
            this.LB_coef_mut.TabIndex = 4;
            this.LB_coef_mut.Text = "Коэффициент мутации";
            // 
            // TB_count_popul
            // 
            this.TB_count_popul.Location = new System.Drawing.Point(209, 45);
            this.TB_count_popul.Name = "TB_count_popul";
            this.TB_count_popul.Size = new System.Drawing.Size(100, 20);
            this.TB_count_popul.TabIndex = 3;
            this.TB_count_popul.Text = "10";
            // 
            // LB_count_popul
            // 
            this.LB_count_popul.AutoSize = true;
            this.LB_count_popul.Location = new System.Drawing.Point(9, 48);
            this.LB_count_popul.Name = "LB_count_popul";
            this.LB_count_popul.Size = new System.Drawing.Size(105, 13);
            this.LB_count_popul.TabIndex = 2;
            this.LB_count_popul.Text = "Количество особей";
            // 
            // TB_eps
            // 
            this.TB_eps.Location = new System.Drawing.Point(209, 19);
            this.TB_eps.Name = "TB_eps";
            this.TB_eps.Size = new System.Drawing.Size(100, 20);
            this.TB_eps.TabIndex = 1;
            this.TB_eps.Text = "1E-008";
            // 
            // LB_eps
            // 
            this.LB_eps.AutoSize = true;
            this.LB_eps.Location = new System.Drawing.Point(10, 22);
            this.LB_eps.Name = "LB_eps";
            this.LB_eps.Size = new System.Drawing.Size(89, 13);
            this.LB_eps.TabIndex = 0;
            this.LB_eps.Text = "Точность (eps) =";
            // 
            // BT_learn
            // 
            this.BT_learn.Location = new System.Drawing.Point(277, 232);
            this.BT_learn.Name = "BT_learn";
            this.BT_learn.Size = new System.Drawing.Size(75, 53);
            this.BT_learn.TabIndex = 1;
            this.BT_learn.Text = "Обучить";
            this.BT_learn.UseVisualStyleBackColor = true;
            this.BT_learn.Click += new System.EventHandler(this.BT_learn_Click);
            // 
            // GB_res
            // 
            this.GB_res.Controls.Add(this.TB_res_count_step);
            this.GB_res.Controls.Add(this.LB_res_count_step);
            this.GB_res.Controls.Add(this.TB_res_eps);
            this.GB_res.Controls.Add(this.LB_res_eps);
            this.GB_res.Location = new System.Drawing.Point(12, 220);
            this.GB_res.Name = "GB_res";
            this.GB_res.Size = new System.Drawing.Size(258, 76);
            this.GB_res.TabIndex = 12;
            this.GB_res.TabStop = false;
            this.GB_res.Text = "Результат обучения";
            // 
            // TB_res_count_step
            // 
            this.TB_res_count_step.Location = new System.Drawing.Point(145, 45);
            this.TB_res_count_step.Name = "TB_res_count_step";
            this.TB_res_count_step.Size = new System.Drawing.Size(100, 20);
            this.TB_res_count_step.TabIndex = 3;
            // 
            // LB_res_count_step
            // 
            this.LB_res_count_step.AutoSize = true;
            this.LB_res_count_step.Location = new System.Drawing.Point(9, 48);
            this.LB_res_count_step.Name = "LB_res_count_step";
            this.LB_res_count_step.Size = new System.Drawing.Size(103, 13);
            this.LB_res_count_step.TabIndex = 2;
            this.LB_res_count_step.Text = "Затрачено шагов =";
            // 
            // TB_res_eps
            // 
            this.TB_res_eps.Location = new System.Drawing.Point(145, 19);
            this.TB_res_eps.Name = "TB_res_eps";
            this.TB_res_eps.Size = new System.Drawing.Size(100, 20);
            this.TB_res_eps.TabIndex = 1;
            // 
            // LB_res_eps
            // 
            this.LB_res_eps.AutoSize = true;
            this.LB_res_eps.Location = new System.Drawing.Point(10, 22);
            this.LB_res_eps.Name = "LB_res_eps";
            this.LB_res_eps.Size = new System.Drawing.Size(129, 13);
            this.LB_res_eps.TabIndex = 0;
            this.LB_res_eps.Text = "Достигнутая точность =";
            // 
            // LB_err
            // 
            this.LB_err.AutoSize = true;
            this.LB_err.Location = new System.Drawing.Point(21, 299);
            this.LB_err.Name = "LB_err";
            this.LB_err.Size = new System.Drawing.Size(31, 13);
            this.LB_err.TabIndex = 13;
            this.LB_err.Text = "err = ";
            // 
            // LB_max_err
            // 
            this.LB_max_err.AutoSize = true;
            this.LB_max_err.Location = new System.Drawing.Point(21, 312);
            this.LB_max_err.Name = "LB_max_err";
            this.LB_max_err.Size = new System.Drawing.Size(56, 13);
            this.LB_max_err.TabIndex = 14;
            this.LB_max_err.Text = "max_err = ";
            // 
            // LB_min_err
            // 
            this.LB_min_err.AutoSize = true;
            this.LB_min_err.Location = new System.Drawing.Point(21, 325);
            this.LB_min_err.Name = "LB_min_err";
            this.LB_min_err.Size = new System.Drawing.Size(53, 13);
            this.LB_min_err.TabIndex = 15;
            this.LB_min_err.Text = "min_err = ";
            // 
            // BT_write
            // 
            this.BT_write.Enabled = false;
            this.BT_write.Location = new System.Drawing.Point(277, 291);
            this.BT_write.Name = "BT_write";
            this.BT_write.Size = new System.Drawing.Size(75, 53);
            this.BT_write.TabIndex = 16;
            this.BT_write.Text = "Записать результат в БД";
            this.BT_write.UseVisualStyleBackColor = true;
            this.BT_write.Click += new System.EventHandler(this.BT_write_Click);
            // 
            // GeneticAlgorithmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 358);
            this.Controls.Add(this.BT_write);
            this.Controls.Add(this.LB_min_err);
            this.Controls.Add(this.LB_max_err);
            this.Controls.Add(this.LB_err);
            this.Controls.Add(this.GB_res);
            this.Controls.Add(this.BT_learn);
            this.Controls.Add(this.GB_gen_par);
            this.Name = "GeneticAlgorithmForm";
            this.Text = "GeneticAlgorithmForm";
            this.GB_gen_par.ResumeLayout(false);
            this.GB_gen_par.PerformLayout();
            this.GB_res.ResumeLayout(false);
            this.GB_res.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox GB_gen_par;
        private System.Windows.Forms.TextBox TB_max_step;
        private System.Windows.Forms.Label LB_max_step;
        private System.Windows.Forms.TextBox TB_selection_percent;
        private System.Windows.Forms.Label LB_selection_percent;
        private System.Windows.Forms.TextBox TB_train_percent;
        private System.Windows.Forms.Label LB_train_pecent;
        private System.Windows.Forms.TextBox TB_coef_mut;
        private System.Windows.Forms.Label LB_coef_mut;
        private System.Windows.Forms.TextBox TB_count_popul;
        private System.Windows.Forms.Label LB_count_popul;
        private System.Windows.Forms.TextBox TB_eps;
        private System.Windows.Forms.Label LB_eps;
        private System.Windows.Forms.Button BT_learn;
        private System.Windows.Forms.GroupBox GB_res;
        private System.Windows.Forms.TextBox TB_res_count_step;
        private System.Windows.Forms.Label LB_res_count_step;
        private System.Windows.Forms.TextBox TB_res_eps;
        private System.Windows.Forms.Label LB_res_eps;
        private System.Windows.Forms.CheckBox CB_lin_repr;
        private System.Windows.Forms.Label LB_err;
        private System.Windows.Forms.Label LB_max_err;
        private System.Windows.Forms.Label LB_min_err;
        private System.Windows.Forms.Button BT_write;
    }
}