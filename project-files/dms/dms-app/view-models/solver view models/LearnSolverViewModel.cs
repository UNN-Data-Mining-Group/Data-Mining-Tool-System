using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;
using dms.models;
using dms.solvers.neural_nets.perceptron;
using dms.solvers;

namespace dms.view_models
{
    public class LearningModel : ViewmodelBase
    {
        private ActionHandler deleteHandler;
        private int number;
        private string selectedSelection;
        private string selectedScenario;

        public LearningModel(LearnSolverViewModel main)
        {
            deleteHandler = new ActionHandler(() => main.Delete(this), e => true);
            selectedSelection = Selections[0];
            selectedScenario = LearningScenarios[0];
            SelectedPreprocessing = Preprocessings[0];
            Number = -1;
            LearningScenarioID = -1;
            SelectionID = -1;
        }

        public int Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        public string[] Selections {
            get
            {
                listSelections = Entity.all(typeof(Selection));
                string[] nameSelections = new string[listSelections.Count];
                int i = 0;
                foreach (Selection ls in listSelections)
                {
                    if (i == 0)
                        SelectionID = ls.ID;
                    nameSelections[i] = ls.Name;
                }
                if (nameSelections.Length == 0)
                    return new string[] { "Нет созданных выборок" };
                else return nameSelections;
            }
        }
        public string[] LearningScenarios { get
            {
                listLearningScenarios = Entity.all(typeof(LearningScenario));
                string[] nameLearningScenarios = new string[listLearningScenarios.Count];
                int i = 0;
                foreach (LearningScenario ls in listLearningScenarios)
                {
                    if (i == 0)
                        LearningScenarioID = ls.ID;
                    nameLearningScenarios[i] = ls.Name;
                }
                if (nameLearningScenarios.Length == 0)
                    return new string[] { "Нет созданных сценариев" };
                else return nameLearningScenarios;
            }
        }
        public string[] Preprocessings { get { return new string[] { "Преобразование 1", "Преобразование 2" }; } }
        public string SelectedSelection
        {
            get
            {
                return selectedSelection;
            }
            set
            {
                selectedSelection = value;
            }
        }
        public string SelectedScenario
        {
            get
            {
                return selectedScenario;
            }
            set
            {
                selectedScenario = value;
            }
        }
        public int SelectionID { get; set; }
        public int LearningScenarioID { get; set; }
        public string SelectedPreprocessing { get; set; }
        public ICommand DeleteCommand { get { return deleteHandler; } }
        public List<Entity> listLearningScenarios { get; private set; }
        public List<Entity> listSelections { get; private set; }
    }

    public class LearnSolverViewModel : ViewmodelBase
    {
        private ActionHandler addHandler;
        private ActionHandler learnHandler;
        private TaskSolver solver;

        public LearnSolverViewModel(string taskName, TaskSolver taskSolver)
        {
            TaskName = taskName;
            SolverName = taskSolver.Name;
            solver = taskSolver;
            LearningList = new ObservableCollection<LearningModel>();
            addHandler = new ActionHandler(() => Add(new LearningModel(this)), e => true);
            learnHandler = new ActionHandler(customLearningSolver, e => true);
        }

        public string TaskName { get; }
        public string SolverName { get; }
        public TaskSolver Solver {
            get
            {
                return solver;
            }
        }
        public ObservableCollection<LearningModel> LearningList { get; }
        public ICommand AddCommand { get { return addHandler; } }
        public ICommand LearnCommand { get { return learnHandler; } }
        public List<Entity> listLearningScenarion { get; private set; }

        public void Delete(LearningModel l)
        {
            LearningList.Remove(l);
            for(int i = 0; i < LearningList.Count; i++)
            {
                LearningList[i].Number = i + 1;
            }
        }
        public void Add(LearningModel l)
        {
            LearningList.Add(l);
            l.Number = LearningList.Count;
        }

        private void learnSolver() {
            foreach (LearningModel learningModel in LearningList) {
                LearningAlgo la = new LearningAlgo()
                {
                    //GeneticParams = learningModel.LearningScenarios,
                    
                };
            }
        }
        private void customLearningSolver() {
            LearningScenario learningScenarios = (LearningScenario) LearningScenario.getById(1, typeof(LearningScenario));
            LearningAlgo la = new LearningAlgo()
            {
                GeneticParams = (GeneticParam) learningScenarios.LAParameters,
                usedAlgo = learningScenarios.LearningAlgorithmName
            };
            float[][] x = new float[][] 
            {
                new float[] { 0, 0.3f, 1f, 5f }
            };
            float[] y = new float[] { 2f };
            PerceptronTopology topology = Solver.Description as PerceptronTopology;
            ISolver isolver = new PerceptronManaged(topology);
            la.startLearn(isolver, x, y);
        }
    }
}
