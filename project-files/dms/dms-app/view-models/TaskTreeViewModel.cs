using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using dms.tools;

namespace dms.view_models
{
    public class TaskTreeViewModel : ViewmodelBase
    {
        private ObservableCollection<TaskTree> tasks;
        private ActionHandler createTask;

        public event EventHandler<EventArgs<TaskCreationViewModel>> requestTaskCreation;
        public event EventHandler<EventArgs<SelectionCreationViewModel>> requestSelectionCreation;
        public event EventHandler<EventArgs<TaskInfoViewModel>> requestTaskInfoShow;
        public event EventHandler<EventArgs<SelectionInfoViewModel>> requestSelectionInfoShow;

        public TaskTreeViewModel()
        {
            Tasks = new ObservableCollection<TaskTree>
            {
                new TaskTree("Ирис", 
                    new string[] { "Выборка 1", "Выборка 2" },
                    new string[] { "Персептрон 1", "Персептрон 2" },
                    new string[] { "Дерево 1", },
                    new string[] { }, 
                    this), 
                new TaskTree("Морское ушко", 
                    new string[] {"Выборка 1"}, 
                    new string[] { "П_му1"}, 
                    new string[] { }, 
                    new string[] { "Решение 1"},
                    this)
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

        public void DeleteTask(string taskName)
        {
            TaskTree t = null;
            foreach(TaskTree item in Tasks)
            {
                if (item.Title.Equals(taskName))
                {
                    t = item;
                    break;
                }
            }
            if (t != null)
            {
                Tasks.Remove(t);
            }
        }

        public void ShowCreateTaskDialog()
        {
            TaskCreationViewModel t = new TaskCreationViewModel();
            requestTaskCreation?.Invoke(this, new EventArgs<TaskCreationViewModel>(t));
        }

        public void ShowTaskInfoDialog(string taskName)
        {
            TaskInfoViewModel t = new TaskInfoViewModel(taskName);
            requestTaskInfoShow?.Invoke(this, new EventArgs<TaskInfoViewModel>(t));
        }

        public void ShowSelectionInfoDialog(string taskName, string selectionName)
        {
            SelectionInfoViewModel t = new SelectionInfoViewModel(taskName, selectionName);
            requestSelectionInfoShow?.Invoke(this, new EventArgs<SelectionInfoViewModel>(t));
        }

        public void ShowCreateSelectionDialog(string taskName)
        {
            SelectionCreationViewModel t = new SelectionCreationViewModel(taskName);
            requestSelectionCreation?.Invoke(this, new EventArgs<SelectionCreationViewModel>(t));
        }

        public void DeleteSelection(string task, string selection)
        {
            foreach(TaskTree t in Tasks)
            {
                if(t.Title.Equals(task))
                {
                    SelectionTree st = t.Content[0] as SelectionTree;
                    SelectionLeaf toDelete = null;
                    foreach (SelectionLeaf s in st.Content)
                    {
                        if(s.Title.Equals(selection))
                        {
                            toDelete = s;
                            break;
                        }
                    }
                    if (toDelete != null)
                    {
                        st.Content.Remove(toDelete);
                        return;
                    }
                }
            }
        }
    }
}
