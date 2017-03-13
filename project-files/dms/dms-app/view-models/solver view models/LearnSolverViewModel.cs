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
    public class LearningModel : ViewmodelBase
    {
        private ActionHandler deleteHandler;
        private int number;

        public LearningModel(LearnSolverViewModel main)
        {
            deleteHandler = new ActionHandler(() => main.Delete(this), e => true);
            SelectedSelection = Selections[0];
            SelectedScenario = LearningScenarios[0];
            SelectedPreprocessing = Preprocessings[0];
            Number = -1;
        }

        public int Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        public string[] Selections { get { return new string[] { "Выборка 1", "Выборка 2" }; } }
        public string[] LearningScenarios { get { return new string[] { "Сценарий 1", "Сценарий 2" }; } }
        public string[] Preprocessings { get { return new string[] { "Преобразование 1", "Преобразование 2" }; } }
        public string SelectedSelection { get; set; }
        public string SelectedScenario { get; set; }
        public string SelectedPreprocessing { get; set; }
        public ICommand DeleteCommand { get { return deleteHandler; } }
    }

    public class LearnSolverViewModel : ViewmodelBase
    {
        private ActionHandler addHandler;
        private ActionHandler learnHandler;

        public LearnSolverViewModel(models.Task task, models.TaskSolver solver)
        {
            TaskName = task.Name;
            SolverName = solver.Name;
            LearningList = new ObservableCollection<LearningModel>();
            addHandler = new ActionHandler(() => Add(new LearningModel(this)), e => true);
            learnHandler = new ActionHandler(() => { }, e => true);
        }

        public string TaskName { get; }
        public string SolverName { get; }
        public ObservableCollection<LearningModel> LearningList { get; }
        public ICommand AddCommand { get { return addHandler; } }
        public ICommand LearnCommand { get { return learnHandler; } }

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
    }
}
