using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.solvers.neural_nets;
using dms.solvers.neural_nets.perceptron;
using dms.tools;
using dms.models;

namespace dms.view_models
{
    public class LayerViewModel : ViewmodelBase
    {
        private ActionHandler deleteHandler;
        private int number;

        public LayerViewModel(int number = -1, Action<LayerViewModel> delete = null)
        {
            deleteHandler = new ActionHandler(()=>delete(this), o => true);
            Number = number;
            NeuronsCount = 1;
            ActivateFunctions = ActivationFunctionTypes.TypeNames;
            SelectedAF = ActivateFunctions[0];
            IsUsingW0 = false;
        }

        public int Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        public int NeuronsCount { get; set; }
        public string[] ActivateFunctions { get; }
        public string SelectedAF { get; set; }
        public bool IsUsingW0 { get; set; }
        public ICommand Delete { get { return deleteHandler; } }
    }

    public class PerceptronParametersViewModel : ISolverParameterViewModel
    {
        private ActionHandler addLayerHandler;

        public event Action CanCreateChanged;

        public PerceptronParametersViewModel()
        {
            HiddenLayers = new ObservableCollection<LayerViewModel> ();
            OutputLayer = new LayerViewModel();
            InputNeuronsCount = 1;
            addLayerHandler = new ActionHandler(
                () => 
                {
                    HiddenLayers.Add(new LayerViewModel(HiddenLayers.Count + 1, DeleteHiddenLayer));
                    CanCreateChanged?.Invoke();
                }, o => true);
        }
        public ObservableCollection<LayerViewModel> HiddenLayers { get; private set; }
        public LayerViewModel OutputLayer { get; }
        public int InputNeuronsCount { get; set; }
        public ICommand AddLayer { get { return addLayerHandler; } }
        public void DeleteHiddenLayer(LayerViewModel l)
        {
            if (HiddenLayers.Contains(l))
            {
                HiddenLayers.Remove(l);
                for(int i = 0; i < HiddenLayers.Count; i++)
                {
                    HiddenLayers[i].Number = i + 1;
                }
            }
            CanCreateChanged?.Invoke();
        }

        public void CreateSolver(string name, models.Task task)
        {
            int layers = 2 + HiddenLayers.Count;
            int[] neurons = new int[layers];
            bool[] delays = new bool[layers - 1];
            string[] afs = new string[layers - 1];

            neurons[0] = InputNeuronsCount;
            for (int i = 1; i < layers - 1; i++)
                neurons[i] = HiddenLayers[i-1].NeuronsCount;
            neurons[layers - 1] = OutputLayer.NeuronsCount;

            for(int i = 0; i < layers - 2; i++)
            {
                delays[i] = HiddenLayers[i].IsUsingW0;
                afs[i] = HiddenLayers[i].SelectedAF;
            }
            delays[layers - 2] = OutputLayer.IsUsingW0;
            afs[layers - 2] = OutputLayer.SelectedAF;

            PerceptronTopology t = new PerceptronTopology(layers, neurons, delays, afs);

            TaskSolver ts = new TaskSolver()
            {
                Name = name,
                TypeName = "Perceptron",
                TaskID = task.ID, 
                Description = t
            };

            ts.save();
        }

        public bool CanCreateSolver(string name, models.Task task)
        {
            return true;
        }
    }
}
