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
        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public string Type { get; set; }
        public string[] TypesList { get { return new string[] { "Линейная нормализация 1 (к float)", "Нелинейная нормализация 2 (к float)", "нормализация 3 (к int)", /*"бинаризация",*/ "без предобработки" }; } }
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

        [Serializable]
        public class PrepParameter
        {
            public float maxValue { get; set; }
            public float minValue { get; set; }
            public float centerValue { get; set; }
            public float countValues { get; set; }
            public float countNumbers { get; set; }
        }

        [Serializable()]
        public class ValuesForParameter
        {
            public int parameterId { get; set; }
            public List<Entity> values = new List<Entity>();
        }

        [Serializable()]
        public class SerializableList
        {
            public int selectionId { get; set; }
            public List<int> parameterIds = new List<int>();
            public List<services.preprocessing.normalization.IParameter> prepParameters = new List<services.preprocessing.normalization.IParameter>();
            public List<ValuesForParameter> parametersValues = new List<ValuesForParameter>();
        }

        [Serializable()]
        public class PreprocessingTemplate : IPreprocessingParameters
        {
            public string PreprocessingName { get; set; }
            public Pair BaseTemplate { get; set; }
            public List<SerializableList> info = new List<SerializableList>();
            public List<Parameter> parameters = new List<Parameter>();
            public List<string> types { get; set; }

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

        public float rightRangeValue;
        public float leftRangeValue;

        public Pair BaseTemplate { get; set; }
        public Pair PerformedTemplate { get; set; }

        public string[] BaseTemplateList { get; private set; }
        public string[] PerformedTemplateList { get; private set; }

        public int[] BaseTemplateIdList { get; private set; }
        public int[] PerformedTemplateIdList { get; private set; }

        public TemplateViewModel BaseTemplateViewModel { get { return new TemplateViewModel(BaseTemplate.Id); } }
        public TemplateViewModel PerformedTemplateViewModel { get { return new TemplateViewModel(PerformedTemplate.Id, 1); } }

        public PreprocessingViewModel(models.Task task, int templateId)
        {
            IntervalNameA = "A = ";
            IntervalNameB = "B = ";
            //Параметр влияет на степень нелинейности изменения переменной в нормализуемом интервале
            AName = "(Параметр нормализации) a = ";
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

                //Определение индекса последней выборки
                string templateNameForEmptyField = "Шаблон " + ((taskTemplates != null) ? taskTemplates.Count + 1 : 1);

                NewTemplateName = templateNameForEmptyField;//"Шаблон для " + PreprocessingName;
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

                TaskTemplate template_temp = ((TaskTemplate)services.DatabaseManager.SharedManager.entityById(TemplateId, typeof(TaskTemplate)));
                PreprocessingTemplate pt = (PreprocessingTemplate)template_temp.PreprocessingParameters;

                if (pt == null)
                {
                    Pair pair_2 = new Pair();
                    pair_2.Id = BaseTemplateIdList[0];
                    pair_2.Name = BaseTemplateList[0];
                    BaseTemplate = pair_2;
                }
                else
                {
                    BaseTemplate = pt.BaseTemplate;
                }


                List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                    .addCondition("TaskTemplateID", "=", TemplateId.ToString()), typeof(models.Parameter));
                PreprocessingParameters = new PreprocessingParameterViewModel[parameters.Count];
                index = 0;
                foreach (Entity entity in parameters)
                {
                    PreprocessingParameters[index] = new PreprocessingParameterViewModel();
                    PreprocessingParameters[index].ParameterId = entity.ID;
                    PreprocessingParameters[index].ParameterName = ((models.Parameter)entity).Name;
                    PreprocessingParameters[index].Position = ((models.Parameter)entity).Index;
                    if (template == null)
                    {
                        PreprocessingParameters[index].Type = "без предобработки";
                    }
                    else
                    {
                        PreprocessingTemplate pp = (PreprocessingTemplate)template.PreprocessingParameters;
                        List<Parameter> list = pp.parameters;
                        List<string> types = pp.types;
                        PreprocessingParameters[index].Type = types[index];
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
            float left = 0, right = 1, a = 0;
            if (IntervalValueA != null && IntervalValueB != null)
            {
                left = float.Parse(IntervalValueA.Replace(".", ","));
                right = float.Parse(IntervalValueB.Replace(".", ","));
            }
            if (AValue != null)
            {
                a = float.Parse(AValue);
            }
            
            if (PerformedTemplate != null)
            {
                for (int i = 0; i < PerformedTemplateList.Count(); i++)
                {
                    if (PerformedTemplateList[i].Equals(PerformedTemplate.Name))
                    {
                        PerformedTemplate.Id = PerformedTemplateIdList[i];
                        break;
                    }
                }
            }
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

                List<Entity> ps = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                            .addCondition("TaskTemplateID", "=", BaseTemplate.Id.ToString()), typeof(models.Parameter));
                for (int i = 0; i < PreprocessingParameters.Count(); i++)
                {
                    PreprocessingParameters[i].ParameterId = ps[i].ID;
                    PreprocessingParameters[i].ParameterName = ((models.Parameter)ps[i]).Name;
                }

                List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", BaseTemplate.Id.ToString()), typeof(Selection));

                int newTaskTemplateId;
                PreprocessingTemplate pp = new PreprocessingTemplate();
                if (IsUsingExitingTemplate)
                {
                    newTaskTemplateId = TemplateId;
                }
                else
                {
                    pp.PreprocessingName = PreprocessingName;
                    pp.BaseTemplate = BaseTemplate;
                    string templateName = (NewTemplateName == null || NewTemplateName == "") ? "New Preprocessing Template" : NewTemplateName;
                    newTaskTemplateId = new DataHelper().addTaskTemplate(templateName, Task.ID, pp);
                }

                List<PreprocessingParameterViewModel> preprocessingParametersTemp = new List<PreprocessingParameterViewModel>();
                List<string> types = new List<string>();
                List<Parameter> parameters = new List<Parameter>();
                List<int> paramIds = new List<int>();
                foreach (Entity sel in selections)
                {
                    List<services.preprocessing.normalization.IParameter> listOfIParameters = new List<services.preprocessing.normalization.IParameter>();
                    List<ValuesForParameter> listOfValuesForParameter = new List<ValuesForParameter>();
                    int newSelectionId = PreprocessingManager.PrepManager.addNewEntitiesForPreprocessing(
                            ((Selection)sel).Name, ((Selection)sel).RowCount, Task.ID, newTaskTemplateId);
                    int oldSelectionId = sel.ID;
                    foreach (PreprocessingParameterViewModel prepParam in PreprocessingParameters)
                    {
                        string prepType = prepParam.Type;
                        int oldParamId = prepParam.ParameterId;

                        if ("бинаризация".Equals(prepType))
                        {
                            bool canCreate = true;
                            List<Entity> newParameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                            .addCondition("TaskTemplateID", "=", newTaskTemplateId.ToString()), typeof(models.Parameter));
                            List<Entity> newAddedParameters = new List<Entity>();
                            List<string> classes = PreprocessingManager.PrepManager.getClasses(oldSelectionId, newTaskTemplateId, oldParamId);
                            foreach (Entity entity in newParameters)
                            {
                                //TypeParameter type = getTypeParameter(prepType, oldParamId);
                                if (classes.Contains(((models.Parameter)entity).Name))
                                //(((models.Parameter)entity).Name.Equals(prepParam.ParameterName)) && ((models.Parameter)entity).Type.Equals(type)) ?????
                                {
                                    newAddedParameters.Add(entity);
                                    canCreate = false;
                                }
                            }
                            if (canCreate)
                            {
                                newParameters = PreprocessingManager.PrepManager.getNewParametersForBinarizationType(oldSelectionId, newTaskTemplateId, oldParamId);
                            }
                            else
                            {
                                newParameters = newAddedParameters;
                            }

                            int i = 0;
                            foreach (Entity entity in newParameters)
                            {
                                PreprocessingParameterViewModel ppVM = new PreprocessingParameterViewModel();
                                ppVM.Position = i + 1;
                                ppVM.ParameterId = entity.ID;
                                ppVM.ParameterName = ((models.Parameter)entity).Name;
                                ppVM.Type = "бинаризация";
                                if (canCreate)
                                {
                                    preprocessingParametersTemp.Add(ppVM);
                                    types.Add("бинаризация");
                                    parameters.Add(new view_models.Parameter(ppVM.ParameterName, ppVM.Type, ((models.Parameter)entity).Comment, entity.ID));
                                }

                                i++;
                                Dictionary<List<Entity>, services.preprocessing.normalization.IParameter> output = PreprocessingManager.PrepManager.executePreprocessing(newSelectionId, 
                                                                                                                                                                         oldSelectionId,
                                                                                                                                                                         oldParamId, 
                                                                                                                                                                         prepType, 
                                                                                                                                                                         i - 1,
                                                                                                                                                                         entity.ID,
                                                                                                                                                                         left,
                                                                                                                                                                         right,
                                                                                                                                                                         a);
                                services.preprocessing.normalization.IParameter p = output.Values.ElementAt(0);
                                List<Entity> valuesForParameter = output.Keys.ElementAt(0);
                                continue;
                            }
                        }
                        else
                        {
                            models.Parameter oldParam = ((models.Parameter)services.DatabaseManager.SharedManager.entityById(oldParamId, typeof(models.Parameter)));

                            TypeParameter type = getTypeParameter(prepType, oldParamId);
                            bool canCreate = true;
                            int newParamId = -1;
                            List<Entity> newParameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                            .addCondition("TaskTemplateID", "=", newTaskTemplateId.ToString()), typeof(models.Parameter));
                            foreach (Entity entity in newParameters)
                            {
                                if (((models.Parameter)entity).Name.Equals(prepParam.ParameterName) && ((models.Parameter)entity).Type.Equals(type))
                                {
                                    newParamId = entity.ID;
                                    canCreate = false;
                                    break;
                                }
                            }
                            if (canCreate)
                            {
                                newParamId = new DataHelper().addOneParameter(prepParam.ParameterName, oldParam.Comment, newTaskTemplateId, prepParam.Position,
                                    oldParam.IsOutput, type);
                                PreprocessingParameterViewModel ppVM = new PreprocessingParameterViewModel();
                                ppVM.ParameterName = prepParam.ParameterName;
                                ppVM.Position = prepParam.Position;
                                ppVM.Type = prepParam.Type;
                                ppVM.ParameterId = newParamId;

                                parameters.Add(new view_models.Parameter(prepParam.ParameterName, prepParam.Type, oldParam.Comment, newParamId));
                                types.Add(prepType);
                                preprocessingParametersTemp.Add(ppVM);
                            }

                            Dictionary<List<Entity>, services.preprocessing.normalization.IParameter> output = PreprocessingManager.PrepManager.executePreprocessing(newSelectionId, 
                                                                                                                                                                     oldSelectionId, 
                                                                                                                                                                     oldParamId, 
                                                                                                                                                                     prepType,
                                                                                                                                                                     prepParam.Position, 
                                                                                                                                                                     newParamId, 
                                                                                                                                                                     left, 
                                                                                                                                                                     right,
                                                                                                                                                                     a);
                            services.preprocessing.normalization.IParameter p = output.Values.ElementAt(0);
                            List<Entity> valuesForParameter = output.Keys.ElementAt(0);
                            //->
                            ValuesForParameter valuesList = new ValuesForParameter();
                            valuesList.parameterId = newParamId;
                            valuesList.values = valuesForParameter;

                            listOfValuesForParameter.Add(valuesList);
                            //<-
                            listOfIParameters.Add(p);
                            paramIds.Add(newParamId);
                        }
                    }
                    SerializableList list = new SerializableList();
                    list.selectionId = newSelectionId;
                    list.parameterIds = paramIds;
                    list.prepParameters = listOfIParameters;
                    list.parametersValues = listOfValuesForParameter;
                    pp.info.Add(list);

                    new DataHelper().updateTaskTemplate(newTaskTemplateId, pp);
                }
                if (parameters != null && types != null)
                {
                    pp.parameters = parameters;
                    pp.types = types;
                    PreprocessingManager.PrepManager.updateTaskTemplate(newTaskTemplateId, pp);
                }
                if (preprocessingParametersTemp != null)
                {
                    PreprocessingParameters = preprocessingParametersTemp.ToArray();
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

        private string intervalNameA;
        private string intervalValueA;
        public string IntervalNameA
        {
            get
            {
                return intervalNameA;
            }
            set
            {
                intervalNameA = value;
                NotifyPropertyChanged("IntervalNameA");
            }
        }

        public string IntervalValueA
        {
            get
            {
                return intervalValueA;
            }
            set
            {
                intervalValueA = value;
                NotifyPropertyChanged("IntervalValueA");
            }
        }

        private string intervalNameB;
        private string intervalValueB;
        public string IntervalNameB
        {
            get
            {
                return intervalNameB;
            }
            set
            {
                intervalNameB = value;
                NotifyPropertyChanged("IntervalNameB");
            }
        }

        public string IntervalValueB
        {
            get
            {
                return intervalValueB;
            }
            set
            {
                intervalValueB = value;
                NotifyPropertyChanged("IntervalValueB");
            }
        }

        private string aName;
        private string aValue;
        public string AName
        {
            get
            {
                return aName;
            }
            set
            {
                aName = value;
                NotifyPropertyChanged("AName");
            }
        }

        public string AValue
        {
            get
            {
                return aValue;
            }
            set
            {
                aValue = value;
                NotifyPropertyChanged("AValue");
            }
        }
    }
}