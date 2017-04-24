using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using dms.tools;
using Microsoft.Win32;

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
            exportSystem = new ActionHandler(ShowExportSystemDialog, e => true);
            importSystem = new ActionHandler(ShowImportSystemDialog, e => true);
        }

        public event EventHandler<EventArgs<TaskCreationViewModel>> requestTaskCreation;
        public event Action<bool> requestTaskTreeShow;
        public event Action<bool> requestLearnPaneShow;
        public event Action<bool> requestImportSystem;
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
        public ICommand ShowExportSystemDialogCommand
        {
            get { return exportSystem; }
        }
        public ICommand ShowImportSystemDialogCommand
        {
            get { return importSystem; }
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

        public void ShowExportSystemDialog()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "dmsexp files (*.dmsexp)|*.dmsexp";
            sfd.FilterIndex = 2;
            sfd.Title = "Экспорт системы";
            if (sfd.ShowDialog() == true)
            {
                services.archivation.ArchivatorService.SharedManager.exportSystem(sfd.FileName);
            }
        }

        public void ShowImportSystemDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "dmsexp files (*.dmsexp)|*.dmsexp";
            openFileDialog.FilterIndex = 2;
            openFileDialog.Title = "Импорт системы";
            if (openFileDialog.ShowDialog() == true)
            {
                services.archivation.ArchivatorService.SharedManager.importSystem(openFileDialog.FileName);
                requestImportSystem?.Invoke(true);
            }
        }

        public void ShowLearningScenariosManager()
        {
            LearningScenarioManagerViewModel t = new LearningScenarioManagerViewModel();
            requestLSShow?.Invoke(t);
        }

        private ActionHandler showScenarios;
        private ActionHandler createTask;
        private ActionHandler exportSystem;
        private ActionHandler importSystem;
        private bool isTaskTreeVisible;
        private bool isLearnPaneVisible;
    }
}
