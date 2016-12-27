using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;

namespace dms.view_models
{
    public class LearningStatistic
    {
        public string LearningScenarioName { get; set; }
        public float MisstakeTest { get { return 10.5f; } }
        public float MisstakeTrain { get { return 8.75f; } }
    }

    public class PreprocessingLearnViewModel
    {
        public PreprocessingLearnViewModel()
        {
            LearningStatistics = new LearningStatistic[] 
            {
                new LearningStatistic { LearningScenarioName = "Сценарий 1" },
                new LearningStatistic { LearningScenarioName = "Сценарий 2" }
            };
        }
        public string Name { get; set; }
        public LearningStatistic[] LearningStatistics { get; private set; }
    }

    public class LearnedSolverViewModel
    {
        public LearnedSolverViewModel()
        {
            PreprocessingList = new PreprocessingLearnViewModel[]
            {
                new PreprocessingLearnViewModel{Name = "Предобработка 1"},
                new PreprocessingLearnViewModel{Name = "Предобработка 2"}
            };
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
                        SelectedScenario = "Сценарий 1", SelectedPreprocessing = "Предобработка 1"
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
            deleteHandler = new ActionHandler(()=>parent.DeleteLearningDetailsItem(this), e=>true);
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

        public SelectionLearnStatisticViewModel(string taskName, string selectionName)
        {
            LearnedSolvers = new LearnedSolverViewModel[]
            {
                new LearnedSolverViewModel {Name = "Персептрон 1"},
                new LearnedSolverViewModel {Name = "Персептрон 2"}
            };

            SolversToLearn = new ObservableCollection<SolverLearnRowViewModel>
            {
                new SolverLearnRowViewModel(this) {SelectedName = "Персептрон 1"}
            };
            TaskName = taskName;
            SelectionName = selectionName;
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

        public void DeleteSolverToLearn(SolverLearnRowViewModel vm)
        {
            SolversToLearn.Remove(vm);
        }
    }
}
