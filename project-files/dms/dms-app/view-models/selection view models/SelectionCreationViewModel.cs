using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using dms.tools;
using dms.services.preprocessing;

namespace dms.view_models
{
    public class SelectionCreationViewModel : ViewmodelBase
    {
        public int selectionId;
        public event Action<bool> selectionCreate;
        private ActionHandler browseFileCommandHandler;
        private ActionHandler cancelHandler;
        private ActionHandler createHandler;
        private string selectionName;
        private bool hasHeader;
        private int countRows;
        private string filePath;
        private string delimiter;
        private bool canUseExitingTemplate;
        private bool canCreateTemplate;
        private bool isUsingExitingTemplate;
        private string selectedTemplate;
        private ObservableCollection<string> templateList;
        private string newTemplateName;

        public event EventHandler OnClose;

        public ICommand BrowseFileCommand { get { return browseFileCommandHandler; } }
        public ICommand CancelCommand { get { return cancelHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }

        public int TaskId { get; set; }
        public string ParentTask { get; set; }
        public string SelectionName { get { return selectionName; } set { selectionName = value; NotifyPropertyChanged(); } }
        public bool HasHeader { get { return hasHeader; } set { hasHeader = value; NotifyPropertyChanged(); } }
        public int CountRows { get { return countRows; } set { countRows = value; NotifyPropertyChanged(); } }
        public string FilePath { get { return filePath; } set { filePath = value; NotifyPropertyChanged(); } }
        public List<string> DelimiterList { get { return new List<string> { ".", "," , "|"}; } }
        public string Delimiter { get { return delimiter; } set { delimiter = value; NotifyPropertyChanged(); } }
        public bool CanUseExitingTemplate
        {
            get { return canUseExitingTemplate; }
            set
            {
                canUseExitingTemplate = value;
                IsUsingExitingTemplate = IsUsingExitingTemplate && value;
                NotifyPropertyChanged();
                createHandler.RaiseCanExecuteChanged();
            }
        }
        public bool IsUsingNewTemplate { get { return CanCreateTemplate && !IsUsingExitingTemplate; } }
        public bool CanCreateTemplate {
            get { return canCreateTemplate; }
            set
            {
                canCreateTemplate = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsUsingNewTemplate");
                createHandler.RaiseCanExecuteChanged();
            }
        }

        public bool IsUsingExitingTemplate
        {
            get { return isUsingExitingTemplate; }
            set
            {
                isUsingExitingTemplate = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsUsingNewTemplate");
            }
        }

        public string SelectedTemplate { get { return selectedTemplate; } set { selectedTemplate = value; NotifyPropertyChanged(); } }
        public ObservableCollection<string> TemplateList { get { return templateList; } set { templateList = value; NotifyPropertyChanged(); } }
        public string NewTemplateName { get { return newTemplateName; } set { newTemplateName = value; NotifyPropertyChanged(); } }
        public float EnumPercent { get; set; }

        public ObservableCollection<ParameterCreationViewModel> Parameters { get; set; }
        public TaskTreeViewModel VM;
        public SelectionCreationViewModel(int taskId, TaskTreeViewModel vm)
        {
            VM = vm;
            TaskId = taskId;
            ParentTask = ((dms.models.Task)dms.services.DatabaseManager.SharedManager.entityById(taskId, typeof(dms.models.Task))).Name;
            Random r = new Random();
            SelectionName = "Выборка " + r.Next(1, 1000);
            HasHeader = false;
            CountRows = 0;
            FilePath = "";
            Delimiter = ",";
            EnumPercent = 5;

            browseFileCommandHandler = new ActionHandler(BrowseFile, (o) => true);
            cancelHandler = new ActionHandler(Cancel, o => true);
            createHandler = new ActionHandler(Create, o => CanUseExitingTemplate && CanCreateTemplate);

            Parameters = new ObservableCollection<ParameterCreationViewModel>();
            CanUseExitingTemplate = CanCreateTemplate = false;
            TemplateList = new ObservableCollection<string>();
        }

        public void Cancel()
        {
            OnClose?.Invoke(this, null);
        }

        public void Create()
        {
            string templateName = newTemplateName == null ? "Template" : newTemplateName;
            ParameterCreationViewModel[] parameters = Parameters.ToArray();
            selectionId = PreprocessingManager.PrepManager.parseSelection(templateName, filePath, delimiter.ToCharArray()[0], TaskId, selectionName, parameters);
            PreprocessingManager.PrepManager.updateTask(TaskId, PreprocessingManager.PrepManager.getCountParameters(), 
                PreprocessingManager.PrepManager.getCountRows());

            VM.SendRequestCreateView(this);
            OnClose?.Invoke(this, null);
        }

        public void BrowseFile()
        {
            FilePath = "part_of_selection_file.txt";//"/usr/file1.txt";"selection_file.txt";

            updateAllowedTemplates();
        }

        private void updateAllowedTemplates()
        {
            CanUseExitingTemplate = true;
            CanCreateTemplate = true;

            Parameters.Clear();
            string[] paramTypes = PreprocessingManager.PrepManager.getParameterTypes(FilePath, delimiter.ToCharArray()[0]);
            int step = 0;
            foreach(string type in paramTypes)
            {
                step++;
                Parameters.Add(new ParameterCreationViewModel(step, "Parameter " + step, type + "(" + EnumPercent + ")", false));
            }
            CountRows = PreprocessingManager.PrepManager.getCountRows();

            TemplateList.Clear();
            TemplateList.Add("шаблон 1");
            TemplateList.Add("шаблон 2");

            SelectedTemplate = TemplateList[0];
        }
    }
}
