using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

using dms.tools;

namespace dms.view_models
{
    public interface ISolverParameterViewModel
    {
        void CreateSolver(string name, string taskName);
        bool CanCreateSolver(string name, string taskName);
        event Action CanCreateChanged;
    }

    public class SolverParameterFactory
    {
        public static string[] Types { get { return new string[] { "Персептрон", "Дерево решений" }; } }
        public static ISolverParameterViewModel Create(string type)
        {
            if (type.Equals("Персептрон"))
                return new PerceptronParametersViewModel();
            else if (type.Equals("Дерево решений"))
                return new DecisionTreeParametersViewModel();
            else
                return null;
        }
    }

    public class SolverCreationViewModel : ViewmodelBase
    {
        private string selectedType;
        private ActionHandler createHandler;
        private ActionHandler cancelHandler;
        private ISolverParameterViewModel parameters;

        public SolverCreationViewModel(string taskName)
        {
            TaskName = taskName;
            SolverName = "Решатель 1";
            SolverTypes = SolverParameterFactory.Types;
            SelectedType = SolverTypes[0];
            createHandler = new ActionHandler(CreateSolver, o => Parameters.CanCreateSolver(SolverName, TaskName));
            cancelHandler = new ActionHandler(() => OnClose?.Invoke(this, null), o => true);
        }

        public string TaskName { get; }
        public string SolverName { get; set; }
        public string SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                Parameters = SolverParameterFactory.Create(selectedType);
                Parameters.CanCreateChanged += () => createHandler.RaiseCanExecuteChanged();
                NotifyPropertyChanged();

                if (createHandler != null)
                    createHandler.RaiseCanExecuteChanged();
            }
        }
        public string[] SolverTypes { get; }
        public ISolverParameterViewModel Parameters { get { return parameters; } private set { parameters = value; NotifyPropertyChanged(); } }
        public ICommand Create { get { return createHandler; } }
        public ICommand Cancel { get { return cancelHandler; } }
        public EventHandler OnClose;

        public void CreateSolver()
        {
            Parameters.CreateSolver(SolverName, TaskName);
            OnClose?.Invoke(this, null);
        }
    }
}
