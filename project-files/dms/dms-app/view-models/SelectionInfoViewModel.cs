using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class SelectionInfoViewModel : ViewmodelBase
    {
        private string selectedPreprocessing;

        private string[][] originalData;
        private string[] originalColumns;

        private string[][] preprocData;
        private string[] preprocColumns;

        public SelectionInfoViewModel(string taskName, string selectionName)
        {
            originalData = new string[][]
           {
                new string[] {"1", "1.0", "2", "cat"},
                new string[] {"2", "3.14", "-5", "dog"},
                new string[] {"3", "0.18", "3", "parrot"}
           };
            originalColumns = new string[] { "Номер строки", "Матрица", "Ложки не существует", "Класс" };

            preprocData = new string[][]
            {
                new string[] { "1", "0.3", "0.8", "1", "0", "0"},
                new string[] { "2", "1", "0", "0", "1", "0"},
                new string[] { "3", "0", "1", "0", "0", "1" }
            };
            preprocColumns = new string[] { "Номер строки", "Матрица", "Ложки не существует", "Кошка?", "Собака?", "Попугай?" };

            TaskName = taskName;
            SelectionName = selectionName;
            CountRows = 3;
            PreprocessingList = new string[] { "Без преобразования", "Преобразование 1" };
            SelectedPreprocessing = "Без преобразования";
        }

        public string TaskName { get; }
        public string SelectionName { get; }
        public int CountRows { get; }
        public string[] PreprocessingList { get; }
        public string SelectedPreprocessing
        {
            get { return selectedPreprocessing; }
            set
            {
                selectedPreprocessing = value;
                if (selectedPreprocessing.Equals(PreprocessingList[0]))
                {
                    Data = originalData;
                    DataColumns = originalColumns;
                }
                else
                {
                    Data = preprocData;
                    DataColumns = preprocColumns;
                }
                NotifyPropertyChanged("Data");
                NotifyPropertyChanged("DataColumns");
            }
        }
        public string[][] Data { get; private set; }
        public string[] DataColumns { get; private set; }
    }
}
