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
        public string[] TypesList { get { return new string[] { "нормализация", "бинаризация", "без предобработки" }; } }
    }

    public class PreprocessingViewModel : ViewmodelBase
    {
        private ActionHandler cancelHandler;
        private ActionHandler createHandler;
        public ICommand CancelCommand { get { return cancelHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }
        public event EventHandler OnClose;

        public int templateId;
        private int TaskId;

        public PreprocessingViewModel(int taskId)
        {
            TaskId = taskId;
            //TaskTemplates
            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", taskId.ToString()), typeof(TaskTemplate));
            BaseTemplateList = new string[taskTemplates.Count];
            BaseTemplateListPair = new Pair[taskTemplates.Count];
            int index = 0;
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
            PerformedTemplate = PerformedTemplateListPair[0]; 
            BaseTemplate = BaseTemplateListPair[0];
            IsUsingExitingTemplate = false;
            TaskName = ((dms.models.Task)dms.services.DatabaseManager.SharedManager.entityById(taskId, typeof(dms.models.Task))).Name;
            PreprocessingName = "Преобразование 1";
            
            templateId = taskTemplates[0].ID;
            List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", templateId.ToString()), typeof(dms.models.Parameter));
            PreprocessingParameters = new PreprocessingParameterViewModel[parameters.Count];
            index = 0;
            foreach (Entity entity in parameters)
            {
                PreprocessingParameters[index] = new PreprocessingParameterViewModel();
                PreprocessingParameters[index].ParameterId = entity.ID;
                PreprocessingParameters[index].ParameterName = ((dms.models.Parameter)entity).Name;
                PreprocessingParameters[index].Type = "без предобработки";
                index++;
            }
            
            cancelHandler = new ActionHandler(Cancel, o => true);
            createHandler = new ActionHandler(Create, o => CanUseExitingTemplate && CanCreateTemplate);
            CanUseExitingTemplate = CanCreateTemplate = true;
        }

        [Serializable()]
        public class PreprocessingTemplate : IPreprocessingParameters
        {
            public string PreprocessingName { get; set; }
            public int BaseTemplate { get; set; }
            public Tuple<Parameter, string>[] newParam;
            public int sizeS;

            public void initArray(int size)
            {
                sizeS = size;
                newParam = new Tuple<Parameter, string>[size];
            }

            public void set(Parameter p, string type, int index)
            {
                newParam[index] = new Tuple<Parameter, string>(p, type);
            }

            public Tuple<Parameter, string>[] get()
            {
                return newParam;
            }
            
            public void testMethod()
            {

            }
        }

        public void Create()
        {
            List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", BaseTemplate.Id.ToString()), typeof(Selection));
            int newSelectionId = -1;
            int taskTemplateId;
            if (!IsUsingExitingTemplate)
            {
                PreprocessingTemplate pp = new PreprocessingTemplate();
                pp.PreprocessingName = PreprocessingName;
                pp.BaseTemplate = BaseTemplate.Id;
                pp.initArray(PreprocessingParameters.Length);
                int step = 0;
                foreach (PreprocessingParameterViewModel prepParam in PreprocessingParameters)
                {
                    Parameter p = new Parameter(prepParam.ParameterName, prepParam.Type, "nothing");
                    pp.set(p, prepParam.Type, step);
                    step ++;
                }
                taskTemplateId = new DataHelper().addTaskTemplate(NewTemplateName, TaskId, pp);
                newSelectionId = PreprocessingManager.PrepManager.addNewEntities(((Selection)selections[0]).Name, ((Selection)selections[0]).RowCount,
                    TaskId, taskTemplateId);
            } else
            {
                taskTemplateId = templateId;
            }
            string str = NewTemplateName;
            bool v = IsUsingExitingTemplate;
            int index = 0;
            foreach (PreprocessingParameterViewModel prepParam in PreprocessingParameters)
            {
                int paramId = prepParam.ParameterId;
                string prepType = prepParam.Type;
                
                int selectionId = selections[0].ID;
                PreprocessingManager.PrepManager.executePreprocessing(taskTemplateId, newSelectionId, selectionId, paramId, prepType);
                index++;
            }

             OnClose?.Invoke(this, null);
        }

        public void Cancel()
        {
            OnClose?.Invoke(this, null);
        }

        public string TaskName { get; }
        public string PreprocessingName { get; set; }
        public Pair BaseTemplate { get; set; }
        public Pair PerformedTemplate { get; set; }
        public PreprocessingParameterViewModel[] PreprocessingParameters { get; private set; }
        public Pair[] BaseTemplateListPair { get; private set; }
        public Pair[] PerformedTemplateListPair { get; private set; }
        public string[] BaseTemplateList { get; private set; }
        public string[] PerformedTemplateList { get; private set; }
     //   public bool IsUsingExitingTemplate { get; set; }
        public string NewTemplateName { get; set; }
        public TemplateViewModel BaseTemplateViewModel { get { return new TemplateViewModel(BaseTemplate.Name); } }
        public TemplateViewModel PerformedTemplateViewModel { get { return new TemplateViewModel(PerformedTemplate.Name, 1); } }

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
