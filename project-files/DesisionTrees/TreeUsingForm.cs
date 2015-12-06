using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SII;
using System.Globalization;

namespace DesisionTrees
{
    public partial class TreeUsingForm : Form
    {
        CultureInfo UsCulture = new CultureInfo("en-US");
        private SQLManagerForTrees sqlManager = new SQLManagerForTrees();
        public TreeNode root;
        public List<Parametr> arr_Params;

        public TreeUsingForm(List<Parametr> arrParams, int root_id)
        {
            InitializeComponent();
            FillInputValTable(arrParams);
            root = sqlManager.GetNodeWithRequest(root_id);
            arr_Params = arrParams;
        }

        public void FillInputValTable(List<Parametr> arrParams)
        {
            dgwInputVal.Rows.Clear();
            dgwInputVal.Columns.Clear();
            for (int i = 1; i < arrParams.Count; i++)
            {
                dgwInputVal.Columns.Add(arrParams[i].Name, arrParams[i].Name);
            }
            dgwInputVal.Rows.Add();
        }

        public String TreeUsing(String[] param_row, TreeNode curNode)
        {
            while (curNode.is_leaf != true)
            {

                if ((arr_Params[curNode.rule.index_of_param - 1].Type == TypeParametr.Real) || (arr_Params[curNode.rule.index_of_param - 1].Type == TypeParametr.Int))
                {
                    if (Convert.ToDouble(param_row[curNode.rule.index_of_param - 1], UsCulture) <= Convert.ToDouble(curNode.rule.value, UsCulture))
                    {
                        curNode = curNode.right_child;
                    }
                    else
                    {
                        curNode = curNode.left_child;
                    }
                }
                else
                {
                    if (param_row[curNode.rule.index_of_param - 1] == curNode.rule.value)
                    {
                        curNode = curNode.right_child;
                    }
                    else
                    {
                        curNode = curNode.left_child;
                    }
                }
            }
            return curNode.rule.value;
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {

            String[] input_row = new String[arr_Params.Count - 1];
            for (int i = 0; i < arr_Params.Count - 1; i++)
            {
                input_row[i] = dgwInputVal.Rows[0].Cells[i].Value.ToString();
            }
            String answer = TreeUsing(input_row, root);
            lblAnswer.Text = answer;
            lblAnswer.Visible = true;
        }
    }
}
