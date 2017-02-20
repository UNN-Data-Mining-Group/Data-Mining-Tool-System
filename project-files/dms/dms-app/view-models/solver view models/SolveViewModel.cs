using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;

namespace dms.view_models
{
    public class LearningInfo
    {
        public string SelectionName { get; set; }
        public string LearningScenarioName { get; set; }
        public string PreprocessingName { get; set; }
        public float TrainMistake { get; set; }
        public float TestMistake { get; set; }
    }

    public struct X
    {
        public string Value { get; set; }
        public string ParameterDescription { get; set; }
    }

    public class SolvingInstance
    {
        private ActionHandler deleteHandler;

        public X[] X { get; set; }
        public ObservableCollection<string> Y { get; set; }

        public SolvingInstance(SolveViewModel vm)
        {
            deleteHandler = new ActionHandler(() => vm.DeleteSolvingInstance(this), e => true);
            X = new X[3]
                {
                    new X { ParameterDescription="Параметр 1" },
                    new X { ParameterDescription="Параметр 2" },
                    new X { ParameterDescription="Параметр 3" }
                };
            Y = new ObservableCollection<string>(new string[2]);
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

        public SolveViewModel(string taskName, string solverName)
        {
            SolvingList = new ObservableCollection<SolvingInstance>();
            SolverName = solverName;
            TaskName = taskName;
            LearningList = new LearningInfo[] 
            {
                new LearningInfo
                {
                    SelectionName ="Выборка 1",
                    LearningScenarioName = "Сценарий 1",
                    PreprocessingName ="без преобразования",
                    TestMistake = 20.5f,
                    TrainMistake = 12.4f
                },
                new LearningInfo
                {
                    SelectionName ="Выборка 1",
                    LearningScenarioName = "Сценарий 2",
                    PreprocessingName ="Преобразование 1",
                    TestMistake = 12.2f,
                    TrainMistake = 8.9f
                },
            };
            addHandler = new ActionHandler(() => 
            {
                SolvingList.Add(new SolvingInstance(this));
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
            Random r = new Random();
            foreach (var item in SolvingList)
            {
                for (int i = 0; i < item.Y.Count; i++)
                {
                    item.Y[i] = r.Nextfloat().ToString();
                }
            }
        }

        public string[] Solutions { get { return new string[] { "Решение 1", "Решение 2" }; } }
        public string SelectedSolution { get; set; }
    }
}
