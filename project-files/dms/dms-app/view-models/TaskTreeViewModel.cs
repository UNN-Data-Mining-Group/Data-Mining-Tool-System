using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class TreeSection : ViewmodelBase
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; NotifyPropertyChanged(); }
        }
        public TreeSection[] Content { get; set; }
        public TreeSection() { Title = ""; Content = null; }
        public TreeSection(string title) : this() { Title = title; }
        public TreeSection(string title, string[] content)
        {
            Title = title;
            Content = new TreeSection[content.Length];
            for(int i = 0; i < content.Length; i++)
            {
                Content[i] = new TreeSection(content[i]);
            }
        }
    }
    public class SelectionTree : TreeSection
    {
        public SelectionTree(string[] selections) : base("Выборки", selections) { }
    }
    public class PerceptronTree : TreeSection
    {
        public PerceptronTree(string[] perceptrons) : base("Персептроны", perceptrons) { }
    }
    public class DecisionTreesTree : TreeSection
    {
        public DecisionTreesTree(string[] trees) : base("Деревья решений", trees) { }
    }
    public class SolverTree  : TreeSection
    {
        public SolverTree(string[] per, string[] des) : base("Решатели")
        {
            Content = new TreeSection[] { new PerceptronTree(per), new DecisionTreesTree(des) };
        }
    }
    public class SolutionsTree : TreeSection
    {
        public SolutionsTree(string[] solutions) : base("Решения", solutions) { }
    }
    public class TaskTree : TreeSection
    {
        private ICommand _deleteCommand;
        private Action<TaskTree> _onDelete;

        public TaskTree(string Name, 
            string[] sel, string[] per, string[] des, string[] solv, 
            Action<TaskTree> onDelete)
        {
            Title = Name;
            Content = new TreeSection[] { new SelectionTree(sel), new SolverTree(per, des), new SolutionsTree(solv) };
            _onDelete = onDelete;
        }

        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new ActionHandler(() => Delete(), (e) => true));
            }
        }

        public void Delete()
        {
            Title += "-deleted";
            _onDelete(this);
        }
    }

    public class TaskTreeViewModel : ViewmodelBase
    {
        private ObservableCollection<TaskTree> tasks;
        private ActionHandler createTask;

        public event EventHandler<EventArgs<TaskCreationViewModel>> requestTaskCreation;

        public TaskTreeViewModel()
        {
            Tasks = new ObservableCollection<TaskTree>
            {
                new TaskTree("Ирис", 
                    new string[] { "Выборка 1", "Выборка 2" },
                    new string[] { "Персептрон 1", "Персептрон 2" },
                    new string[] { "Дерево 1", },
                    new string[] { }, 
                    Delete), 
                new TaskTree("Морское ушко", 
                    new string[] {"Выборка 1"}, 
                    new string[] { "П_му1"}, 
                    new string[] { }, 
                    new string[] { "Решение 1"},
                    Delete)
                };

            createTask = new ActionHandler(ShowCreateTaskDialog, e => true);
        }

        public ObservableCollection<TaskTree> Tasks
        {
            get { return tasks; }
            set { tasks = value; NotifyPropertyChanged(); }
        }

        public ICommand ShowCreateTaskDialogCommand
        {
            get { return createTask; }
        }

        public void Delete(TaskTree t)
        {
            if (Tasks.Contains(t))
                Tasks.Remove(t);
        }

        public void ShowCreateTaskDialog()
        {
            TaskCreationViewModel t = new TaskCreationViewModel();

            if (requestTaskCreation != null)
                requestTaskCreation(this, new EventArgs<TaskCreationViewModel>(t));
        }
    }
}
