using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class WardTree : TreeSection
    {
        public WardTree(models.Task task, models.TaskSolver[] wards,
            TaskTreeViewModel vm) : base("Сети Ворда", wards.Select(x => x.Name).ToArray())
        {
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < wards.Length; i++)
            {
                Content.Add(new SolverLeaf(task, wards[i], "Нейронная сеть Ворда", vm));
            }
        }
    }
}
