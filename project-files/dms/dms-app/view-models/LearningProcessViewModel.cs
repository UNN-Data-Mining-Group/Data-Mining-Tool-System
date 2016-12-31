using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class LearningProcessItemViewModel
    {
        public string SolverName { get; set; }
        public string ScenarioName { get; set; }
        public string TaskName { get; set; }
        public string SelectionName { get; set; }
        public string PreprocessingName { get; set; }
        public int Progress { get; set; }
        public float TrainErr { get; set; }
        public float TestErr { get; set; }
        public bool CanWriteResults { get { return Progress == 100; } }
    }

    public class LearningProcessViewModel
    {
        public ObservableCollection<LearningProcessItemViewModel> LearningProcessList
        {
            get;
        }
        public LearningProcessViewModel()
        {
            LearningProcessList = new ObservableCollection<LearningProcessItemViewModel>
            {
                new LearningProcessItemViewModel
                {
                    SolverName = "Решатель 1",
                    ScenarioName = "Сценарий 1",
                    TaskName = "Задача 1",
                    SelectionName = "Выборка 1",
                    PreprocessingName = "Предобработка 1",
                    Progress = 53,
                    TrainErr = 42,
                    TestErr = 78
                },
                new LearningProcessItemViewModel
                {
                    SolverName = "Решатель 2",
                    ScenarioName = "Сценарий 1",
                    TaskName = "Задача 1",
                    SelectionName = "Выборка 2",
                    PreprocessingName = "Предобработка 2",
                    Progress = 100,
                    TrainErr = 7,
                    TestErr = 12
                }
            };
        }
    }
}
