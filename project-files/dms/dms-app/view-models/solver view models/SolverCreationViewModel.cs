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
        void CreateSolver(string name, models.Task task);
        bool CanCreateSolver(string name, models.Task task);
        event Action CanCreateChanged;
    }

    public class SolverParameterFactory
    {
        public static string[] Types { get { return new string[] { "Персептрон", "Сеть Ворда", "Дерево решений CART", "Дерево решений C4.5", "Сверточная нейронная сеть", "Сеть Кохонена" }; } }
        public static ISolverParameterViewModel Create(string type)
        {
            if (type.Equals("Персептрон"))
                return new PerceptronParametersViewModel();
            else if (type.Equals("Дерево решений CART"))
                return new DecisionTreeCARTParametersViewModel();
            else if (type.Equals("Дерево решений C4.5"))
                return new DecisionTreeC4_5ParametersViewModel();
            else if (type.Equals("Сеть Ворда"))
                return new WardNetParametersViewModel();
            else if (type.Equals("Сверточная нейронная сеть"))
                return new ConvNNParametersViewModel();
            else if (type.Equals("Сеть Кохонена"))
                return new KohonenParametersViewModel();
            else
                return null;
        }
    }

    public class SolverCreationViewModel : ViewmodelBase
    {
        private models.Task parentTask;
        private string selectedType;
        private ActionHandler createHandler;
        private ActionHandler cancelHandler;
        private ISolverParameterViewModel parameters;

        public SolverCreationViewModel(models.Task task)
        {
            parentTask = task;
            TaskName = task.Name;
            SolverName = "Решатель 1";
            SolverTypes = SolverParameterFactory.Types;
            SelectedType = SolverTypes[0];
            createHandler = new ActionHandler(CreateSolver, o => Parameters.CanCreateSolver(SolverName, parentTask));
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
        public ISolverParameterViewModel Parameters
        {
            get
            {
                return parameters;
            }
            private set
            {
                parameters = value;
                NotifyPropertyChanged();
            }
        }
        public ICommand Create { get { return createHandler; } }
        public ICommand Cancel { get { return cancelHandler; } }
        public event EventHandler OnClose;

        public void CreateSolver()
        {
            Parameters.CreateSolver(SolverName, parentTask);
            OnClose?.Invoke(this, null);
        }
    }
}
