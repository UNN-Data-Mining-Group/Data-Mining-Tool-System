using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using dms.tools;
using dms.models;
using System.Collections.Generic;

namespace dms.view_models
{
    public class LearningAlgoParamsViewModel : ViewmodelBase
    {
        public string[] ParamsName { get; }
        public float[] ParamsValue { get; set; }
        public LearningAlgoParamsViewModel(string[] parNames, float[] parValues)
        {
            ParamsName = parNames;
            ParamsValue = parValues;
        }
    }

    public class VectorQuantumParamsViewModel : LearningAlgoParamsViewModel
    {
        public VectorQuantumParamsViewModel(string[] parNames, float[] parValues) 
            : base(parNames, parValues) {}
        public bool HasPretrain
        {
            get { return ParamsValue[0] == 1.0f; }
            set { ParamsValue[0] = (value == true ? 1.0f : 0.0f); }
        }
        public float Eps
        {
            get { return ParamsValue[1]; }
            set { ParamsValue[1] = value; }
        }
        public int Iterations
        {
            get { return Convert.ToInt32(ParamsValue[2]); }
            set { ParamsValue[2] = value; }
        }
        public int Seed
        {
            get { return Convert.ToInt32(ParamsValue[3]); }
            set { ParamsValue[3] = value; }
        }
        public float A
        {
            get { return ParamsValue[4]; }
            set { ParamsValue[4] = value; }
        }
        public float B
        {
            get { return ParamsValue[5]; }
            set { ParamsValue[5] = value; }
        }
        public bool Normalize
        {
            get { return ParamsValue[6] == 1.0f; }
            set { ParamsValue[6] = (value == true ? 1.0f : 0.0f); }
        }
    }

    public class SelfOrganizerParamsViewModel : LearningAlgoParamsViewModel
    {
        public SelfOrganizerParamsViewModel(string[] parNames, float[] parValues) 
            : base(parNames, parValues) {}
        public int Iterations
        {
            get { return Convert.ToInt32(ParamsValue[0]); }
            set { ParamsValue[0] = value; }
        }
        public int Seed
        {
            get { return Convert.ToInt32(ParamsValue[1]); }
            set { ParamsValue[1] = value; }
        }
        public float Sigma0
        {
            get { return ParamsValue[2]; }
            set { ParamsValue[2] = value; }
        }
        public float L0
        {
            get { return ParamsValue[3]; }
            set { ParamsValue[3] = value; }
        }
        public float minLearningRate
        {
            get { return ParamsValue[4]; }
            set { ParamsValue[4] = value; }
        }
        public float Eps
        {
            get { return ParamsValue[5]; }
            set { ParamsValue[5] = value; }
        }
        public bool Normalize
        {
            get { return ParamsValue[6] == 1.0f; }
            set { ParamsValue[6] = (value == true ? 1.0f : 0.0f); }
        }
    }

    public class LearningScenarioViewModel : ViewmodelBase
    {
        private ActionHandler createHandler;
        private ActionHandler cancelHandler;
        private LearningAlgoManager learningAlgo;
        private string selectionType;
        private string separationParamName;
        private string separationParamValue;
        private LearningAlgoParamsViewModel paramsVm;

        public LearningScenarioViewModel()
        {
            createHandler = new ActionHandler(createScenario, e => true);
            cancelHandler = new ActionHandler(() => OnClose?.Invoke(this, null), e => true);
            learningAlgo = new LearningAlgoManager();
            Name = "Сценарий";
            MixSeed = "123";
            SeparationParamName = SeparationParamNames[0];
            TeacherType = TeacherTypesList[0];
            SelectionType = SelectionTypesList[0];
            ID = -1;
            SeparationParamValue = "80";
        }
        public  ILAParameters AlgoParam
        {
            set
            {
                learningAlgo.LAParams = value;
            }
            get
            {
                return learningAlgo.LAParams;
            }
        }

        public string Name { get; set; }
        public int ID { get; set; }
        public string TeacherType
        {
            get
            {
                return learningAlgo.usedAlgo;
            }
            set
            {
                learningAlgo.usedAlgo = value;
                if (learningAlgo.usedAlgo == "Векторное квантование")
                    ParamsViewModel = new VectorQuantumParamsViewModel(learningAlgo.paramsName, learningAlgo.paramsValue);
                else if (learningAlgo.usedAlgo == "Самоорганизация Кохонена")
                    ParamsViewModel = new SelfOrganizerParamsViewModel(learningAlgo.paramsName, learningAlgo.paramsValue);
                else
                    ParamsViewModel = new LearningAlgoParamsViewModel(learningAlgo.paramsName, learningAlgo.paramsValue);
                NotifyPropertyChanged();
            }
        }
        public LearningAlgoParamsViewModel ParamsViewModel
        {
            get { return paramsVm; }
            private set { paramsVm = value; NotifyPropertyChanged(); }
        }
        public string SelectionType
        {
            get
            {
                return selectionType;
            }
            set
            {
                selectionType = value;
                if (selectionType.Equals(SelectionTypesList[0]))
                {
                    SeparationParamName = SeparationParamNames[0];
                    SeparationParamValue = "80";
                }
                else
                {
                    SeparationParamName = SeparationParamNames[1];
                    SeparationParamValue = "5";
                }
                NotifyPropertyChanged("SelectionType");

            }
        }

        public string SeparationParamName
        {
            get
            {
                return separationParamName;
            }
            set
            {
                separationParamName = value;
                NotifyPropertyChanged("SeparationParamName");
            }
        }
        public string MixSeed { get; set; }
        public string SeparationParamValue
        {
            get
            {
                return separationParamValue;
            }
            set
            {
                separationParamValue = value;
                NotifyPropertyChanged("SeparationParamValue");
            }
        }
        public string[] TeacherTypesList { get { return learningAlgo.teacherTypesList; } }
        public string[] SelectionTypesList { get { return new string[] { "Тестовая/обучающая", "Кроссвалидация" }; } }
        public string[] SeparationParamNames { get { return new string[] { "Процент на обучающую выборку", "Число разделений для kfold" }; } }
        public ICommand CreateCommand { get { return createHandler; } }
        public ICommand CancelCommand { get { return cancelHandler; } }
        public event EventHandler OnClose;

        public void createScenario() {
            LearningScenario ls = new LearningScenario() {
                Name = this.Name,
                LearningAlgorithmName = TeacherType,
                LAParameters = AlgoParam,
                SelectionParameters = SelectionType + "," + MixSeed + "," + SeparationParamValue
            };
            ls.save();
            ID = ls.ID;
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
            deleteHandler = new ActionHandler(() => removeLearningScenario(SelectedScenario), e => SelectedScenario != null);
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
                    SelectionType = learningScenario.SelectionParameters.Split(',')[0],
                    MixSeed = learningScenario.SelectionParameters.Split(',')[1],
                    SeparationParamValue = learningScenario.SelectionParameters.Split(',')[2],
                    TeacherType = learningScenario.LearningAlgorithmName,
                    AlgoParam = learningScenario.LAParameters,
                    ID = learningScenario.ID
                });
            }
            NotifyPropertyChanged();
        }

        private void removeLearningScenario(LearningScenarioViewModel vm)
        {
            foreach (LearningScenario learningScenario in listLearningScenarion)
            {
                if (learningScenario.ID == vm.ID)
                    learningScenario.delete();
            }
            NotifyPropertyChanged();
        }
    }
}
