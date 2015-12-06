using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace LearningAlgorithms
{
    public partial class GeneticAlgorithmForm : Form
    {
        INeuroNetLearning solver;
        GeneticAlgorithm gen;
        double[,] training_set;
        public GeneticAlgorithmForm(INeuroNetLearning solver_, double[,] training_set_)
        {
            InitializeComponent();
            gen = new GeneticAlgorithm();
            solver = solver_;
            training_set = training_set_;
        }
        
        private void BT_learn_Click(object sender, EventArgs e)
        {
            string[] err = { "Неверное значение eps", "Неверное значение количества особей","Неверное значение коэффициента мутации"
                               ,"Неверное значение процента обучающей выборки","Неверное значение процента скрещивания","Неверное значение максимального числа шагов" };
            
            int i = 0;
            try
            {
                gen.set_eps(Convert.ToDouble(TB_eps.Text.ToString()));
                i++;
                gen.set_count_popul(Convert.ToInt32(TB_count_popul.Text.ToString()));
                i++;
                gen.set_coef_mut(Convert.ToDouble(TB_coef_mut.Text.ToString()));
                i++;
                gen.set_persent_train(Convert.ToDouble(TB_train_percent.Text.ToString()));
                i++;
                gen.set_selection_persent(Convert.ToDouble(TB_selection_percent.Text.ToString()));
                i++;
                gen.set_max_step(Convert.ToInt32(TB_max_step.Text.ToString()));
                i++;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(err[i]);
            }
            if (i == 6)
            {
                BT_learn.Enabled = false;
                gen.genom(solver, training_set, CB_lin_repr.Checked);
                BT_learn.Enabled = true;
                BT_write.Enabled = true;
                TB_res_count_step.Text = gen.get_step().ToString();
                TB_res_eps.Text = gen.get_min_err().ToString();
                gen.save_result();
                double[][] training_X = new double[training_set.GetLength(0)][];
                double[] training_Y = new double[training_set.GetLength(0)];

                for (int j = 0; j < training_set.GetLength(0); j++)
                {
                    training_X[j] = new double[training_set.GetLength(1) - 1];
                    training_Y[j] = training_set[j, training_set.GetLength(1) - 1];
                    for (int k = 0; k < training_set.GetLength(1) - 1; k++)
                    {
                        training_X[j][k] = training_set[j, k];
                    }
                }
                double error = 0;
                double max_err = 0,min_err = Double.MaxValue;
                double tmp_err;
                for (int j = 0; j < training_Y.Length; j++)
                {
                    double res = solver.get_res(training_X[j]);
                    tmp_err = Math.Pow(training_Y[j] - res, 2);
                    if (tmp_err > max_err)
                    {
                        max_err = tmp_err;
                    }
                    if (tmp_err < min_err)
                    {
                        min_err = tmp_err;
                    }
                    error += tmp_err;
                }
                error /= training_Y.Length;
                LB_err.Text = "err = "+error.ToString();
                LB_max_err.Text = "max_err = " + max_err.ToString();
                LB_min_err.Text = "min_err = " + min_err.ToString();
            }
        }

        private void BT_write_Click(object sender, EventArgs e)
        {
            solver.write_result(gen.Name);
            MessageBox.Show("Обученные веса записаны в базу данных");
            this.Close();
        }
    }
}
