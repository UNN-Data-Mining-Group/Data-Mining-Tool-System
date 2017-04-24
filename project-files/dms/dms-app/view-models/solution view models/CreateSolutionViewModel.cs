using dms.models;
using dms.tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace dms.view_models
{
    public class CreateSolutionViewModel : ViewmodelBase
    {
        private ActionHandler createHandler;
        private string[] templates;
        private string templateName;
        private string name;
        public event EventHandler OnClose;

        public string TaskName { get; }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
                if (CanSave())
                    createHandler.RaiseCanExecuteChanged();
            }
        }
        public string TemplateName
        {
            get
            {
                return templateName;
            }
            set
            {
                templateName = value;
                NotifyPropertyChanged("TemplateName");
                if (CanSave())
                    createHandler.RaiseCanExecuteChanged();
            }
        }
        public models.Task Task { get; set; }
        public CreateSolutionViewModel(string taskName)
        {
            canSave = true;
            Task = (models.Task)models.Task.where(new Query("Task").addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", taskName), typeof(models.Task))[0];
            createHandler = new ActionHandler(createSolution, e => CanSave());
            TaskName = taskName;
            TemplateName = Templates[0];
        }

        private void createSolution()
        {
            TaskTemplate taskTemplate = (TaskTemplate) TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", TemplateName), typeof(TaskTemplate))[0];
            Selection selection = new Selection()
            {
                Name = Name,
                Type = "solution",
                TaskTemplateID = taskTemplate.ID
            };
            selection.save();
            OnClose?.Invoke(this, null);
        }

        public ICommand CreateCommand { get { return createHandler; } }
        public string[] Templates
        {
            get
            {
                List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                    .addCondition("TaskID", "=", Convert.ToString(Task.ID)), typeof(TaskTemplate));
                List<string> listTemplates = new List<string>();
                foreach (TaskTemplate tasktemplate in taskTemplates)
                {
                    listTemplates.Add(tasktemplate.Name);
                }
                if (listTemplates.Count == 0)
                    canSave = false;
                TemplateName = listTemplates[0];
                if (CanSave())
                    createHandler.RaiseCanExecuteChanged();
                return listTemplates.ToArray();
            }
            set
            {
                templates = value;
                NotifyPropertyChanged("Templates");
            }
        }

        public bool canSave { get; private set; }

        public bool CanSave() {
            if (Name == null || Name.Equals("") || !canSave)
                return false;
            return true;
        }
    }
}
