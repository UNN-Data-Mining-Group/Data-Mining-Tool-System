using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.solvers.neural_nets;
using dms.tools;
using dms.models;

namespace dms.view_models
{
    public class WardNetGroupViewModel : ViewmodelBase
    {
        private int number;
        private ActionHandler deleteHandler;
        private bool canDelete;

        public int Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        public int NeuronsCount { get; set; }
        public string[] ActivateFunctions { get; }
        public string SelectedAF { get; set; }
        public bool IsUsingW0 { get; set; }
        public ICommand Delete { get { return deleteHandler; } }
        public bool CanDelete { get { return canDelete; } set { canDelete = value; deleteHandler.RaiseCanExecuteChanged(); NotifyPropertyChanged(); } }

        public WardNetGroupViewModel(Action<WardNetGroupViewModel> delete)
        {
            ActivateFunctions = ActivationFunctionTypes.TypeNames;
            deleteHandler = new ActionHandler(() => delete(this), e => CanDelete);

            CanDelete = false;
            Number = 1;
            NeuronsCount = 1;
            SelectedAF = ActivateFunctions[0];
            IsUsingW0 = true;
        }
    }
    public class WardNetLayerViewModel : ViewmodelBase
    {
        private int number;
        private int maxAC;
        private int ac;
        private ActionHandler addGroupHandler;
        private ActionHandler deleteHandler;

        public int Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        public int MaxAC { get { return maxAC; } set { maxAC = value; NotifyPropertyChanged(); } }
        public int AC { get { return ac; } set { ac = value; NotifyPropertyChanged(); } }
        public ICommand Delete { get { return deleteHandler; } }
        public ICommand AddGroup { get { return addGroupHandler; } }
        public ObservableCollection<WardNetGroupViewModel> Groups { get; set; }

        public WardNetLayerViewModel(Action<WardNetLayerViewModel> delete = null)
        {
            Action<WardNetGroupViewModel> del = g => 
            {
                Groups.Remove(g);
                for (int i = 0; i < Groups.Count; i++)
                    Groups[i].Number = i + 1;
                if (Groups.Count == 1)
                    Groups[0].CanDelete = false;
            };

            Number = 1;
            MaxAC = 0;
            AC = 0;
            Groups = new ObservableCollection<WardNetGroupViewModel> { new WardNetGroupViewModel(del) };
            addGroupHandler = new ActionHandler(() => 
            {
                Groups.Add(new WardNetGroupViewModel(del) { Number = Groups.Count + 1 });
                for (int i = 0; i < Groups.Count; i++)
                    Groups[i].CanDelete = true;
            }, e => true);
            deleteHandler = new ActionHandler(() => delete(this), e => true);
        }
    }

    public class WardNetParametersViewModel : ViewmodelBase, ISolverParameterViewModel
    {
        private ActionHandler addLayerHandler;
        private int inputMaxAc;
        private int inputAc;

        public event Action CanCreateChanged;

        public WardNetParametersViewModel()
        {
            InputNeuronsCount = 1;
            HiddenLayers = new ObservableCollection<WardNetLayerViewModel>();
            OutputLayer = new WardNetLayerViewModel();
            addLayerHandler = new ActionHandler(
                () => 
                {
                    for (int i = 0; i < HiddenLayers.Count; i++)
                        HiddenLayers[i].MaxAC = HiddenLayers.Count - i;
                    InputLayerMaxAC = HiddenLayers.Count + 1;

                    HiddenLayers.Add(new WardNetLayerViewModel(e =>
                    {
                        HiddenLayers.Remove(e);
                        for (int i = 0; i < HiddenLayers.Count; i++)
                        {
                            HiddenLayers[i].MaxAC = HiddenLayers.Count - i - 1;
                            if (HiddenLayers[i].AC > HiddenLayers[i].MaxAC)
                                HiddenLayers[i].AC = HiddenLayers[i].MaxAC;
                            HiddenLayers[i].Number = i + 1;
                        }
                        InputLayerMaxAC = HiddenLayers.Count;
                        if (InputLayerAC > InputLayerMaxAC)
                            InputLayerAC = InputLayerMaxAC;
                        CanCreateChanged();
                    }) { Number = (HiddenLayers.Count + 1) });
                    CanCreateChanged();
                }, 
                e => true);
        }

        public int InputNeuronsCount { get; set; }
        public int InputLayerMaxAC { get { return inputMaxAc; } private set { inputMaxAc = value;  NotifyPropertyChanged(); } }
        public int InputLayerAC { get { return inputAc; } set { inputAc = value; NotifyPropertyChanged(); } }
        public ObservableCollection<WardNetLayerViewModel> HiddenLayers { get; set; }
        public ICommand AddLayer { get { return addLayerHandler; } }
        public WardNetLayerViewModel OutputLayer { get; }

        public bool CanCreateSolver(string name, models.Task task)
        {
            return HiddenLayers.Count > 0;
        }

        public void CreateSolver(string name, models.Task task)
        {
            int[] g = new int[HiddenLayers.Count + 1];
            int[] ac = new int[HiddenLayers.Count];
            bool[][] delays = new bool[HiddenLayers.Count + 1][];
            string[][] afs = new string[HiddenLayers.Count + 1][];
            int[][] neurons = new int[HiddenLayers.Count + 2][];

            neurons[0] = new int[1];
            neurons[0][0] = InputNeuronsCount;
            ac[0] = InputLayerAC;
            for(int i = 0; i < HiddenLayers.Count - 1; i++)
            {
                ac[i + 1] = HiddenLayers[i].AC;
            }

            for(int i = 0; i < HiddenLayers.Count; i++)
            {
                int groups = HiddenLayers[i].Groups.Count;
                g[i] = groups;
                neurons[i + 1] = new int[groups];
                delays[i] = new bool[groups];
                afs[i] = new string[groups];

                for (int j = 0; j < groups; j++)
                {
                    neurons[i + 1][j] = HiddenLayers[i].Groups[j].NeuronsCount;
                    delays[i][j] = HiddenLayers[i].Groups[j].IsUsingW0;
                    afs[i][j] = HiddenLayers[i].Groups[j].SelectedAF;
                }
            }
            g[HiddenLayers.Count] = OutputLayer.Groups.Count;
            neurons[HiddenLayers.Count + 1] = new int[OutputLayer.Groups.Count];
            delays[HiddenLayers.Count] = new bool[OutputLayer.Groups.Count];
            afs[HiddenLayers.Count] = new string[OutputLayer.Groups.Count];

            for (int j = 0; j < OutputLayer.Groups.Count; j++)
            {
                neurons[HiddenLayers.Count + 1][j] = OutputLayer.Groups[j].NeuronsCount;
                delays[HiddenLayers.Count][j] = OutputLayer.Groups[j].IsUsingW0;
                afs[HiddenLayers.Count][j] = OutputLayer.Groups[j].SelectedAF;
            }

            WardNNTopology wnn = new WardNNTopology(neurons, delays, afs, g, ac, HiddenLayers.Count + 2);

            TaskSolver ts = new TaskSolver()
            {
                Name = name,
                TypeName = "WardNN",
                TaskID = task.ID,
                Description = wnn
            };

            ts.save();
        }
    }
}
