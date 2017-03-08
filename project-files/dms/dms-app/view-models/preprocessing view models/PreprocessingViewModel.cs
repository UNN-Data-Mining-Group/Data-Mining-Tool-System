using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using dms.tools;
using dms.models;
using dms.services.preprocessing;

namespace dms.view_models
{
    public class PreprocessingParameterViewModel
    {
        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public string Type { get; set; }
        public string[] TypesList { get { return new string[] { "нормализация 1 (к float)", "нормализация 2 (к int)", "бинаризация", "без предобработки" }; } }
    }

    public class PreprocessingViewModel : ViewmodelBase
    { 
        private ActionHandler cancelHandler;
        private ActionHandler createHandler;
        public ICommand CancelCommand { get { return cancelHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }
        public event EventHandler OnClose;

        public int TemplateId;
        private int TaskId;

        public PreprocessingViewModel(int taskId, int templateId)
        {
            TaskId = taskId;
            TaskName = ((dms.models.Task)dms.services.DatabaseManager.SharedManager.entityById(taskId, typeof(dms.models.Task))).Name;
            //TaskTemplates
            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", taskId.ToString()), typeof(TaskTemplate));
            BaseTemplateList = new string[taskTemplates.Count];
            BaseTemplateListPair = new Pair[taskTemplates.Count];
            int index = 0;
            bool canCreate = false;
            if (taskTemplates.Count != 0)
            {
                canCreate = true;
                foreach (Entity entity in taskTemplates)
            {
                Pair pair = new Pair();
                pair.Id = entity.ID;
                pair.Name = ((TaskTemplate)entity).Name;
                BaseTemplateList[index] = pair.Name;
                BaseTemplateListPair[index] = pair;
                    index++;
            }
            PerformedTemplateListPair = BaseTemplateListPair;
            PerformedTemplateList = BaseTemplateList;
            
            IsUsingExitingTemplate = false;
            Random r = new Random();
            PreprocessingName = "Преобразование " + r.Next(1, 1000);
            NewTemplateName = "New Template";
            TaskTemplate template = null;
            if (templateId == -1)
            {
                TemplateId = taskTemplates[0].ID;
                PerformedTemplatePair = PerformedTemplateListPair[0];
                BaseTemplatePair = BaseTemplateListPair[0];

                PerformedTemplate = PerformedTemplateListPair[0].Name;
                BaseTemplate = BaseTemplateListPair[0].Name;
             }
             else
                {
                    TemplateId = templateId;
                    template = ((dms.models.TaskTemplate)dms.services.DatabaseManager.SharedManager.entityById(templateId, typeof(dms.models.TaskTemplate)));
                    Pair pair = new view_models.PreprocessingViewModel.Pair();
                    pair.Id = templateId;
                    pair.Name = template.Name;
                    PerformedTemplatePair = pair;//PerformedTemplateListPair[0];
                    BaseTemplatePair = BaseTemplateListPair[0];

                    PerformedTemplate = template.Name; //PerformedTemplateListPair[0].Name;
                    BaseTemplate = BaseTemplateListPair[0].Name;
                }
            
            List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", TemplateId.ToString()), typeof(dms.models.Parameter));
            PreprocessingParameters = new PreprocessingParameterViewModel[parameters.Count];
            index = 0;
            foreach (Entity entity in parameters)
            {
                PreprocessingParameters[index] = new PreprocessingParameterViewModel();
                PreprocessingParameters[index].ParameterId = entity.ID;
                PreprocessingParameters[index].ParameterName = ((dms.models.Parameter)entity).Name;
                if (template == null)
                {
                    PreprocessingParameters[index].Type = "без предобработки";
                }
                else
                {
                    PreprocessingViewModel.PreprocessingTemplate pp = (PreprocessingViewModel.PreprocessingTemplate) template.PreprocessingParameters;
                    Dictionary<Parameter, string> dictionary = pp.get();
                    Parameter[] paramss = dictionary.Keys.ToArray();
                    PreprocessingParameters[index].Type = paramss[index].Type;
                }
                
                index++;
            }
            }
            
            cancelHandler = new ActionHandler(Cancel, o => true);
            createHandler = new ActionHandler(Create, o => canCreate && CanUseExitingTemplate && CanCreateTemplate);
            CanUseExitingTemplate = CanCreateTemplate = true;// true;
            IsUsingExitingTemplate = false;
         //   CanCreateTemplate = true;
        }

        [Serializable()]
        public class PreprocessingTemplate : IPreprocessingParameters
        {
            public string PreprocessingName { get; set; }
            public Pair BaseTemplate { get; set; }
            public Dictionary<Parameter, string> parameters = new Dictionary<Parameter, string>();
            public int sizeS;

            public void set(Parameter p, string type, int index)
            {
                parameters.Add(p, type);
            }

            public Dictionary<Parameter, string> get()
            {
                return parameters;
            }

            public void testMethod()
            {

            }
        }

        public void Create()
        {
            if (BaseTemplate != null)
            {
                List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", BaseTemplatePair.Id.ToString()), typeof(Selection));
                int taskTemplateId;

                if (IsUsingExitingTemplate)
                {
                    taskTemplateId = TemplateId;
                }
                else
                {
                    PreprocessingTemplate pp = new PreprocessingTemplate();
                    pp.PreprocessingName = PreprocessingName;
                    pp.BaseTemplate = BaseTemplatePair;
                    int step = 0;
                    foreach (PreprocessingParameterViewModel prepParam in PreprocessingParameters)
                    {
                        Parameter p = new Parameter(prepParam.ParameterName, prepParam.Type, "");
                        pp.set(p, prepParam.Type, step);
                        step++;
                    }
                    taskTemplateId = new DataHelper().addTaskTemplate(NewTemplateName + " - " + PreprocessingName, TaskId, pp);
                }
                bool canAdd = true;
                foreach (Entity sel in selections)
                {
                    int newSelectionId = -1;
                    if (!IsUsingExitingTemplate)
                    {
                        newSelectionId = PreprocessingManager.PrepManager.addNewEntitiesForPreprocessing(
                            ((Selection)sel).Name,
                            ((Selection)sel).RowCount, TaskId, taskTemplateId);
                    }

                    int index = 0;

                    foreach (PreprocessingParameterViewModel prepParam in PreprocessingParameters)
                    {
                        int paramId = prepParam.ParameterId;
                        string prepType = prepParam.Type;

                        int selectionId = sel.ID;
                        PreprocessingManager.PrepManager.executePreprocessing(taskTemplateId, newSelectionId, selectionId, paramId,
                            prepType, PreprocessingParameters.Count(), index + 1, canAdd);
                        index++;

                    }
                    canAdd = false;
                }
            }
            
             OnClose?.Invoke(this, null);
        }

        public void Cancel()
        {
            OnClose?.Invoke(this, null);
        }

        public string TaskName { get; }
        public string PreprocessingName { get; set; }
        public Pair BaseTemplatePair { get; set; }
        public Pair PerformedTemplatePair { get; set; }
        public string BaseTemplate { get; set; }
        public string PerformedTemplate { get; set; }
        public PreprocessingParameterViewModel[] PreprocessingParameters { get; private set; }
        public Pair[] BaseTemplateListPair { get; private set; }
        public Pair[] PerformedTemplateListPair { get; private set; }
        public string[] BaseTemplateList { get; private set; }
        public string[] PerformedTemplateList { get; private set; }
     //   public bool IsUsingExitingTemplate { get; set; }
        public string NewTemplateName { get; set; }
        public TemplateViewModel BaseTemplateViewModel { get { return new TemplateViewModel(BaseTemplatePair.Id); } }
        public TemplateViewModel PerformedTemplateViewModel {
            get {                
                return new TemplateViewModel(PerformedTemplatePair.Id, 1);
            }
        }
        [Serializable]
        public class Pair
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
        private bool canUseExitingTemplate;
        private bool canCreateTemplate;
        private bool isUsingExitingTemplate;
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
        public bool CanCreateTemplate
        {
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
    }
}
