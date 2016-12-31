using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using dms.tools;

namespace dms.view_models
{
    public class MainWindowViewModel : ViewmodelBase
    {
        public MainWindowViewModel()
        {
            IsTaskTreeVisible = true;
            IsLearnPaneVisible = true;
            createTask = new ActionHandler(ShowCreateTaskDialog, e => true);
            showScenarios = new ActionHandler(ShowLearningScenariosManager, e => true);
        }

        public event EventHandler<EventArgs<TaskCreationViewModel>> requestTaskCreation;
        public event Action<bool> requestTaskTreeShow;
        public event Action<bool> requestLearnPaneShow;
        public event Action<LearningScenarioManagerViewModel> requestLSShow;

        public bool IsTaskTreeVisible
        {
            get { return isTaskTreeVisible; }
            set
            {
                if (value != isTaskTreeVisible)
                {
                    requestTaskTreeShow?.Invoke(value);
                    isTaskTreeVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLearnPaneVisible
        {
            get { return isLearnPaneVisible; }
            set
            {
                if (value != IsLearnPaneVisible)
                {
                    requestLearnPaneShow?.Invoke(value);
                    isLearnPaneVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand ShowCreateTaskDialogCommand
        {
            get { return createTask; }
        }
        public ICommand ShowLearningScenarioManagerCommand
        {
            get { return showScenarios; }
        }

        public void ShowCreateTaskDialog()
        {
            TaskCreationViewModel t = new TaskCreationViewModel();
            requestTaskCreation?.Invoke(this, new EventArgs<TaskCreationViewModel>(t));
        }

        public void ShowLearningScenariosManager()
        {
            LearningScenarioManagerViewModel t = new LearningScenarioManagerViewModel();
            requestLSShow?.Invoke(t);
        }

        private ActionHandler showScenarios;
        private ActionHandler createTask;
        private bool isTaskTreeVisible;
        private bool isLearnPaneVisible;
    }
}
