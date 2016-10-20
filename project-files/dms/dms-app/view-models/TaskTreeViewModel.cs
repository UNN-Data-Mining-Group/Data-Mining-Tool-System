using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class Task
    {
        public string Name { get; set; }
        public string Dataset { get; set; }
        public string[] NeuroNets { get; set; }

        public Task(string name, string dataset, string[] neuronets = null)
        {
            Name = name;
            Dataset = dataset;
            NeuroNets = neuronets;
        }
    }

    public class TaskTreeViewModel
    {
        public TaskTreeViewModel()
        {
            Tasks = new ObservableCollection<Task>{
                new Task("Task1", "set1", new string[]{ "net1", "net2"}),
                new Task("Task2", "set2")
            };
        }

        public ObservableCollection<Task> Tasks { get; set; }
    }
}
