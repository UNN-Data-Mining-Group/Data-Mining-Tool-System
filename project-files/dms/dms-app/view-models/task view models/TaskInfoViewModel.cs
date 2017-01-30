using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using dms.tools;

namespace dms.view_models
{
    public class Parameter
    {
        public Parameter(string name, string type, string comment)
        {
            Name = name;
            Type = type;
            Comment = comment;
        }
        public string Name { get; }
        public string Type { get; }
        public string Comment { get; }
    }

    public class Preprocessing
    {
        public Preprocessing(string name, string baseTemplate, 
            string performedTemplate, Tuple<Parameter, string>[] preproc)
        {
            Name = name;
            BaseTemplateName = baseTemplate;
            PerformedTemplateName = performedTemplate;
            ParameterProcessing = preproc;
        }
        public string Name { get; }
        public string BaseTemplateName { get; }
        public string PerformedTemplateName { get; }
        public Tuple<Parameter, string>[] ParameterProcessing { get; }
    }

    public class TaskInfoViewModel : ViewmodelBase
    {
        private TemplateViewModel selectedTemplate;
        private ActionHandler moreHandler;
        private Preprocessing selectedPreprocessing;

        public TaskInfoViewModel(models.Task task)
        {
            var p1 = new Parameter("Параметр 1", "float", "");
            var p2 = new Parameter("Параметр 2", "int", "");
            var p3 = new Parameter("Параметр 3", "enum", "выходной параметр");

            TaskName = task.Name;
            PreprocessingList = new Preprocessing[]
            {
                new Preprocessing("Преобразование 1", "Один шаблон", "Другой шаблон", new Tuple<Parameter, string>[] 
                {
                    new Tuple<Parameter, string>(p1, "normalize"),
                    new Tuple<Parameter, string>(p2, "none"),
                    new Tuple<Parameter, string>(p3, "binarise")
                })
            };
            Templates = new TemplateViewModel []{ new TemplateViewModel("Один шаблон"), new TemplateViewModel("Другой шаблон", 1) };
            SelectedTemplate = Templates[0];

            moreHandler = new ActionHandler(() => OnShowPreprocessingDetails?.Invoke(new PreprocessingViewModel(taskId)), o => SelectedPreprocessing != null);
        }

        public string TaskName { get; }
        public TemplateViewModel[] Templates { get; }
        public TemplateViewModel SelectedTemplate
        {
            get { return selectedTemplate; }
            set
            {
                selectedTemplate = value;
                NotifyPropertyChanged();
            }
        }
        public Preprocessing[] PreprocessingList { get; }
        public Preprocessing SelectedPreprocessing
        {
            get { return selectedPreprocessing; }
            set { selectedPreprocessing = value; moreHandler.RaiseCanExecuteChanged(); }
        }
        public ICommand MoreCommand { get { return moreHandler; } }
        public event Action<PreprocessingViewModel> OnShowPreprocessingDetails;
    }
}
