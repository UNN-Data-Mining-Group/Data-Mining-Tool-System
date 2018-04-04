using dms.models;
using dms.solvers;
using dms.solvers.neural_nets.conv_net;
using dms.solvers.neural_nets.perceptron;
using dms.solvers.neural_nets.ward_net;
using dms.solvers.neural_nets.kohonen;
using dms.tools;
using dms.view_models.solver_view_models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using dms.solvers.decision.tree;

namespace dms.view_models
{
    public class LearningModel : ViewmodelBase
    {
        private ActionHandler deleteHandler;
        private int number;
        private string selectedSelection;
        private string selectedPreprocessing;
        private string[] preprocessing;
        private string[] selections;

        public LearningModel(LearnSolverViewModel main, models.Task task, TaskSolver taskSolver)
        {
            CurTaskSolver = taskSolver;
            Task = task;
            deleteHandler = new ActionHandler(() => main.Delete(this), e => true);
            Selections = Selections;
            SelectedSelection = Selections[0];
            SelectedScenario = LearningScenarios[0];
            Preprocessings = Preprocessings;
            selectedPreprocessing = Preprocessings[0];
            Number = -1;
            LearningScenarioID = -1;
            SelectionID = -1;
            CanSolve = true;
        }

        public int Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        public string[] Selections
        {
            get
            {

                List<Entity> taskTemplates = TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("TaskID", "=", Convert.ToString(Task.ID)), typeof(TaskTemplate));
                HashSet<string> setSelections = new HashSet<string>();
                List<Selection> listSelections = new List<Selection>();
                foreach (TaskTemplate tt in taskTemplates)
                {
                    listSelections.Add((Selection)Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                    .addCondition("TaskTemplateID", "=", Convert.ToString(tt.ID)), typeof(Selection))[0]);
                }
                int i = 0;
                foreach (Selection ls in listSelections)
                {
                    if (i == 0)
                        SelectionID = ls.ID;
                    setSelections.Add(ls.Name);
                    i++;
                }
                if (setSelections.Count == 0)
                {
                    CanSolve = false;
                    return new string[] { "Нет созданных выборок" };
                }
                else
                {
                    return setSelections.ToArray();
                }
            }
            set
            {
                selections = value;
                NotifyPropertyChanged("Selections");
            }
        }
        public string[] LearningScenarios
        {
            get
            {
                List<Entity> listLearningScenarios = null;
                string typeSolver = CurTaskSolver.TypeName;
                if (typeSolver.Equals("DecisionTree"))
                {
                    listLearningScenarios = LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                    .addCondition("LearningAlgorithmName", "=", "CART")
                    .addCondition("LearningAlgorithmName", "=", "C4.5", "OR"), typeof(LearningScenario));
                }
                else if (typeSolver.Equals("Perceptron"))
                {
                    listLearningScenarios = LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                    .addCondition("LearningAlgorithmName", "=", "Генетический алгоритм")
                    .addCondition("LearningAlgorithmName", "=", "Обратное распространение ошибки", "OR"), typeof(LearningScenario));
                }
                else if (typeSolver.Equals("WardNN"))
                {
                    listLearningScenarios = LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                    .addCondition("LearningAlgorithmName", "=", "Генетический алгоритм")
                    .addCondition("LearningAlgorithmName", "=", "Обратное распространение ошибки", "OR"), typeof(LearningScenario));
                }
                else if (typeSolver.Equals("ConvNN"))
                {
                    listLearningScenarios = Entity.all(typeof(LearningScenario));
                }
                else if (typeSolver.Equals("KohonenNet"))
                {
                    listLearningScenarios = LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                    .addCondition("LearningAlgorithmName", "=", "Самоорганизация Кохонена")
                    .addCondition("LearningAlgorithmName", "=", "Векторное квантование", "OR"), typeof(LearningScenario));
                }
                string[] nameLearningScenarios = new string[listLearningScenarios.Count];
                int i = 0;
                foreach (LearningScenario ls in listLearningScenarios)
                {
                    if (i == 0)
                        LearningScenarioID = ls.ID;
                    nameLearningScenarios[i] = ls.Name;
                    i++;
                }
                if (nameLearningScenarios.Length == 0)
                {
                    CanSolve = false;
                    return new string[] { "Нет созданных сценариев" };
                }
                else return nameLearningScenarios;
            }
        }

        public Dictionary<string, int> TaskTemplateIDs
        {
            get;
            set;
        }

        public string[] Preprocessings
        {
            get
            {
                List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", SelectedSelection), typeof(Selection));
                List<Entity> listTaskTemplate = Entity.all(typeof(TaskTemplate));
                List<string> nameTaskTemplate = new List<string>();
                Dictionary<string, int> mapIdNameTaskTEmplates = new Dictionary<string, int>();
                int i = 0;
                foreach (Selection selection in selections)
                {
                    List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                    .addCondition("SelectionID", "=", selection.ID.ToString()), typeof(SelectionRow));
                    List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                    .addCondition("TaskTemplateID", "=", selection.TaskTemplateID.ToString()), typeof(dms.models.Parameter));
                    TaskTemplate taskTemplate = (TaskTemplate)TaskTemplate.getById(selection.TaskTemplateID, typeof(TaskTemplate));
                    int stepRow = 0;
                    bool isNotStringOutputValue = true;
                    foreach (Entity selRow in selectionRows)
                    {
                        int selectionRowId = selRow.ID;
                        int stepParam = 0;
                        foreach (Entity param in parameters)
                        {
                            int paramId = param.ID;
                            List<Entity> value = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                                .addCondition("ParameterID", "=", paramId.ToString()).
                                addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));
                            if (((dms.models.Parameter)param).IsOutput == 1)
                            {
                                string outputValue = ((ValueParameter)value[0]).Value;
                                float outputFloat;
                                if (!float.TryParse(outputValue, out outputFloat))
                                {
                                    isNotStringOutputValue = false;
                                }
                                goto outerloop;
                            }
                            stepParam++;
                        }
                        stepRow++;
                    }

                    outerloop:;
                    if (isNotStringOutputValue)
                    {
                        if (i == 0)
                            LearningScenarioID = taskTemplate.ID;
                        nameTaskTemplate.Add(taskTemplate.Name);
                        mapIdNameTaskTEmplates.Add(taskTemplate.Name, taskTemplate.ID);
                        i++;
                    }
                }
                if (nameTaskTemplate.Count == 0)
                {
                    CanSolve = false;
                    return new string[] { "Нет созданных преобразований" };
                }
                else
                {
                    TaskTemplateIDs = mapIdNameTaskTEmplates;
                    return nameTaskTemplate.ToArray();
                }
            }
            set
            {
                preprocessing = value;
                NotifyPropertyChanged("Preprocessings");
            }
        }
        public string SelectedSelection
        {
            get
            {
                return selectedSelection;
            }
            set
            {
                selectedSelection = value;
                NotifyPropertyChanged("SelectedSelection");
                Preprocessings = Preprocessings;
                SelectedPreprocessing = Preprocessings[0];
            }
        }
        public bool CanSolve { get; set; }
        public models.Task Task { get; set; }
        public string SelectedScenario { get; set; }
        public int SelectionID { get; set; }
        public int LearningScenarioID { get; set; }
        public string SelectedPreprocessing
        {
            get
            {
                return selectedPreprocessing;
            }
            set
            {
                selectedPreprocessing = value;
                NotifyPropertyChanged("SelectedPreprocessing");
            }
        }
        public ICommand DeleteCommand { get { return deleteHandler; } }

        public TaskSolver CurTaskSolver { get; private set; }
    }

    public class LearnSolverViewModel : ViewmodelBase
    {
        private ActionHandler addHandler;
        private ActionHandler learnHandler;
        private TaskSolver solver;

        public LearnSolverViewModel(models.Task task, TaskSolver taskSolver)
        {
            TaskName = task.Name;
            SolverName = taskSolver.Name;
            solver = taskSolver;
            LearningList = new ObservableCollection<LearningModel>();
            addHandler = new ActionHandler(() => Add(new LearningModel(this, task, taskSolver)), e => true);
            learnHandler = new ActionHandler(learnSolver, e => LearningList.All(lm => lm.CanSolve));
        }

        public event EventHandler OnClose;

        public string TaskName { get; }
        public string SolverName { get; }
        public TaskSolver Solver
        {
            get
            {
                return solver;
            }
        }
        public ObservableCollection<LearningModel> LearningList { get; }
        public ICommand AddCommand { get { return addHandler; } }
        public ICommand LearnCommand { get { return learnHandler; } }
        public List<Entity> listLearningScenarion { get; private set; }

        public void Delete(LearningModel l)
        {
            LearningList.Remove(l);
            for (int i = 0; i < LearningList.Count; i++)
            {
                LearningList[i].Number = i + 1;
            }
        }
        public void Add(LearningModel l)
        {
            LearningList.Add(l);
            l.Number = LearningList.Count;
        }

        private void learnSolver()
        {
            foreach (LearningModel learningModel in LearningList)
            {
                TaskTemplate taskTemplate = (TaskTemplate)TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("ID", "=", learningModel.TaskTemplateIDs[learningModel.SelectedPreprocessing].ToString()), typeof(TaskTemplate))[0];
                Selection selection = (Selection)Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplate.ID.ToString())
                .addCondition("Name", "=", learningModel.SelectedSelection), typeof(Selection))[0];
                int countRows = selection.RowCount;
                LearningScenario learningScenario = (LearningScenario)LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                .addCondition("Name", "=", learningModel.SelectedScenario), typeof(LearningScenario))[0];

                List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selection.ID.ToString()), typeof(SelectionRow));
                List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", selection.TaskTemplateID.ToString()), typeof(dms.models.Parameter));
                int stepRow = 0;
                float[][] inputData = new float[countRows][];
                float[] outputData = new float[countRows];
                for (int i = 0; i < countRows; i++)
                {
                    inputData[i] = new float[parameters.Count - 1];
                }
                int outputParam = 0;
                bool inEnd = true;
                for(int i = 0; i < parameters.Count; i++)
                {
                    if(((models.Parameter)parameters[i]).IsOutput == 1)
                    {
                        if (i == 0)
                            inEnd = false;
                        outputParam = parameters[i].ID;
                    }
                }

                string[][] vals = Selection.valuesOfSelectionId(selection.ID);
                float[][] fvals = new float[selection.RowCount][];
                for(int i = 0; i < selection.RowCount; i++)
                {
                    fvals[i] = new float[parameters.Count];
                    int count = parameters.Count - 1;
                    int start = 0;
                    int outputIndex = count;
                    if (!inEnd)
                    {
                        count = parameters.Count;
                        start = 1;
                        outputIndex = 0;
                    }
                    for (int j = start; j < count; j++)
                    {
                        inputData[i][j - start] = float.Parse(vals[i][j].Replace(".",","));
                    }
                    outputData[i] = float.Parse(vals[i][outputIndex].Replace(".", ","));
                }
                ISolver isolver = null;
                if (Solver.Description is PerceptronTopology)
                {
                    PerceptronTopology topology = Solver.Description as PerceptronTopology;
                    isolver = new PerceptronManaged(topology);
                }
                else if (Solver.Description is ConvNNTopology)
                {
                    ConvNNTopology topology = Solver.Description as ConvNNTopology;
                    isolver = new ConvNNManaged(topology);
                }
                else if (Solver.Description is WardNNTopology)
                {
                    WardNNTopology topology = Solver.Description as WardNNTopology;
                    isolver = new WardNNManaged(topology);
                }
                else if (Solver.Description is KohonenNNTopology)
                {
                    KohonenNNTopology topology = Solver.Description as KohonenNNTopology;
                    isolver = new KohonenManaged(topology);
                }
                else if (Solver.Description is TreeDescription)
                {
                    TreeDescription topology = Solver.Description as TreeDescription;
                    isolver = new solvers.decision.tree.DecisionTree(topology);                    
                }
                else throw new EntryPointNotFoundException();
                SeparationOfDataSet s = new SeparationOfDataSet(isolver, learningScenario, inputData, outputData);
                LearnedSolver ls = new LearnedSolver()
                {
                    SelectionID = selection.ID,
                    LearningScenarioID = learningScenario.ID,
                    TaskSolverID = Solver.ID,
                    Soul = s.separationAndLearn(selection.ID, outputParam)
                };
                ls.save();

                LearningQuality lq = new LearningQuality()
                {
                    LearnedSolverID = ls.ID,
                    MistakeTrain = Convert.ToInt32(s.MistakeTrain),
                    MistakeTest = Convert.ToInt32(s.MistakeTest),
                    ClosingError = s.ClosingError
                };
                lq.save();
            }
            OnClose?.Invoke(this, null);
        }
    }
}