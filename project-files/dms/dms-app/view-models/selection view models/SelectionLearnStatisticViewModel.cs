using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;
using dms.models;

namespace dms.view_models
{
    public class LearningStatistic
    {
        public LearningStatistic(LearnedSolver learnedSolver)
        {
            LearningQuality learningQuality = (LearningQuality)LearningQuality.where(new Query("LearningQuality").addTypeQuery(TypeQuery.select)
                .addCondition("LearnedSolverID", "=", learnedSolver.ID.ToString()), typeof(LearningQuality))[0];
            MistakeTest = learningQuality.MistakeTest;
            MistakeTrain = learningQuality.MistakeTrain;
            ClosingError = Convert.ToSingle(learningQuality.ClosingError);

        }
        public string LearningScenarioName { get; set; }
        public float MistakeTest { get; set; }
        public float ClosingError { get; set; }
        public float MistakeTrain { get; set; }
    }

    public class PreprocessingLearnViewModel
    {
        public PreprocessingLearnViewModel(List<LearnedSolver> learnedSolvers)
        {
            List<LearningStatistic> learningStatistic = new List<LearningStatistic>();
            foreach (LearnedSolver learnedSolver in learnedSolvers)
            {
                LearningScenario learningScenario = (LearningScenario)LearningScenario.getById(learnedSolver.LearningScenarioID, typeof(LearningScenario));
                learningStatistic.Add(new LearningStatistic(learnedSolver) { LearningScenarioName = learningScenario.Name });
            }
            LearningStatistics = learningStatistic.ToArray();
        }
        public string Name { get; set; }
        public LearningStatistic[] LearningStatistics { get; private set; }
    }

    public class LearnedSolverViewModel
    {
        public LearnedSolverViewModel(List<LearnedSolver> learnedSolvers)
        {
            foreach (LearnedSolver learnedSolver in learnedSolvers)
            {
                Selection selection = (Selection)Selection.getById(learnedSolver.SelectionID, typeof(Selection));
                TaskTemplate taskTemplate = (TaskTemplate)TaskTemplate.getById(selection.TaskTemplateID, typeof(TaskTemplate));
                PreprocessingList = new PreprocessingLearnViewModel[]
                {
                new PreprocessingLearnViewModel(learnedSolvers) {Name = taskTemplate.Name}
                };
            }
        }
        public string Name { get; set; }
        public PreprocessingLearnViewModel[] PreprocessingList { get; private set; }
    }

    public class SolverLearnRowViewModel
    {
        private ActionHandler addHandler;
        private ActionHandler deleteHandler;

        public SolverLearnRowViewModel(SelectionLearnStatisticViewModel parent)
        {
            LearningDetails = new ObservableCollection<LearningDetailsViewModel>();
            LearningDetails.Add(new LearningDetailsViewModel(this) { SelectedScenario = "Сценарий 1", SelectedPreprocessing = "Предобработка 1" });
            addHandler = new ActionHandler(() =>
            {
                LearningDetails.Add(
                    new LearningDetailsViewModel(this)
                    {
                        SelectedScenario = "Сценарий 1",
                        SelectedPreprocessing = "Предобработка 1"
                    });
            }, e => true);
            deleteHandler = new ActionHandler(() => parent.DeleteSolverToLearn(this), e => true);
        }
        public string SelectedName { get; set; }
        public string[] Names { get { return new string[] { "Персептрон 1", "Персептрон 2" }; } }
        public ObservableCollection<LearningDetailsViewModel> LearningDetails { get; private set; }
        public ICommand AddCommand { get { return addHandler; } }
        public ICommand DeleteCommand { get { return deleteHandler; } }
        public void DeleteLearningDetailsItem(LearningDetailsViewModel vm)
        {
            LearningDetails.Remove(vm);
        }
    }

    public class LearningDetailsViewModel
    {
        private ActionHandler deleteHandler;
        public LearningDetailsViewModel(SolverLearnRowViewModel parent)
        {
            deleteHandler = new ActionHandler(() => parent.DeleteLearningDetailsItem(this), e => true);
        }
        public string SelectedScenario { get; set; }
        public string SelectedPreprocessing { get; set; }

        public string[] LearningScenarios { get { return new string[] { "Сценарий 1", "Сценарий 2" }; } }
        public string[] Preprocessings { get { return new string[] { "Предобработка 1", "Предобработка 2" }; } }
        public ICommand DeleteCommand { get { return deleteHandler; } }
    }

    public class SelectionLearnStatisticViewModel : ViewmodelBase
    {
        private ActionHandler addHandler;
        private ActionHandler learnCommand;

        public SelectionLearnStatisticViewModel(Selection selection, string taskName)
        {
            List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("Name", "=", selection.Name), typeof(Selection));
            List<Entity> learnedSolvers = new List<Entity>();
            foreach (Selection sel in selections)
                learnedSolvers.AddRange(LearnedSolver.where(new Query("LearnedSolver").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", sel.ID.ToString()), typeof(LearnedSolver)));
            List<LearnedSolverViewModel> listLearnedSolverViewModel = new List<LearnedSolverViewModel>();
            ISet<string> nameLearnedSolver = new HashSet<string>();
            Dictionary<string, List<LearnedSolver>> dictionarytaskSolverToLearnedSolver = new Dictionary<string, List<LearnedSolver>>();
            foreach (LearnedSolver learnedSolver in learnedSolvers)
            {
                TaskSolver taskSolver = (TaskSolver)TaskSolver.getById(learnedSolver.TaskSolverID, typeof(TaskSolver));
                List<LearnedSolver> learnedSolverForCurSolver;
                if (dictionarytaskSolverToLearnedSolver.TryGetValue(taskSolver.Name, out learnedSolverForCurSolver))
                {
                    learnedSolverForCurSolver.Add(learnedSolver);
                    dictionarytaskSolverToLearnedSolver[taskSolver.Name] = learnedSolverForCurSolver;
                }
                else
                {
                    learnedSolverForCurSolver = new List<LearnedSolver>() { learnedSolver };
                    dictionarytaskSolverToLearnedSolver.Add(taskSolver.Name, learnedSolverForCurSolver);
                }
            }
            foreach (KeyValuePair<string, List<LearnedSolver>> pair in dictionarytaskSolverToLearnedSolver)
            {
                listLearnedSolverViewModel.Add(new LearnedSolverViewModel(pair.Value) { Name = pair.Key });
            }
            LearnedSolvers = listLearnedSolverViewModel.ToArray();

            SolversToLearn = new ObservableCollection<SolverLearnRowViewModel>
            {
                new SolverLearnRowViewModel(this) {SelectedName = "Персептрон 1"}
            };
            CurSelection = selection;
            TaskName = taskName;
            SelectionName = selection.Name;
            addHandler = new ActionHandler(
                () =>
                {
                    SolversToLearn.Add(new SolverLearnRowViewModel(this)
                    {
                        SelectedName = "Персептрон " + (SolversToLearn.Count + 1)
                    });
                }, e => true);
        }
        public string TaskName { get; }
        public string SelectionName { get; }
        public LearnedSolverViewModel[] LearnedSolvers { get; private set; }
        public ObservableCollection<SolverLearnRowViewModel> SolversToLearn { get; private set; }
        public ICommand AddCommand { get { return addHandler; } }
        public ICommand LearnCommand { get { return learnCommand; } }

        public Selection CurSelection { get; private set; }

        public void DeleteSolverToLearn(SolverLearnRowViewModel vm)
        {
            SolversToLearn.Remove(vm);
        }
    }
}