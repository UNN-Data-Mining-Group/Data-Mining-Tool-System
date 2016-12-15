using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using dms.tools;

namespace dms.view_models
{
    public class SelectionCreationViewModel : ViewmodelBase
    {
        private ActionHandler deleteSelectionHandler;
        private ActionHandler browseFileCommandHandler;
        private string selectionName;
        private bool hasHeader;
        private int countRows;
        private string filePath;
        private string delimiter;

        public event EventHandler OnDelete;
        public event EventHandler<EventArgs<Tuple<string, bool>>> OnFileChanged;

        public ICommand DeleteSelectionCommand { get { return deleteSelectionHandler; } }
        public ICommand BrowseFileCommand { get { return browseFileCommandHandler; } }

        public string ParentTask { get; set; }
        public string SelectionName { get { return selectionName; } set { selectionName = value; NotifyPropertyChanged(); } }
        public bool HasHeader { get { return hasHeader; } set { hasHeader = value; NotifyPropertyChanged(); } }
        public int CountRows { get { return countRows; } set { countRows = value; NotifyPropertyChanged(); } }
        public string FilePath { get { return filePath; } set { filePath = value; NotifyPropertyChanged(); } }
        public List<string> DelimiterList { get { return new List<string> { ".", "," }; } }
        public string Delimiter { get { return delimiter; } set { delimiter = value; NotifyPropertyChanged(); } }

        public SelectionCreationViewModel(string taskName)
        {
            ParentTask = taskName;
            Random r = new Random();
            SelectionName = "Выборка " + r.Next(1, 1000);
            HasHeader = false;
            CountRows = 0;
            FilePath = "";
            Delimiter = ",";

            deleteSelectionHandler = new ActionHandler(DeleteSelection, (o) => true);
            browseFileCommandHandler = new ActionHandler(BrowseFile, (o) => true);
        }

        public void BrowseFile()
        {
            FilePath = "/usr/file1.txt";
            CountRows = 2000;

            //Параметром передается пара значений: строка, хранящая путь до файла и флаг, говорящий о том,
            //включается файл в общий список (true) или удаляется из него (false)
            OnFileChanged?.Invoke(this, new EventArgs<Tuple<string, bool>>(new Tuple<string, bool>(FilePath, true)));
        }

        public void DeleteSelection()
        {
            OnDelete?.Invoke(this, null);

            //Параметром передается пара значений: строка, хранящая путь до файла и флаг, говорящий о том,
            //включается файл в общий список (true) или удаляется из него (false)
            OnFileChanged?.Invoke(this, new EventArgs<Tuple<string, bool>>(new Tuple<string, bool>(FilePath, false)));
        }
    }
}
