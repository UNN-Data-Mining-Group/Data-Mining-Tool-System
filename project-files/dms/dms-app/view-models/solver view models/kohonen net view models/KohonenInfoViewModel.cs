using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.solvers.neural_nets.kohonen;
using System.Collections.ObjectModel;
using System.Windows.Input;
using dms.tools;
using HexagonalGrid;
using dms.models;

namespace dms.view_models
{
    public class TileGraphViewModel : ViewmodelBase
    {
        private int tileSize;
        private List<Tuple<int2d, double>> weights;
        private string selectedParameter;

        public TileGraphViewModel(int w, int h, int s)
        {
            Width = w;
            Height = h;
            SizeOfTile = s;
            DeleteGraph = new ActionHandler(() => DeleteCallback(this), e => true);
        }

        public int Width { get; }
        public int Height { get; }
        public int SizeOfTile
        {
            get { return tileSize; }
            set { tileSize = value; NotifyPropertyChanged(); }
        }
        public Action<TileGraphViewModel> DeleteCallback { get; set; }
        public ICommand DeleteGraph { get; }
        public string[] ParameterNames { get; set; }
        public string SelectedParameter
        {
            get { return selectedParameter; }
            set
            {
                selectedParameter = value;
                int index = 0;
                for(int i = 0; i < ParameterNames.Length; i++)
                {
                    if (selectedParameter == ParameterNames[i])
                    {
                        index = i;
                        break;
                    }
                }

                Weights = Data[index];
            }
        }
        public List<Tuple<int2d, double>> Weights
        {
            set
            {
                weights = value;
                NotifyPropertyChanged();
            }
            get
            {
                return weights;
            }
        }
        public List<Tuple<int2d, double>>[] Data { get; set; }
    }
    public class KohonenInfoViewModel : ViewmodelBase
    {
        private models.Task task;
        private TaskSolver taskSolver;
        private int tileSize;
        private string selectedScenario;
        private string selectedSelection;
        private string selectedTemplate;
        private ActionHandler addHandler;
        private string[] paramNames;
        private List<Tuple<int2d, double>>[] data;

        public KohonenInfoViewModel(models.Task task, models.TaskSolver solver)
        {
            this.task = task;
            taskSolver = solver;

            KohonenNNTopology t = solver.Description as KohonenNNTopology;
            Inputs = Convert.ToInt32(t.GetInputsCount());
            Outputs = Convert.ToInt32(t.GetOutputsCount());
            Width = t.GetLayerWidth();
            Height = t.GetLayerHeight();
            Metric = t.GetMetric();
            ClassInitializer = t.GetClassInitializer();
            ClassEps = t.GetClassEps();
            Name = solver.Name;
            TaskName = task.Name;
            TileGraphs = new ObservableCollection<TileGraphViewModel>();
            SizeOfTile = 10;
            paramNames = new string[] { };
            data = new List<Tuple<int2d, double>>[] { };

            addHandler = new ActionHandler(
                () => 
                {
                    TileGraphs.Add(new TileGraphViewModel(Width, Height, SizeOfTile)
                    {
                        DeleteCallback = DeleteGraph,
                        ParameterNames = paramNames,
                        Data = data
                    });
                },
                (e) => true);
        }

        public int Inputs { get; }
        public int Outputs { get; }
        public int Width { get; }
        public int Height { get; }
        public float ClassEps { get; }
        public string Metric { get; }
        public string ClassInitializer { get; }
        public string Name { get; }
        public string TaskName { get; }
        public int SizeOfTile
        {
            get { return tileSize; }
            set
            {
                tileSize = value;
                foreach (var item in TileGraphs)
                    item.SizeOfTile = tileSize;
            }
        }
        public ObservableCollection<TileGraphViewModel> TileGraphs { get; }
        public string[] LearningScenarios
        {
            get
            {
                List<Entity> learnedSolvers = LearnedSolver.where(new Query("LearnedSolver")
                   .addTypeQuery(TypeQuery.select)
                   .addCondition("TaskSolverID", "=", taskSolver.ID.ToString()),
                   typeof(LearnedSolver));
                List<Entity> listLearningScenarios = new List<Entity>();
                foreach (LearnedSolver s in learnedSolvers)
                    listLearningScenarios.Add(LearningScenario.getById(s.LearningScenarioID, typeof(LearningScenario)));

                string[] nameLearningScenarios = new string[listLearningScenarios.Count];
                int i = 0;
                foreach (LearningScenario ls in listLearningScenarios)
                { 
                    nameLearningScenarios[i] = ls.Name;
                    i++;
                }
                return nameLearningScenarios;
            }
        }
        public string[] Selections
        {
            get
            {
                List<Entity> learnedSolvers = LearnedSolver.where(new Query("LearnedSolver")
                    .addTypeQuery(TypeQuery.select)
                    .addCondition("TaskSolverID", "=", taskSolver.ID.ToString()),
                    typeof(LearnedSolver));
                List<Entity> selections = new List<Entity>();
                foreach (LearnedSolver s in learnedSolvers)
                    selections.Add(Selection.getById(s.SelectionID, typeof(Selection)));
                HashSet<string> setSelections = new HashSet<string>();

                foreach (Selection ls in selections)
                    setSelections.Add(ls.Name);
                return setSelections.ToArray();
            }
        }
        public string[] Templates
        {
            get
            {
                List<Entity> learnedSolvers = LearnedSolver.where(new Query("LearnedSolver")
                    .addTypeQuery(TypeQuery.select)
                    .addCondition("TaskSolverID", "=", taskSolver.ID.ToString()),
                    typeof(LearnedSolver));
                List<Entity> selectionsByName = Selection.where(new Query("Selection")
                    .addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", SelectedSelection), typeof(Selection));

                List<Entity> selectionsBySolver = new List<Entity>();
                foreach (LearnedSolver s in learnedSolvers)
                    selectionsBySolver.Add(Selection.getById(s.SelectionID, typeof(Selection)));

                List<Entity> selections = new List<Entity>();
                foreach(Selection item in selectionsByName)
                {
                    bool isFound = false;
                    foreach(Selection item2 in selectionsBySolver)
                    {
                        if (item.ID == item2.ID)
                        {
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound == true)
                        selections.Add(item);
                }

                List<Entity> listTaskTemplate = Entity.all(typeof(TaskTemplate));
                string[] nameTaskTemplate = new string[selections.Count];
                int i = 0;
                foreach (Selection selection in selections)
                {
                    TaskTemplate taskTemplate = (TaskTemplate)TaskTemplate.getById(selection.TaskTemplateID, typeof(TaskTemplate));
                    nameTaskTemplate[i] = taskTemplate.Name;
                    i++;
                }
                return nameTaskTemplate;
            }
        }
        public string SelectedScenario
        {
            get { return selectedScenario; }
            set
            {
                if (selectedScenario != value)
                    TileGraphs.Clear();
                selectedScenario = value;
                NotifyPropertyChanged("CanAddGraph");
            }
        }
        public string SelectedSelection
        {
            get { return selectedSelection; }
            set
            {
                if (selectedSelection != value)
                    TileGraphs.Clear();
                selectedSelection = value;
                SelectedTemplate = Templates[0];
                NotifyPropertyChanged("Templates");
                NotifyPropertyChanged("CanAddGraph");
            }
        }
        public string SelectedTemplate
        {
            get{ return selectedTemplate; }
            set
            {
                if (selectedTemplate != value)
                    TileGraphs.Clear();
                selectedTemplate = value;

                List<Entity> templates = TaskTemplate.where(new Query("TaskTemplate")
                    .addTypeQuery(TypeQuery.select).addCondition("TaskID", "=", task.ID.ToString())
                    .addCondition("Name", "=", SelectedTemplate), typeof(TaskTemplate));
                if (templates.Count > 1)
                    throw new Exception("Achtung in database");
                
                TaskTemplate t = templates[0] as TaskTemplate;
                List<Entity> pars = models.Parameter.where(new Query("Parameter")
                    .addTypeQuery(TypeQuery.select).addCondition("TaskTemplateID", "=", t.ID.ToString()),
                    typeof(models.Parameter));

                int paramsCount = pars.Count + 2;
                paramNames = new string[paramsCount];
                foreach(models.Parameter parameter in pars)
                {
                    paramNames[parameter.Index] = parameter.Name;
                }
                paramNames[pars.Count] = "Количество ошибок";
                paramNames[pars.Count + 1] = "Количество верных ответов";

                NotifyPropertyChanged();
                NotifyPropertyChanged("CanAddGraph");
            }
        }
        public bool CanAddGraph
        {
            get
            {
                bool res = SelectedSelection != null &&
                    SelectedScenario != null &&
                    SelectedTemplate != null;
                if (res == true)
                {
                    Entity taskTemplate = TaskTemplate.where(new Query("TaskTemplate")
                        .addTypeQuery(TypeQuery.select)
                        .addCondition("Name", "=", SelectedTemplate),
                        typeof(TaskTemplate))[0];
                    Entity selection = Selection.where(new Query("Selection")
                        .addTypeQuery(TypeQuery.select)
                        .addCondition("TaskTemplateID", "=", taskTemplate.ID.ToString()),
                        typeof(Selection))[0];
                    Entity scenario = LearningScenario.where(new Query("LearningScenario")
                        .addTypeQuery(TypeQuery.select)
                        .addCondition("Name", "=", SelectedScenario),
                        typeof(LearningScenario))[0];

                    LearnedSolver solver = LearnedSolver.where(new Query("LearnedSolver")
                        .addTypeQuery(TypeQuery.select)
                        .addCondition("SelectionID", "=", selection.ID.ToString())
                        .addCondition("LearningScenarioID", "=", scenario.ID.ToString())
                        .addCondition("TaskSolverID", "=", taskSolver.ID.ToString()), 
                        typeof(LearnedSolver))[0] as LearnedSolver;

                    KohonenManaged km = solver.Soul as KohonenManaged;
                    data = km.GetVisualData();
                }
                return res;
            }
        }
        public ICommand AddGraph { get { return addHandler; } }

        public void DeleteGraph(TileGraphViewModel graph)
        {
            TileGraphs.Remove(graph);
        }
    }
}
