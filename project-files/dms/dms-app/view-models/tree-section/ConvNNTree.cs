using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class ConvNNTree : TreeSection
    {
        public ConvNNTree(models.Task task, models.TaskSolver[] convs,
            TaskTreeViewModel vm) : base("Сверточные НС", convs.Select(x => x.Name).ToArray())
        {
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < convs.Length; i++)
            {
                Content.Add(new SolverLeaf(task, convs[i], "Сверточная нейронная сеть", vm));
            }
        }
    }
}
