using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using dms.models;

using dms.tools;

namespace dms.view_models
{
    [Serializable]
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
            string performedTemplate, Tuple<Parameter, string>[] preproc, int id)
        {
            PerformedTemplateId = id;
            Name = name;
            BaseTemplateName = baseTemplate;
            PerformedTemplateName = performedTemplate;
            ParameterProcessing = preproc;
        }
        public int PerformedTemplateId { get; }
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
            TaskName = task.Name;
            List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", task.ID.ToString()), typeof(TaskTemplate));

            List<TaskTemplate> prepTemplates = new List<TaskTemplate>();
            Templates = new TemplateViewModel[taskTemplates.Count];
            int step = 0;
            if (taskTemplates.Count != 0)
            {
                foreach (Entity template in taskTemplates)
                {
                    if (((TaskTemplate)template).PreprocessingParameters != null)
                    {
                        prepTemplates.Add((TaskTemplate)template);
                    }
                    else
                    {
                        Templates[step] = new TemplateViewModel(((TaskTemplate)template).ID, step);
                        step++;
                    }
                }
                PreprocessingList = new Preprocessing[prepTemplates.Count];

                int index = 0;
                foreach (TaskTemplate template in prepTemplates)
                {
                    PreprocessingViewModel.PreprocessingTemplate pp =
                        (PreprocessingViewModel.PreprocessingTemplate)template.PreprocessingParameters;
                    Dictionary<Parameter, string> dictionary = pp.get();
                    Tuple<Parameter, string>[] tuple = new Tuple<Parameter, string>[dictionary.Count];
                    int i = 0;
                    foreach (KeyValuePair<Parameter, string> kp in dictionary)
                    {
                        tuple[i] = new Tuple<Parameter, string>(kp.Key, kp.Value);
                        i++;
                    }
                    PreprocessingList[index] = new Preprocessing(pp.PreprocessingName, pp.BaseTemplate.Name, template.Name, tuple, template.ID);

                    index++;
                }
                SelectedTemplate = Templates[0];
            }

            moreHandler = new ActionHandler(() => OnShowPreprocessingDetails?.Invoke(new PreprocessingViewModel(task.ID, SelectedPreprocessing.PerformedTemplateId)), o => SelectedPreprocessing != null);
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
