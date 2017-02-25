using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using dms.tools;
using dms.models;
using System.Collections.Generic;

namespace dms.view_models
{
    public class LearningScenarioViewModel : ViewmodelBase
    {
        private ActionHandler createHandler;
        private ActionHandler cancelHandler;
        private LearningAlgo learningAlgo;
        private String selectionType;
        public LearningScenarioViewModel()
        {
            createHandler = new ActionHandler(createScenario, e => true);
            cancelHandler = new ActionHandler(() => OnClose?.Invoke(this, null), e => true);
            learningAlgo = new LearningAlgo();
            Name = "Сценарий";
            MixSeed = "123";
            TeacherType = TeacherTypesList[0];
            SelectionType = SelectionTypesList[0];
        }

        public string[] ParamsName { get { return learningAlgo.paramsName; } }
        public float[] ParamsValue
        {
            get
            {
                return learningAlgo.paramsValue;
            }
        }

        public string Name { get; set; }
        public string TeacherType { get; set; }
        public string SelectionType
        {
            get
            {
                return selectionType;
            }
            set
            {
                learningAlgo.usedAlgo = value;
                selectionType = value;
            }
        }
        public string MixSeed { get; set; }
        public string[] TeacherTypesList { get { return learningAlgo.teacherTypesList; } }
        public string[] SelectionTypesList { get { return new string[] { "Тестовая/обучающая", "Кроссвалидация" }; } }
        public ICommand CreateCommand { get { return createHandler; } }
        public ICommand CancelHandler { get { return cancelHandler; } }
        public event EventHandler OnClose;

        public void createScenario() {
            LearningScenario ls = new LearningScenario() {
                Name = this.Name,
                LearningAlgorithmName = TeacherType,
                LAParameters = learningAlgo.GeneticParams,
                SelectionParameters = SelectionType + "," + MixSeed
            };

            ls.save();
            OnClose?.Invoke(this, null);
        }

    }

    public class LearningScenarioManagerViewModel : ViewmodelBase
    {
        private LearningScenarioViewModel selectedScenario;
        private ActionHandler propertiesHandler;
        private ActionHandler deleteHandler;
        private ActionHandler createHandler;
        private LearningScenario learningScenario;

        public LearningScenarioManagerViewModel()
        {
            learningScenario = new LearningScenario();
            listLearningScenarion = Entity.all(typeof(LearningScenario));
            ScenarioList = new ObservableCollection<LearningScenarioViewModel>();
            updateLearningScenariosInfo();
            propertiesHandler = new ActionHandler(() => requestShowLearningScenario?.Invoke(SelectedScenario), e => SelectedScenario != null);
            deleteHandler = new ActionHandler(() => { }, e => SelectedScenario != null);
            createHandler = new ActionHandler(() => requestCreateLearningScenario?.Invoke(), e => true);
        }

        public event Action requestCreateLearningScenario;
        public event Action<LearningScenarioViewModel> requestShowLearningScenario;
        public ObservableCollection<LearningScenarioViewModel> ScenarioList { get; private set; }
        public List<Entity> listLearningScenarion { get; private set; }
        public LearningScenarioViewModel SelectedScenario 
        {
            get { return selectedScenario; }
            set 
            { 
                selectedScenario = value;
                deleteHandler.RaiseCanExecuteChanged();
                propertiesHandler.RaiseCanExecuteChanged();
            }
        }
        public ICommand PropertiesCommand { get { return propertiesHandler; } }
        public ICommand DeleteCommand { get { return deleteHandler; } }
        public ICommand CreateCommand { get { return createHandler; } }

        private void updateLearningScenariosInfo() {
            foreach (LearningScenario learningScenario in listLearningScenarion)
            {
                ScenarioList.Add(new LearningScenarioViewModel
                {
                    Name = learningScenario.Name,
                    SelectionType = learningScenario.SelectionParameters.Split(',')[0]
                });
            }
        }

        private void removeLearningScenario(LearningScenarioViewModel vm)
        {
            
        }
    }
}
