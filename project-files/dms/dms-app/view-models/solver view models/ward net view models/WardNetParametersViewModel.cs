using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.solvers.neural_nets;
using dms.solvers.neural_nets.ward_net;
using dms.tools;
using dms.models;

using WardLayer = dms.solvers.neural_nets.ward_net.Layer;

namespace dms.view_models
{
    public class WardNetGroupViewModel : ViewmodelBase
    {
        private int number;
        private ActionHandler deleteHandler;
        private bool canDelete;

        public int Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        public long NeuronsCount { get; set; }
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
            IsUsingW0 = false;
        }
    }
    public class WardNetLayerViewModel : ViewmodelBase
    {
        private int number;
        private long maxAC;
        private long ac;
        private ActionHandler addGroupHandler;
        private ActionHandler deleteHandler;

        public int Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        public long MaxAC { get { return maxAC; } set { maxAC = value; NotifyPropertyChanged(); } }
        public long AC { get { return ac; } set { ac = value; NotifyPropertyChanged(); } }
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
        private long inputMaxAc;
        private long inputAc;

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

        public long InputNeuronsCount { get; set; }
        public long InputLayerMaxAC { get { return inputMaxAc; } private set { inputMaxAc = value;  NotifyPropertyChanged(); } }
        public long InputLayerAC { get { return inputAc; } set { inputAc = value; NotifyPropertyChanged(); } }
        public ObservableCollection<WardNetLayerViewModel> HiddenLayers { get; set; }
        public ICommand AddLayer { get { return addLayerHandler; } }
        public WardNetLayerViewModel OutputLayer { get; }

        public bool CanCreateSolver(string name, models.Task task)
        {
            return HiddenLayers.Count > 0;
        }

        public void CreateSolver(string name, models.Task task)
        {
            InputLayer input = new InputLayer
            {
                NeuronsCount = InputNeuronsCount,
                ForwardConnection = InputLayerAC
            };
            var layers = new List<WardLayer>();
            for(int i = 0; i < HiddenLayers.Count; i++)
            {
                var groups = new List<NeuronsGroup>();
                for(int j = 0; j < HiddenLayers[i].Groups.Count; j++)
                {
                    groups.Add(new NeuronsGroup
                    {
                        NeuronsCount = HiddenLayers[i].Groups[j].NeuronsCount,
                        ActivationFunction = HiddenLayers[i].Groups[j].SelectedAF,
                        HasDelay = HiddenLayers[i].Groups[j].IsUsingW0
                    });
                }
                layers.Add(new WardLayer
                {
                    ForwardConnection = HiddenLayers[i].AC,
                    Groups = groups
                });
            }

            var groupsOutput = new List<NeuronsGroup>();
            for (int j = 0; j < OutputLayer.Groups.Count; j++)
            {
                groupsOutput.Add(new NeuronsGroup
                {
                    NeuronsCount = OutputLayer.Groups[j].NeuronsCount,
                    ActivationFunction = OutputLayer.Groups[j].SelectedAF,
                    HasDelay = OutputLayer.Groups[j].IsUsingW0
                });
            }
            layers.Add(new WardLayer
            {
                ForwardConnection = OutputLayer.AC,
                Groups = groupsOutput
            });

            WardNNTopology wnn = new WardNNTopology(input, layers);

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
