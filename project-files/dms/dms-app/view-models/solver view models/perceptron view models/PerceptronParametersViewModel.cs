using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

using dms.tools;

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
            ActivateFunctions = new string[] { "Сигмоидальная", "Пороговая" };
            SelectedAF = ActivateFunctions[0];
            IsUsingW0 = true;
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

        public void CreateSolver(string name, string taskName)
        {
            
        }

        public bool CanCreateSolver(string name, string taskName)
        {
            return HiddenLayers.Count > 0;
        }
    }
}
