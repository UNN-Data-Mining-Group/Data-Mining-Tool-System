using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;
using dms.models;
using dms.solvers.neural_nets.perceptron;
using dms.solvers;
using System.Globalization;
using dms.view_models.solver_view_models;
using dms.solvers.neural_nets;
using dms.solvers.neural_nets.conv_net;
using dms.solvers.neural_nets.ward_net;

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

        public LearningModel(LearnSolverViewModel main)
        {
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
        public string[] Selections {
            get
            {
                HashSet<string> setSelections = new HashSet<string>();
                listSelections = Entity.all(typeof(Selection));
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
        public string[] LearningScenarios { get
            {
                listLearningScenarios = Entity.all(typeof(LearningScenario));
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
        public string[] Preprocessings
        {
            get
            {
                List<Entity> selections = Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                    .addCondition("Name", "=", SelectedSelection), typeof(Selection));
                listTaskTemplate = Entity.all(typeof(TaskTemplate));
                string[] nameTaskTemplate = new string[selections.Count];
                int i = 0;
                foreach (Selection selection in selections)
                {
                    TaskTemplate taskTemplate = (TaskTemplate)TaskTemplate.getById(selection.TaskTemplateID, typeof(TaskTemplate));
                    if (i == 0)
                        LearningScenarioID = taskTemplate.ID;
                    nameTaskTemplate[i] = taskTemplate.Name;
                    i++;
                }
                if (nameTaskTemplate.Length == 0)
                {
                    CanSolve = false;
                    return new string[] { "Нет созданных преобразований" };
                }
                else return nameTaskTemplate;
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
        public List<Entity> listLearningScenarios { get; private set; }
        public List<Entity> listTaskTemplate { get; private set; }
        public List<Entity> listSelections { get; private set; }
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
            addHandler = new ActionHandler(() => Add(new LearningModel(this)), e => true);
            learnHandler = new ActionHandler(learnSolver, e => LearningList.All(lm => lm.CanSolve));
        }

        public event EventHandler OnClose;

        public string TaskName { get; }
        public string SolverName { get; }
        public TaskSolver Solver {
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

        private void learnSolver() {
            foreach (LearningModel learningModel in LearningList) {
                TaskTemplate taskTemplate = (TaskTemplate)TaskTemplate.where(new Query("TaskTemplate").addTypeQuery(TypeQuery.select)
                .addCondition("Name", "=", learningModel.SelectedPreprocessing), typeof(TaskTemplate))[0];
                Selection selection = (Selection) Selection.where(new Query("Selection").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplate.ID.ToString())
                .addCondition("Name", "=", learningModel.SelectedSelection), typeof(Selection))[0];
                int countRows = selection.RowCount;
                LearningScenario learningScenario = (LearningScenario) LearningScenario.where(new Query("LearningScenario").addTypeQuery(TypeQuery.select)
                .addCondition("Name", "=", learningModel.SelectedScenario), typeof(LearningScenario))[0];
                
                List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selection.ID.ToString()), typeof(SelectionRow));
                List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", selection.TaskTemplateID.ToString()), typeof(dms.models.Parameter));
                int stepRow = 0;
                float[][] inputData = new float[countRows][];
                float[] outputData = new float[countRows];
                string[] outputDataForStrings = new string[countRows];
                for (int i = 0; i < countRows; i++)
                {
                    inputData[i] = new float[parameters.Count - 1];
                }
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
                            if (float.TryParse(outputValue, out outputFloat))
                                outputData[stepRow] = outputFloat;
                            else
                                outputDataForStrings[stepRow] = outputValue;

                        }
                        else
                            inputData[stepRow][stepParam] = float.Parse(((ValueParameter)value[0]).Value, CultureInfo.InvariantCulture.NumberFormat);
                        stepParam++;
                    }
                    stepRow++;
                }
                INeuralNetwork isolver = null;
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
                else
                    throw new EntryPointNotFoundException();
                SeparationOfDataSet s = new SeparationOfDataSet(isolver, learningScenario, inputData, outputData);
                s.separationAndLearn();
                LearnedSolver ls = new LearnedSolver()
                {
                    SelectionID = selection.ID,
                    LearningScenarioID = learningScenario.ID,
                    TaskSolverID = Solver.ID,
                    Soul = isolver
                };
                ls.save();

                LearningQuality lq = new LearningQuality()
                {
                    LearnedSolverID = ls.ID,
                    MistakeTrain = Convert.ToInt32(s.MistakeTrain),
                    MistakeTest = Convert.ToInt32(s.MistakeTest)
                };
                lq.save();
            }
            OnClose?.Invoke(this, null);
        }
    }
}
