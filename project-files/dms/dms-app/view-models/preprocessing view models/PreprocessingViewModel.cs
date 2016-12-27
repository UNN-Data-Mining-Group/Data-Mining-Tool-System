using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class PreprocessingParameterViewModel
    {
        public string ParameterName { get; set; }
        public string Type { get; set; }
        public string[] TypesList { get { return new string[] { "нормализация", "бинаризация", "без предобработки" }; } }
    }

    public class PreprocessingViewModel : ViewmodelBase
    {
        public PreprocessingViewModel(string taskName)
        {
            PreprocessingName = "Преобразование 1";
            PreprocessingParameters = new PreprocessingParameterViewModel[]
            {
                new PreprocessingParameterViewModel
                {
                    ParameterName = "Параметр 1",
                    Type = "нормализация"
                },
                new PreprocessingParameterViewModel
                {
                    ParameterName = "Параметр 2",
                    Type = "без предобработки"
                },
                new PreprocessingParameterViewModel
                {
                    ParameterName = "Параметр 3",
                    Type = "бинаризация"
                }
            };
            BaseTemplateList = new string[]
            {
                "Шаблон 1",
                "Шаблон 2"
            };
            PerformedTemplateList = BaseTemplateList;
            PerformedTemplate = PerformedTemplateList[1];
            BaseTemplate = BaseTemplateList[0];
            IsUsingExitingTemplate = false;
            TaskName = taskName;
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
