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

        public PreprocessingViewModel(int taskId)
        {
            //TaskTemplates
            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", taskId.ToString()), typeof(TaskTemplate));
            BaseTemplateList = new string[taskTemplates.Count];
            int index = 0;
            foreach (Entity entity in taskTemplates)
            {
                BaseTemplateList[index] = ((TaskTemplate)entity).Name;
                    index++;
            }

            PerformedTemplateList = BaseTemplateList;
            PerformedTemplate = PerformedTemplateList[0]; 
            BaseTemplate = BaseTemplateList[0];
            IsUsingExitingTemplate = false;
            TaskName = ((dms.models.Task)dms.services.DatabaseManager.SharedManager.entityById(taskId, typeof(dms.models.Task))).Name;
            PreprocessingName = "Преобразование 1";

            //переписать
            int taskTemplateId = taskTemplates[0].ID;
            templateId = taskTemplateId;
            List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(dms.models.Parameter));
            PreprocessingParameters = new PreprocessingParameterViewModel[parameters.Count];
            index = 0;
          //  PreprocessingParameters = new PreprocessingParameterViewModel[5];
            foreach (Entity entity in parameters)
        //    for (int i = 0; i < 5; i++)
            {

                PreprocessingParameters[index] = new PreprocessingParameterViewModel();
                PreprocessingParameters[index].ParameterId = entity.ID;
                PreprocessingParameters[index].ParameterName = ((dms.models.Parameter)entity).Name;
                PreprocessingParameters[index].Type = "без предобработки";
                index++;
            }
            
            cancelHandler = new ActionHandler(Cancel, o => true);
            createHandler = new ActionHandler(Create, o => true);
        }

        public void Create()
        {
            //создание преобразования
            if (NewTemplateName != "") { }
            int index = 0;
            foreach (PreprocessingParameterViewModel prepParam in PreprocessingParameters)
            {
                int paramId = prepParam.ParameterId;
                string prepType = prepParam.Type;

                List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", templateId.ToString()), typeof(Selection));
                int selectionId = selections[0].ID;
                PreprocessingManager.PrepManager.executePreprocessing(selectionId, paramId, prepType);
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
        public string BaseTemplate { get; set; }
        public string PerformedTemplate { get; set; }
        public PreprocessingParameterViewModel[] PreprocessingParameters { get; private set; }
        public string[] BaseTemplateList { get; private set; }
        public string[] PerformedTemplateList { get; private set; }
        public bool IsUsingExitingTemplate { get; set; }
        public string NewTemplateName { get; set; }
        public TemplateViewModel BaseTemplateViewModel { get { return new TemplateViewModel(BaseTemplate); } }
        public TemplateViewModel PerformedTemplateViewModel { get { return new TemplateViewModel(PerformedTemplate, 1); } }
    }
}
