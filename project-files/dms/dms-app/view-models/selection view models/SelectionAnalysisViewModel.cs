using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.tools;
using System.Windows.Input;
using dms.services.preprocessing;

namespace dms.view_models
{
    public class SelectionAnalysisViewModel : ViewmodelBase
    {
        private ActionHandler left, right;
        public ICommand LeftPage { get { return left; } }
        public ICommand RightPage { get { return right; } }
        public string Page { get { return page; } private set { page = value; NotifyPropertyChanged(); } }
        private string page;
        private int curPage;
        private int maxPage;
        private const int elementsInPage = 10;//30;

        public string TaskName { get; }
        public int TaskId { get; }
        public string SelectionName { get; }
        private int SelectionId { get; set; }
        public int CountRows { get; }

        public string[] AnalysisList { get { return new string[] { "Empty", "Корреляционный анализ (метод квадратов Пирсона)", "Корреляционный анализ (ранговый метод Спирмена)" }; } }
        private string analysis;
        public string Analysis
        {
            get { return analysis; }
            set
            {
                analysis = value;
                updateTable();
                DataColumns = originalColumns;
                NotifyPropertyChanged("Data");
                NotifyPropertyChanged("DataColumns");
            }
        }
        private string[][] originalData;
        private string[] originalColumns;
        public string[][] Data { get; private set; }
        public string[] DataColumns { get; private set; }

        public SelectionAnalysisViewModel(int taskId, int selectionId)
        {
            Data = new string[elementsInPage][];
            left = new ActionHandler(
                () =>
                {
                    curPage--;
                    Page = curPage + "/" + maxPage;
                    updatePage();
                    left.RaiseCanExecuteChanged();
                    right.RaiseCanExecuteChanged();
                }, (e) => { return curPage > 1; });
            right = new ActionHandler(
                () =>
                {
                    curPage++;
                    Page = curPage + "/" + maxPage;
                    updatePage();
                    left.RaiseCanExecuteChanged();
                    right.RaiseCanExecuteChanged();
                }, (e) => { return curPage < maxPage; });

            SelectionId = selectionId;
            Selection selection = ((Selection)services.DatabaseManager.SharedManager.entityById(selectionId, typeof(Selection)));
            SelectionName = selection.Name;
            CountRows = selection.RowCount;
            TaskId = taskId;
            TaskName = ((models.Task)services.DatabaseManager.SharedManager.entityById(taskId, typeof(models.Task))).Name;

            Selection selEntity = (Selection)Selection.getById(SelectionId, typeof(Selection));
            models.Parameter[] parameters = models.Parameter.parametersOfTaskTemplateId(selEntity.TaskTemplateID).ToArray();

            curPage = 1;
            maxPage = 2;// selection.RowCount / elementsInPage;
            Page = curPage + "/" + maxPage;

            Analysis = AnalysisList.First();
        }        

        private void updatePage()
        {
           
                
            if (originalData != null)
            {
                Data = new string[elementsInPage][];
                for (int i = 0; i < elementsInPage; i++)
                {
                    Data[i] = originalData[i + (curPage - 1) * elementsInPage];
                }
            }
            else
            {
                Data = new string[1][];
                Data[0] = new string[4] { "", "", "", "" };
            }
                NotifyPropertyChanged("Data");
            
            
        }

        private void updateTable()
        {

            //рисуем заголовки
            //Strong, medium, small, very small
            originalColumns = new string[4];
            originalColumns[0] = "Strong correltion";
            originalColumns[1] = "Medium correltion";
            originalColumns[2] = "Small correltion";
            originalColumns[3] = "Very small correltion";
            //рисуем содержимое
            Selection selection = (Selection)Selection.getById(SelectionId, typeof(Selection));
//            models.Parameter[] parameters = models.Parameter.parametersOfTaskTemplateId(templateId).ToArray();
            Analysis.TypeAnalysis analysisType = services.preprocessing.Analysis.TypeAnalysis.Empty;
            int rowCount = 0;
            if (AnalysisList.ElementAt(1).Equals(Analysis))
            {
                analysisType = services.preprocessing.Analysis.TypeAnalysis.PearsonMethod;
                Analysis analysis = new Analysis();
                originalData = analysis.executeCorrelationAnalysis(analysisType, SelectionId, selection.TaskTemplateID);
                rowCount = analysis.rowSize;
            }
            else if (AnalysisList.ElementAt(2).Equals(Analysis))
            {
                Analysis analysis = new Analysis();
                analysisType = services.preprocessing.Analysis.TypeAnalysis.SpearmanMethod;
                originalData = new Analysis().executeCorrelationAnalysis(analysisType, SelectionId, selection.TaskTemplateID);
                rowCount = analysis.rowSize;
            }

            curPage = 1;
            maxPage = rowCount / elementsInPage;
            Page = curPage + "/" + maxPage;
            updatePage();
        }
    }
}
