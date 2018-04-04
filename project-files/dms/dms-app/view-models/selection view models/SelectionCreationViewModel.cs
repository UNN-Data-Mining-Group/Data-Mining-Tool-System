using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using dms.tools;
using dms.services.preprocessing;
using dms.models;
using System.IO;
using Microsoft.Win32;

namespace dms.view_models
{
    public class SelectionCreationViewModel : ViewmodelBase
    {
        public class Pair
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public event Action<bool> selectionCreate;
        private ActionHandler browseFileCommandHandler;
        private ActionHandler cancelHandler;
        private ActionHandler createHandler;

        public int selectionId;
        private string selectionName;

        private bool canUseExitingTemplate;
        private bool canCreateTemplate;
        private bool isUsingExitingTemplate;

        public event EventHandler OnClose;

        public ICommand BrowseFileCommand { get { return browseFileCommandHandler; } }
        public ICommand CancelCommand { get { return cancelHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }

        public int TaskId { get; set; }
        public string ParentTask { get; set; }
        public string SelectionName { get { return selectionName; } set { selectionName = value; NotifyPropertyChanged(); } }

        private bool hasHeader;
        public bool HasHeader { get { return hasHeader; } set { hasHeader = value; NotifyPropertyChanged(); } }

        private int countRows;
        public int CountRows { get { return countRows; } set { countRows = value; NotifyPropertyChanged(); } }

        private string filePath;
        public string FilePath { get { return filePath; } set { filePath = value; NotifyPropertyChanged(); } }

        private string delimiter;
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
                canCreateTemplate = value;// && (NewTemplateName != null && NewTemplateName != "");
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

        private Pair selectedTemplate;
        public Pair SelectedTemplate
        {
            get { return selectedTemplate; }
            set { selectedTemplate = value;
                NotifyPropertyChanged(); }
        }

        private ObservableCollection<string> templateList;
        public ObservableCollection<string> TemplateList { get { return templateList; } set { templateList = value; NotifyPropertyChanged(); } }

        private string newTemplateName;
        public string NewTemplateName { get { return newTemplateName; } set { newTemplateName = value; NotifyPropertyChanged(); } }
        

        public float EnumPercent { get; set; }

        public ObservableCollection<ParameterCreationViewModel> Parameters { get; set; }
        public TaskTreeViewModel taskTreeVM;

        public SelectionCreationViewModel(int taskId, TaskTreeViewModel vm)
        {
            taskTreeVM = vm;
            TaskId = taskId;
            ParentTask = ((models.Task)services.DatabaseManager.SharedManager.entityById(taskId, typeof(models.Task))).Name;
            Random r = new Random();
            int number = r.Next(1, 1000);
            SelectionName = "Выборка " + number;
            NewTemplateName = "Базовый шаблон для Выборки " + number;
            HasHeader = false;
            CountRows = 0;
            FilePath = "";
            Delimiter = ",";
            EnumPercent = 5;

            browseFileCommandHandler = new ActionHandler(BrowseFile, (o) => true);
            cancelHandler = new ActionHandler(Cancel, o => true);
            createHandler = new ActionHandler(Create, o => (CanUseExitingTemplate || CanCreateTemplate));

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
            int taskTemplateId;
            if (IsUsingExitingTemplate)
            {
                taskTemplateId = SelectedTemplate.Id;
            }
            else
            {
                //Определение индекса последней выборки
                List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                    .addCondition("TaskId", "=", TaskId.ToString()), typeof(TaskTemplate));
                string templateNameForEmptyField = "Template " + ((taskTemplates != null) ? taskTemplates.Count + 1 : 1);
                string templateName = (newTemplateName == null || newTemplateName == "") ? templateNameForEmptyField : newTemplateName;
                DataHelper helper = new DataHelper();

                //ppParameters = null для главного шаблона
                IPreprocessingParameters ppParameters = null;
                taskTemplateId = helper.addTaskTemplate(templateName, TaskId, ppParameters);
            }
            
            ParameterCreationViewModel[] parameters = Parameters.ToArray();
            selectionId = PreprocessingManager.PrepManager.parseSelection(taskTemplateId, filePath, delimiter.ToCharArray()[0],
                TaskId, selectionName, parameters, HasHeader, IsUsingExitingTemplate);
            PreprocessingManager.PrepManager.updateTask(TaskId, PreprocessingManager.PrepManager.getCountParameters(), 
                PreprocessingManager.PrepManager.getCountRows());

            taskTreeVM.SendRequestCreateView(this);
            OnClose?.Invoke(this, null);
        }

        public void BrowseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "data files (*.data)|*.data|csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = @"datasets";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                    FilePath = Path.GetFullPath(filename);
            }
            updateAllowedTemplates();
        }
        
        private void updateAllowedTemplates()
        {
            CanUseExitingTemplate = true;
            CanCreateTemplate = true;

            Parameters.Clear();
            string[] paramTypes = PreprocessingManager.PrepManager.getParametersTypes(FilePath, delimiter.ToCharArray()[0], HasHeader, EnumPercent);
            string[] parametersName = null;
            if (HasHeader)
            {
                parametersName = PreprocessingManager.PrepManager.getParametersName();
            }
            int step = 0;
            foreach (string type in paramTypes)
            {
                step++;
                string name = "Parameter " + step;
                if (HasHeader)
                {
                    name = parametersName[step - 1];
                }
                Parameters.Add(new ParameterCreationViewModel(step, name, type + "(" + EnumPercent + ")", false));
            }
            CountRows = PreprocessingManager.PrepManager.getCountRows();

            TemplateList.Clear();
            List<Entity> templates = searchTaskTemplates(paramTypes);
            Pair pair = new Pair();
            if (templates.Count != 0)
            {
                pair.Id = templates[0].ID;
                pair.Name = TemplateList[0];
            }
            else
            {
                pair.Id = 1;
                pair.Name = "";
            }
            SelectedTemplate = pair;
        }

        private List<Entity> searchTaskTemplates(string[] selParameters)
        {
            List<Entity> templates = new List<Entity>();
            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", TaskId.ToString()), typeof(TaskTemplate));

            foreach (Entity taskTemplate in taskTemplates)
            {
                bool canAbord = false;
                List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplate.ID.ToString()), typeof(models.Parameter));

                if (selParameters.Length == parameters.Count)
                {
                    int index = 0;
                    foreach (string parameter in selParameters)
                    {
                        if (((models.Parameter)parameters[index]).Type !=
                            Parser.SelectionParser.getTypeParameter(selParameters[index]))
                        {
                            canAbord = true;
                            break;
                        }
                        index++;
                    }
                    if (!canAbord)
                    {
                        templates.Add(taskTemplate);
                        TemplateList.Add(((TaskTemplate)taskTemplate).Name);
                    }
                }
            }
            if (templates.Count.Equals(0))
            {
                CanUseExitingTemplate = false;
            }
            return templates;
        }
    }
}
