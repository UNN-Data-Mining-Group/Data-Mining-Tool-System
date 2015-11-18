using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SII;

namespace DesisionTrees
{
    public class Rule
    {
        public int index_of_param;
        public String value;

        public Rule(int _param = 0, String _value = "")
        {
            index_of_param = _param;
            value = _value;
        }
    }

    public class VeryfiedClassInfo
    {
        public String class_name;
        public int number_of_checked;

        public VeryfiedClassInfo()
        {
            number_of_checked = 0;
            class_name = "";
        }
    }

    public class TreeNode
    {
        public TreeNode left_child;
        public TreeNode right_child;
        public Rule rule;
        public bool is_leaf;
        //public VeryfiedClassInfo[] checked_classes;
        

        public TreeNode(TreeNode l_child = null, TreeNode r_child =null, Rule new_rule = null, bool isleaf = false) //VeryfiedClassInfo[] ch_classes=null)
        {
            left_child = l_child;
            right_child = r_child;
            rule = new_rule;
            is_leaf = isleaf;
            //checked_classes = ch_classes;
        }

        public bool IsLeaf()
        {
            return is_leaf;
        }

        
    }

    public class Tree
    {
        public int ID;
        public int TASK_ID;
        public int SELECTION_ID;
        public int ROOT_ID;
        
        public string[] Row()
        {
            string[] res = new string[4];
            res[0] = ID.ToString();
            res[1] = TASK_ID.ToString();
            res[2] = SELECTION_ID.ToString();
            res[3] = ROOT_ID.ToString();
            return res;
        }
    }

}
