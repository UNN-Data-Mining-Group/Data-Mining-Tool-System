using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class Node
    {
        public string Name { get; set; }
        public Node[] Children { get; set; }

        public Node(string name, Node[] children = null)
        {
            Name = name;
            Children = children;
        }
    }
    public class TaskTreeViewModel
    {
        public TaskTreeViewModel()
        {
            Node task1 = new Node("Ирис", new Node[] {
                new Node("Выборки", new Node[] { new Node("Выборка 1"), new Node("Выборка 2")}),
                new Node("Решатели", new Node[] 
                {
                    new Node("Персептрон", new Node[] { new Node("п1") }),
                    new Node("Деревья решений", new Node[] { new Node("др1"), new Node("др2") })
                })
            });
            Node task2 = new Node("Морское ушко", new Node[] {
                new Node("Выборки", new Node[] { new Node("Выборка 1")}),
                new Node("Решатели", new Node[]
                {
                    new Node("Персептрон", new Node[] { new Node("п2"), new Node("п3") }),
                    new Node("Деревья решений", new Node[] { new Node("др1") }),
                    new Node("Ограниченная машина Больцмана", new Node[] { new Node("б1") })
                })
            });
            Tasks = new ObservableCollection<Node> { task1, task2 };
        }

        public ObservableCollection<Node> Tasks { get; set; }
    }
}
