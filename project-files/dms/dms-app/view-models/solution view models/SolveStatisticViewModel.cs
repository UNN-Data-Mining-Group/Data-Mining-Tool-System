using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dms.view_models
{
    public class SolvingInstanceViewModel
    {
        public string SolverName { get; set; }
        public string SelectionName { get; set; }
        public string ScenarioName { get; set; }
        public string PreprocessingName { get; set; }
        public string[] Y { get; set; }
    }
    public class SolvingRowViewModel
    {
        public string[] X { get; set; }
        public SolvingInstanceViewModel[] Solutions { get; set; }
    }
    public class SolveStatisticViewModel : ViewmodelBase
    {
        private SolvingRowViewModel selectedRow;

        public string TaskName { get; }
        public string Name { get; }
        public string[] XNames { get; set; }
        public string[] YNames { get; set; }
        public SolvingRowViewModel[] Data { get; set; }
        public SolvingRowViewModel SelectedRow { get { return selectedRow; } set { selectedRow = value; NotifyPropertyChanged(); } }

        public SolveStatisticViewModel(string taskName, string name)
        {
            TaskName = taskName;
            Name = name;
            XNames = new string[] { "Параметр 1", "Параметр 2" };
            YNames = new string[] { "Параметр 3" };
            Data = new SolvingRowViewModel[] 
            {
                new SolvingRowViewModel
                {
                    X = new string[] {"1", "2"},
                    Solutions = new SolvingInstanceViewModel[]
                    {
                        new SolvingInstanceViewModel
                        {
                            SolverName = "Решатель 1",
                            SelectionName = "Выборка 1",
                            ScenarioName = "Сценарий 1",
                            PreprocessingName = "Предобработка 1",
                            Y = new string[] {"3"}
                        },
                        new SolvingInstanceViewModel
                        {
                            SolverName = "Решатель 2",
                            SelectionName = "Выборка 2",
                            ScenarioName = "Сценарий 2",
                            PreprocessingName = "Предобработка 2",
                            Y = new string[] {"2"}
                        }
                    }
                },
                new SolvingRowViewModel
                {
                    X = new string[] {"5", "6"},
                    Solutions = new SolvingInstanceViewModel[]
                    {
                        new SolvingInstanceViewModel
                        {
                            SolverName = "Решатель 1",
                            SelectionName = "Выборка 1",
                            ScenarioName = "Сценарий 1",
                            PreprocessingName = "Предобработка 1",
                            Y = new string[] {"1"}
                        }
                    }
                },
                new SolvingRowViewModel
                {
                    X = new string[] {"2", "0"}
                }
            };
        }
    }
}
