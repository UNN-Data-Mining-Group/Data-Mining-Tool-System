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
        public int Position { get; set; }
        public int Id { get; set; }
        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public string Type { get; set; }
        public string[] TypesList { get { return new string[] { "Линейная нормализация 1 (к float)", "Нелинейная нормализация 2 (к float)", "бинаризация", "без предобработки" }; } }//, "нормализация 3 (к int)"
    }

    public class PreprocessingViewModel : ViewmodelBase
    { 
        private ActionHandler cancelHandler;
        private ActionHandler createHandler;
        public ICommand CancelCommand { get { return cancelHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }
        public event EventHandler OnClose;

        public int TemplateId;
        public string NewTemplateName { get; set; }
        
        public models.Task Task { get; }

        public string PreprocessingName { get; set; }
        public PreprocessingParameterViewModel[] PreprocessingParameters { get; private set; }
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

        [Serializable]
        public class Pair
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public Pair BaseTemplate { get; set; }
        public Pair PerformedTemplate { get; set; }

        public string[] BaseTemplateList {get; private set; }
        public string[] PerformedTemplateList { get; private set; }

        public int[] BaseTemplateIdList { get; private set; }
        public int[] PerformedTemplateIdList { get; private set; }

        public TemplateViewModel BaseTemplateViewModel { get { return new TemplateViewModel(BaseTemplate.Id); } }
        public TemplateViewModel PerformedTemplateViewModel { get { return new TemplateViewModel(PerformedTemplate.Id, 1); } }

        public PreprocessingViewModel(models.Task task, int templateId)
        {
            Task = task;
            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", task.ID.ToString()), typeof(TaskTemplate));

            BaseTemplateList = new string[taskTemplates.Count];
            BaseTemplateIdList = new int[taskTemplates.Count];
            
            bool canCreate = false;
            if (taskTemplates.Count != 0)
            {
                int index = 0;
                canCreate = true;
                foreach (Entity entity in taskTemplates)
                {
                    BaseTemplateIdList[index] = entity.ID;
                    BaseTemplateList[index] = ((TaskTemplate)entity).Name;
                    index++;
                }
                PerformedTemplateList = BaseTemplateList;
                PerformedTemplateIdList = BaseTemplateIdList;
            
                IsUsingExitingTemplate = false;
                Random r = new Random();
                PreprocessingName = "Преобразование " + r.Next(1, 1000);
                NewTemplateName = "New Template";
                TaskTemplate template = null;
                Pair pair = new Pair();
                if (templateId == -1)
                {
                    TemplateId = taskTemplates[0].ID;
                    pair.Id = PerformedTemplateIdList[0];
                    pair.Name = PerformedTemplateList[0];
                    PerformedTemplate = pair;
                }
                else
                {
                    TemplateId = templateId;
                    template = ((TaskTemplate)services.DatabaseManager.SharedManager.entityById(templateId, typeof(TaskTemplate)));
                    pair.Id = templateId;
                    pair.Name = template.Name;
                    PerformedTemplate = pair;
                }

                pair.Id = BaseTemplateIdList[0];
                pair.Name = BaseTemplateList[0];
                BaseTemplate = pair;

                List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                    .addCondition("TaskTemplateID", "=", TemplateId.ToString()), typeof(models.Parameter));
                PreprocessingParameters = new PreprocessingParameterViewModel[parameters.Count];
                index = 0;
                foreach (Entity entity in parameters)
                {
                    PreprocessingParameters[index] = new PreprocessingParameterViewModel();
                    PreprocessingParameters[index].ParameterId = entity.ID;
                    PreprocessingParameters[index].ParameterName = ((models.Parameter)entity).Name;
                    if (template == null)
                    {
                        PreprocessingParameters[index].Type = "без предобработки";
                    }
                    else
                    {
                        PreprocessingTemplate pp = (PreprocessingTemplate) template.PreprocessingParameters;
                        Dictionary<Parameter, string> dictionary = pp.get();
                        Parameter[] paramss = dictionary.Keys.ToArray();
                        PreprocessingParameters[index].Type = paramss[index].Type;
                    }
                    index++;
                }
            }
            
            cancelHandler = new ActionHandler(Cancel, o => true);
            createHandler = new ActionHandler(Create, o => canCreate && CanUseExitingTemplate && CanCreateTemplate);
            CanUseExitingTemplate = CanCreateTemplate = true;
            IsUsingExitingTemplate = false;
        }

        public void Create()
        {
            if (BaseTemplate != null)
            {
                for (int i = 0; i < BaseTemplateList.Count(); i++)
                {
                    if (BaseTemplateList[i].Equals(BaseTemplate.Name))
                    {
                        BaseTemplate.Id = BaseTemplateIdList[i];
                        break;
                    }
                }
                List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", BaseTemplate.Id.ToString()), typeof(Selection));

                int taskTemplateId;
                if (IsUsingExitingTemplate)
                {
                    taskTemplateId = TemplateId;
                }
                else
                {
                    PreprocessingTemplate pp = new PreprocessingTemplate();
                    pp.PreprocessingName = PreprocessingName;
                    pp.BaseTemplate = BaseTemplate;

                    List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                                .addCondition("TaskTemplateID", "=", BaseTemplate.Id.ToString()), typeof(models.Parameter));
                    for (int i = 0; i < PreprocessingParameters.Count(); i++)
                    {
                        PreprocessingParameters[i].ParameterId = parameters[i].ID;
                        PreprocessingParameters[i].ParameterName = ((models.Parameter)parameters[i]).Name;
                    }
                    string templateName = (NewTemplateName == null || NewTemplateName == "") ? "New Template" : NewTemplateName;
                     
                    taskTemplateId = new DataHelper().addTaskTemplate(templateName, Task.ID, pp);
                }
                foreach (Entity sel in selections)
                {
                    int newSelectionId = -1;
                    if (!IsUsingExitingTemplate)
                    {
                        newSelectionId = PreprocessingManager.PrepManager.addNewEntitiesForPreprocessing(
                            ((Selection)sel).Name,
                            ((Selection)sel).RowCount, Task.ID, taskTemplateId);
                    }

                    int oldSelectionId = sel.ID;
                    int index = 1;
                    PreprocessingParameterViewModel[] preprocessingParametersTemp = null;
                    foreach (PreprocessingParameterViewModel prepParam in PreprocessingParameters)
                    {
                        string prepType = prepParam.Type;
                        int oldParamId = prepParam.ParameterId;
                        int parameterPos = index;

                        if ("бинаризация".Equals(prepType))
                        {
                            List<Entity> newParameters = PreprocessingManager.PrepManager
                                .getNewParametersForBinarizationType(oldSelectionId, taskTemplateId, oldParamId);
                            int i = 0;
                            preprocessingParametersTemp = new PreprocessingParameterViewModel[newParameters.Count];
                            foreach (Entity entity in newParameters)
                            {
                                preprocessingParametersTemp[i] = new PreprocessingParameterViewModel();
                                preprocessingParametersTemp[i].Position = i + 1;
                              //  preprocessingParametersTemp[i].Id = entity.ID;// prepParam.ParameterId;
                                preprocessingParametersTemp[i].ParameterId = entity.ID;
                                preprocessingParametersTemp[i].ParameterName = ((models.Parameter)entity).Name;
                                preprocessingParametersTemp[i].Type = "бинаризация";
                                i++;
                                index++;
                                //int newSelectionId, int oldSelectionId, int oldParamId, string prepType, int parameterPosition, int newParamId
                                PreprocessingManager.PrepManager.executePreprocessing(newSelectionId, oldSelectionId, oldParamId, prepType, i - 1, entity.ID);
                                continue;
                            }
                        }
                        else
                        {
                            models.Parameter param = ((models.Parameter)services.DatabaseManager.SharedManager.entityById(oldParamId, typeof(models.Parameter)));
                            TypeParameter type = getTypeParameter(prepType, oldParamId);
                            int newParamId = new DataHelper().addOneParameter(prepParam.ParameterName, param.Comment, taskTemplateId, prepParam.Position, 
                                param.IsOutput, type);
                            PreprocessingManager.PrepManager.executePreprocessing(newSelectionId, oldSelectionId, oldParamId, prepType, parameterPos, newParamId);
                            index++;
                        }
                    }
                    if (preprocessingParametersTemp != null)
                    {
                        PreprocessingParameters = PreprocessingParameters.Concat(preprocessingParametersTemp).ToArray();
                    }
                }
            }
            
             OnClose?.Invoke(this, null);
        }

        private TypeParameter getTypeParameter(string prepType, int paramId)
        {
            switch (prepType)
            {
                case "Линейная нормализация 1 (к float)":
                    return TypeParameter.Real;
                case "Нелинейная нормализация 2 (к float)":
                    return TypeParameter.Real;
                case "нормализация 3 (к int)":
                    return TypeParameter.Int;
                case "бинаризация":
                    return TypeParameter.Int;
                case "без предобработки":
                    models.Parameter param = ((models.Parameter)services.DatabaseManager.SharedManager.entityById(paramId, typeof(models.Parameter)));
                    TypeParameter type = param.Type;
                    return type;
                default:
                    return TypeParameter.Real;
            }
        }

        public void Cancel()
        {
            OnClose?.Invoke(this, null);
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
