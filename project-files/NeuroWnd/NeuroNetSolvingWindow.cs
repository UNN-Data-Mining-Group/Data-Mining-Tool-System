using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NeuroWnd.Neuro_Nets;

namespace NeuroWnd
{
    public partial class NeuroNetSolvingWindow : Form
    {
        private NeuroNet currentNet;
        private List<Tuple<Label, TextBox>> inputs;
        private List<Tuple<Label, TextBox>> outputs;

        public NeuroNetSolvingWindow(NeuroNet net)
        {
            InitializeComponent();

            currentNet = net;

            inputs = new List<Tuple<Label, TextBox>>();
            outputs = new List<Tuple<Label, TextBox>>();
            for (int i = 0; i < net.InputNeuronsCount; i++)
            {
                Label lb = new Label();
                lb.Text = "x[" + i + "]=";
                lb.Location = new Point(15, 17 + i * 25);
                lb.Size = new Size(lb.Text.Length * 8, 20);
                gbInputs.Controls.Add(lb);

                TextBox tb = new TextBox();
                tb.Text = "0,0";
                tb.Location = new Point(lb.Text.Length * 10 + 5, 15 + i * 25);
                tb.Size = new Size(100, 20);
                gbInputs.Controls.Add(tb);

                inputs.Add(new Tuple<Label, TextBox>(lb, tb));
            }
            for (int i = 0; i < net.OutputNeuronsCount; i++)
            {
                Label lb = new Label();
                lb.Text = "y[" + i + "]=";
                lb.Location = new Point(15, 17 + i * 25);
                lb.Size = new Size(lb.Text.Length * 8, 20);
                gbOutputs.Controls.Add(lb);

                TextBox tb = new TextBox();
                tb.Text = "0,0";
                tb.Location = new Point(lb.Text.Length * 10 + 5, 15 + i * 25);
                tb.Size = new Size(100, 20);
                gbOutputs.Controls.Add(tb);

                outputs.Add(new Tuple<Label, TextBox>(lb, tb));
            }
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            double[] input = new double[currentNet.InputNeuronsCount];
            double[] output = new double[currentNet.OutputNeuronsCount];

            try
            {

                for (int i = 0; i < currentNet.InputNeuronsCount; i++)
                {
                    input[i] = Convert.ToDouble(inputs[i].Item2.Text);
                }
                output = currentNet.MakeAnswer(input);
                for (int i = 0; i < currentNet.OutputNeuronsCount; i++)
                {
                    outputs[i].Item2.Text = Convert.ToString(output[i]);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
