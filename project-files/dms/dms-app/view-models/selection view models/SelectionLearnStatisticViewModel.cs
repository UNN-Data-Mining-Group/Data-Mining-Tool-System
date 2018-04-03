using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;
using dms.models;
using dms.solvers;
using dms.solvers.neural_nets.perceptron;
using dms.solvers.neural_nets.conv_net;
using dms.solvers.neural_nets.ward_net;
using dms.solvers.decision.tree;
using dms.view_models.solver_view_models;
using System.Globalization;

namespace dms.view_models
{
    public class LearningStatistic
    {
        public LearningStatistic(LearnedSolver learnedSolver)
        {
            LearningQuality learningQuality = (LearningQuality)LearningQuality.where(new Query("LearningQuality").addTypeQuery(TypeQuery.select)
                .addCondition("LearnedSolverID", "=", learnedSolver.ID.ToString()), typeof(LearningQuality))[0];
            MistakeTest = learningQuality.MistakeTest;
            MistakeTrain = learningQuality.MistakeTrain;
            ClosingError = Convert.ToSingle(learningQuality.ClosingError);

        }
        public string LearningScenarioName { get; set; }
        public float MistakeTest { get; set; }
        public float ClosingError { get; set; }
        public float MistakeTrain { get; set; }
    }

    public class PreprocessingLearnViewModel
    {
        public PreprocessingLearnViewModel(List<LearnedSolver> learnedSolvers)
        {
            List<LearningStatistic> learningStatistic = new List<LearningStatistic>();
            foreach (LearnedSolver learnedSolver in learnedSolvers)
            {
                LearningScenario learningScenario = (LearningScenario)LearningScenario.getById(learnedSolver.LearningScenarioID, typeof(LearningScenario));
                learningStatistic.Add(new LearningStatistic(learnedSolver) { LearningScenarioName = learningScenario.Name });
            }
            LearningStatistics = learningStatistic.ToArray();
        }
        public string Name { get; set; }
        public LearningStatistic[] LearningStatistics { get; private set; }
    }

    public class LearnedSolverViewModel
    {
        public LearnedSolverViewModel(List<LearnedSolver> learnedSolvers)
        {
            foreach (LearnedSolver learnedSolver in learnedSolvers)
            {
                Selection selection = (Selection)Selection.getById(learnedSolver.SelectionID, typeof(Selection));
                TaskTemplate taskTemplate = (TaskTemplate)TaskTemplate.getById(selection.TaskTemplateID, typeof(TaskTemplate));
                PreprocessingList = new PreprocessingLearnViewModel[]
                {
                new PreprocessingLearnViewModel(learnedSolvers) {Name = taskTemplate.Name}
                };
            }
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
            CurSelection = parent.CurSelection;
            LearningDetails = new ObservableCollection<LearningDetailsViewModel>();
            LearningDetails.Add(new LearningDetailsViewModel(this) { SelectedScenario = "Сценарий 1", SelectedPreprocessing = "Предобработка 1" });
            addHandler = new ActionHandler(() =>
            {
                LearningDetails.Add(
                    new LearningDetailsViewModel(this)
                    {
                        SelectedScenario = "Сценарий 1",
                        SelectedPreprocessing = "Предобработка 1"
                    });
            }, e => true);
            deleteHandler = new ActionHandler(() => parent.DeleteSolverToLearn(this), e => true);
        }
        public string SelectedName { get; set; }
        public string[] Names
        {
            get
            {
                List<Entity> listSolvers = null;
                listSolvers = Entity.all(typeof(TaskSolver));
                List<string> nameLearningScenarios = new List<string>();
                int i = 0;
                foreach (TaskSolver ls in listSolvers)
                {
                    nameLearningScenarios.Add(ls.Name);
                }
                if (nameLearningScenarios.Count == 0)
                {
                    CanSolve = false;
                    nameLearningScenarios.Add("Нет созданных решателей");
                }
                SelectedName = nameLearningScenarios[0];
                return nameLearningScenarios.ToArray();
            }
        }
        public ObservableCollection<LearningDetailsViewModel> LearningDetails { get; private set; }
        public ICommand AddCommand { get { return addHandler; } }
        public ICommand DeleteCommand { get { return deleteHandler; } }

        public Selection CurSelection { get; private set; }
        public bool CanSolve { get; private set; }

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
            CurSelection = parent.CurSelection;
            deleteHandler = new ActionHandler(() => parent.DeleteLearningDetailsItem(this), e => true);
        }
        public string SelectedScenario { get; set; }
        public string SelectedPreprocessing { get; set; }

        public string[] LearningScenarios
        {
            get
            {
                List<Entity> listLearningScenarios = null;
                //string typeSolver = CurTaskSolver.TypeName;
                // Не убирать!!!!!!

                /* if (typeSolver.Equals("DecisionTree"))
                {
                    listLearningScenarios = LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                    .addCondition("LearningAlgorithmName", "=", "Деревья решений"), typeof(LearningScenario));
                }
                else
                {
                    listLearningScenarios = LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                    .addCondition("LearningAlgorithmName", "!=", "Деревья решений"), typeof(LearningScenario));
                }
                */
                listLearningScenarios = Entity.all(typeof(LearningScenario));
                List<string> nameLearningScenarios = new List<string>();
                int i = 0;
                foreach (LearningScenario ls in listLearningScenarios)
                {
                    nameLearningScenarios.Add(ls.Name);
                }
                if (nameLearningScenarios.Count == 0)
                {
                    CanSolve = false;
                    nameLearningScenarios.Add("Нет созданных сценариев");
                }
                SelectedScenario = nameLearningScenarios[0];
                return nameLearningScenarios.ToArray();
            }
        }
        public string[] Preprocessings
        {
            get
            {
                List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", CurSelection.Name), typeof(Selection));
                List<Entity> listTaskTemplate = Entity.all(typeof(TaskTemplate));
                List<string> nameTaskTemplate = new List<string>();
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
                        nameTaskTemplate.Add(taskTemplate.Name);
                        i++;
                    }
                }
                if (nameTaskTemplate.Count == 0)
                {
                    CanSolve = false;
                    nameTaskTemplate.Add("Нет созданных преобразований");
                }
                SelectedPreprocessing = nameTaskTemplate[0];
                return nameTaskTemplate.ToArray();
            }
        }
        public ICommand DeleteCommand { get { return deleteHandler; } }
        public bool CanSolve { get; set; }

        public Selection CurSelection { get; private set; }
    }

    public class SelectionLearnStatisticViewModel : ViewmodelBase
    {
        private ActionHandler addHandler;
        private ActionHandler learnCommand;

        public SelectionLearnStatisticViewModel(Selection selection, string taskName)
        {
            CurSelection = selection;
            List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("Name", "=", selection.Name), typeof(Selection));
            List<Entity> learnedSolvers = new List<Entity>();
            foreach (Selection sel in selections)
                learnedSolvers.AddRange(LearnedSolver.where(new Query("LearnedSolver").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", sel.ID.ToString()), typeof(LearnedSolver)));
            List<LearnedSolverViewModel> listLearnedSolverViewModel = new List<LearnedSolverViewModel>();
            ISet<string> nameLearnedSolver = new HashSet<string>();
            Dictionary<string, List<LearnedSolver>> dictionarytaskSolverToLearnedSolver = new Dictionary<string, List<LearnedSolver>>();
            foreach (LearnedSolver learnedSolver in learnedSolvers)
            {
                TaskSolver taskSolver = (TaskSolver)TaskSolver.getById(learnedSolver.TaskSolverID, typeof(TaskSolver));
                List<LearnedSolver> learnedSolverForCurSolver;
                if (dictionarytaskSolverToLearnedSolver.TryGetValue(taskSolver.Name, out learnedSolverForCurSolver))
                {
                    learnedSolverForCurSolver.Add(learnedSolver);
                    dictionarytaskSolverToLearnedSolver[taskSolver.Name] = learnedSolverForCurSolver;
                }
                else
                {
                    learnedSolverForCurSolver = new List<LearnedSolver>() { learnedSolver };
                    dictionarytaskSolverToLearnedSolver.Add(taskSolver.Name, learnedSolverForCurSolver);
                }
            }
            foreach (KeyValuePair<string, List<LearnedSolver>> pair in dictionarytaskSolverToLearnedSolver)
            {
                listLearnedSolverViewModel.Add(new LearnedSolverViewModel(pair.Value) { Name = pair.Key });
            }
            LearnedSolvers = listLearnedSolverViewModel.ToArray();

            SolversToLearn = new ObservableCollection<SolverLearnRowViewModel>
            {
                new SolverLearnRowViewModel(this) {SelectedName = "Персептрон 1"}
            };
            TaskName = taskName;
            SelectionName = selection.Name;
            addHandler = new ActionHandler(
                () =>
                {
                    SolversToLearn.Add(new SolverLearnRowViewModel(this)
                    {
                        SelectedName = "Персептрон " + (SolversToLearn.Count + 1)
                    });
                }, e => true);
            learnCommand = new ActionHandler(learnSolver, e => true);
        }
        public string TaskName { get; }
        public string SelectionName { get; }
        public LearnedSolverViewModel[] LearnedSolvers { get; private set; }
        public ObservableCollection<SolverLearnRowViewModel> SolversToLearn { get; private set; }
        public ICommand AddCommand { get { return addHandler; } }
        public ICommand LearnCommand { get { return learnCommand; } }

        public Selection CurSelection { get; private set; }

        public void DeleteSolverToLearn(SolverLearnRowViewModel vm)
        {
            SolversToLearn.Remove(vm);
        }

        private void learnSolver()
        {
            foreach (SolverLearnRowViewModel solverLearnRowViewModel in SolversToLearn)
            {
                TaskSolver Solver = (TaskSolver)TaskSolver.where(new Query("TaskSolver").addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", solverLearnRowViewModel.SelectedName), typeof(TaskSolver))[0];
                foreach (LearningDetailsViewModel learningDetailsViewModel in solverLearnRowViewModel.LearningDetails)
                {
                    TaskTemplate taskTemplate = (TaskTemplate)TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", learningDetailsViewModel.SelectedPreprocessing), typeof(TaskTemplate))[0];
                    Selection selection = (Selection)Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                    .addCondition("TaskTemplateID", "=", taskTemplate.ID.ToString())
                    .addCondition("Name", "=", CurSelection.Name), typeof(Selection))[0];
                    int countRows = selection.RowCount;
                    LearningScenario learningScenario = (LearningScenario)LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", learningDetailsViewModel.SelectedScenario), typeof(LearningScenario))[0];

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
                                outputParam = param.ID;
                                string outputValue = ((ValueParameter)value[0]).Value;
                                float outputFloat;
                                if (float.TryParse(outputValue, out outputFloat))
                                    outputData[stepRow] = outputFloat;
                            }
                            else
                                inputData[stepRow][stepParam] = float.Parse(((ValueParameter)value[0]).Value, CultureInfo.InvariantCulture.NumberFormat);
                            stepParam++;
                        }
                        stepRow++;
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
            }
        }
    }
}