using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class KohonenTree : TreeSection
    {
        public KohonenTree(models.Task task, models.TaskSolver[] koh_nets,
            TaskTreeViewModel vm) : base("Сети Кохонена", koh_nets.Select(x => x.Name).ToArray())
        {
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < koh_nets.Length; i++)
            {
                Content.Add(new SolverLeaf(task, koh_nets[i], "Сеть Кохонена", vm));
            }
        }
    }
}
