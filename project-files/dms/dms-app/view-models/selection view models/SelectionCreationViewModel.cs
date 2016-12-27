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
    public class SelectionCreationViewModel : ViewmodelBase
    {
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

        public string ParentTask { get; set; }
        public string SelectionName { get { return selectionName; } set { selectionName = value; NotifyPropertyChanged(); } }
        public bool HasHeader { get { return hasHeader; } set { hasHeader = value; NotifyPropertyChanged(); } }
        public int CountRows { get { return countRows; } set { countRows = value; NotifyPropertyChanged(); } }
        public string FilePath { get { return filePath; } set { filePath = value; NotifyPropertyChanged(); } }
        public List<string> DelimiterList { get { return new List<string> { ".", "," }; } }
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

        public ObservableCollection<ParameterCreationViewModel> Parameters { get; set; }

        public SelectionCreationViewModel(string taskName)
        {
            ParentTask = taskName;
            Random r = new Random();
            SelectionName = "Выборка " + r.Next(1, 1000);
            HasHeader = false;
            CountRows = 0;
            FilePath = "";
            Delimiter = ",";

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
            OnClose?.Invoke(this, null);
        }

        public void BrowseFile()
        {
            FilePath = "/usr/file1.txt";
            CountRows = 2000;

            updateAllowedTemplates();
        }

        private void updateAllowedTemplates()
        {
            CanUseExitingTemplate = true;
            CanCreateTemplate = true;

            Parameters.Clear();
            Parameters.Add(new ParameterCreationViewModel(1));
            Parameters.Add(new ParameterCreationViewModel(2, "Parameter 2", "enum", true));

            TemplateList.Clear();
            TemplateList.Add("шаблон 1");
            TemplateList.Add("шаблон 2");

            SelectedTemplate = TemplateList[0];
        }
    }
}
