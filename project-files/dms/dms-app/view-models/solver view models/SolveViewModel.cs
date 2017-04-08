using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;
using dms.solvers.neural_nets.perceptron;
using dms.solvers.neural_nets;
using dms.solvers.neural_nets.conv_net;
using dms.solvers.neural_nets.ward_net;
using dms.models;
using dms.solvers;
using dms.services.preprocessing;

namespace dms.view_models
{
    public class LearningInfo
    {
        public string SelectionName { get; set; }
        public string LearningScenarioName { get; set; }
        public string PreprocessingName { get; set; }
        public float TrainMistake { get; set; }
        public float TestMistake { get; set; }
        public float ClosingError { get; set; }
        public int SelectionID { get; set; }
        public int ParameterID { get; set; }

        public models.TaskTemplate TaskTemplate { get; set; }
        public models.LearnedSolver LearnedSolver { get; set; }
    }

    public class X : ViewmodelBase
    {
        private string value;
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                NotifyPropertyChanged();
                this.value = value;
            }
        }
        public string ParameterDescription { get; set; }
    }

    public class SolvingInstance : ViewmodelBase
    {
        private ActionHandler deleteHandler;

        public ObservableCollection<X> X { get; set; } 
        public ObservableCollection<string> Y { get; set; }

        public SolvingInstance(SolveViewModel vm, models.TaskTemplate template)
        {
            deleteHandler = new ActionHandler(() => vm.DeleteSolvingInstance(this), e => true);
            List<models.Parameter> parameters = models.Parameter.parametersOfTaskTemplateId(template.ID);
            X = new ObservableCollection<X>();
            foreach (models.Parameter par in parameters.Where(par => par.IsOutput == 0))
            {
                X.Add(new X { ParameterDescription = par.Name });
            }
            Y = new ObservableCollection<string>(new string[parameters.Count(par => par.IsOutput != 0)]);
        }

        public ICommand DeleteSolvingInstance { get { return deleteHandler; } }
    }

    public class SolveViewModel : ViewmodelBase
    {
        private ActionHandler addHandler;
        private ActionHandler saveHandler;
        private ActionHandler solveHandler;
        private LearningInfo selectedLearning;

        public string SolverName { get; }
        public string TaskName { get; }
        public LearningInfo[] LearningList { get; }
        public LearningInfo SelectedLearning
        {
            get { return selectedLearning; }
            set
            {
                selectedLearning = value;
                addHandler.RaiseCanExecuteChanged();
                SolvingList.Clear();
            } 
        }
        public ObservableCollection<SolvingInstance> SolvingList { get; }
        public ICommand AddSolvingInstance { get { return addHandler; } }
        public ICommand SolveCommand { get { return solveHandler; } }
        public ICommand SaveCommand { get { return saveHandler; } }
        public int SelectionID { get; set; }
        public int ParameterID { get; set; }

        public SolveViewModel(models.Task task, models.TaskSolver solver)
        {
            SolvingList = new ObservableCollection<SolvingInstance>();
            SolverName = solver.Name;
            TaskName = task.Name;
            List<LearningInfo> learningList = new List<LearningInfo>();
            List <models.LearnedSolver> learnedSolvers = models.LearnedSolver.learnedSolversOfTaskSolverID(solver.ID);
            foreach (models.LearnedSolver learnedSolver in learnedSolvers)
            {
                models.Selection selection = (models.Selection)models.Selection.getById(learnedSolver.SelectionID, typeof(models.Selection));
                List<models.LearningQuality> qualities = models.LearningQuality.qualitiesOfSolverId(learnedSolver.ID);
                models.LearningScenario scenario = (models.LearningScenario)models.LearningScenario.getById(learnedSolver.LearningScenarioID, typeof(models.LearningScenario));
                models.TaskTemplate template = (models.TaskTemplate)models.TaskTemplate.getById(selection.TaskTemplateID, typeof(models.TaskTemplate));
                models.LearningQuality quality = (qualities != null && qualities.Count > 0) ? qualities[0] : null;
                List<models.Parameter> parameters = models.Parameter.parametersOfTaskTemplateId(template.ID);
                int outputParam = 0;
                foreach (models.Parameter param in parameters) {
                    if (param.IsOutput == 1) {
                        outputParam = param.ID;
                        break;
                    }
                }
                learningList.Add(new LearningInfo
                {
                    SelectionID = selection.ID,
                    ParameterID = outputParam,
                    SelectionName = selection.Name, 
                    LearningScenarioName = scenario.Name,
                    PreprocessingName = template.Name,
                    TestMistake = (quality != null) ? quality.MistakeTest : 0,
                    TrainMistake = (quality != null) ? quality.MistakeTrain : 0,
                    ClosingError = Convert.ToSingle((quality != null) ? quality.ClosingError : 0),
                    TaskTemplate = template,
                    LearnedSolver = learnedSolver
                });
            }
            LearningList = learningList.ToArray();
            addHandler = new ActionHandler(() => 
            {
                SolvingList.Add(new SolvingInstance(this, this.SelectedLearning.TaskTemplate));
                solveHandler.RaiseCanExecuteChanged();
                saveHandler.RaiseCanExecuteChanged();
            }, e=>SelectedLearning != null);

            solveHandler = new ActionHandler(Solve, e => SolvingList.Count > 0);
            saveHandler = new ActionHandler(() => { }, e => SolvingList.Count > 0);
        }

        public void DeleteSolvingInstance(SolvingInstance i)
        {
            SolvingList.Remove(i);
            solveHandler.RaiseCanExecuteChanged();
            saveHandler.RaiseCanExecuteChanged();
        }

        public void Solve()
        {
            SelectionID = SelectedLearning.SelectionID;
            ParameterID = SelectedLearning.ParameterID;
            ISolver isolver = this.SelectedLearning.LearnedSolver.Soul;
            List<string> curOutputValues = new List<string>();
            foreach (var item in SolvingList)
            {
                curOutputValues.Add(Convert.ToString(isolver.Solve(item.X.Select(x => float.Parse(x.Value)).ToArray())[0]));
            }
            PreprocessingManager preprocessing = new PreprocessingManager();
            List<string> outputValues = preprocessing.getAppropriateValuesAfterInversePreprocessing(curOutputValues, SelectionID, ParameterID);
            foreach (var item in SolvingList)
            {
                for (int i = 0; i < item.Y.Count; i++)
                {
                    item.Y[i] = outputValues[i];
                }                
            }
        }
    }
}
