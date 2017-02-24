using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using dms.tools;
using dms.solvers.neural_nets;
using dms.solvers.neural_nets.conv_net;

namespace dms.view_models
{
    public interface IConvNNLayerParametersVM
    {
        ILayer LayerModel { get; }
        event Func<bool> OnUpdate;
    }

    public class ConvNNConvLayerParametersVM : IConvNNLayerParametersVM
    {
        private int fw, fh, fc, p, sw, sh;
        private string af;

        public int FilterWidth { get { return fw; } set { fw = value; OnUpdate?.Invoke(); } }
        public int FilterHeight { get { return fh; } set { fh = value; OnUpdate?.Invoke(); } }
        public int FilterCount { get { return fc; } set { fc = value; OnUpdate?.Invoke(); } }
        public int Padding { get { return p; } set { p = value; OnUpdate?.Invoke(); } }
        public int StepWidth { get { return sw; } set { sw = value; OnUpdate?.Invoke(); } }
        public int StepHeight { get { return sh; } set { sh = value; OnUpdate?.Invoke(); } }
        public string[] ActivationFunctionNames { get { return ActivationFunctionTypes.TypeNames; } }
        public string ActivationFunction { get { return af; } set { af = value; OnUpdate?.Invoke(); } }

        public ConvNNConvLayerParametersVM(Func<bool> onUpdate)
        {
            FilterWidth = FilterHeight = 3;
            FilterCount = 1;
            Padding = 0;
            StepWidth = StepHeight = 1;
            ActivationFunction = ActivationFunctionNames[0];

            OnUpdate += onUpdate;
            OnUpdate?.Invoke();
        }

        public ILayer LayerModel
        {
            get
            {
                return new ConvolutionLayer
                {
                    FilterWidth = this.FilterWidth,
                    FilterHeight = this.FilterHeight,
                    CountFilters = this.FilterCount,
                    Padding = this.Padding,
                    StrideWidth = this.StepWidth,
                    StrideHeight = this.StepHeight,
                    ActivationFunction = this.ActivationFunction
                };
            }
        }

        public event Func<bool> OnUpdate;
    }

    public class ConvNNPoolLayerParametersVM : IConvNNLayerParametersVM
    {
        private int fw, fh, sw, sh;

        public int FilterWidth { get { return fw; } set { fw = value; OnUpdate?.Invoke(); } }
        public int FilterHeight { get { return fh; } set { fh = value; OnUpdate?.Invoke(); } }
        public int StepWidth { get { return sw; } set { sw = value; OnUpdate?.Invoke(); } }
        public int StepHeight { get { return sh; } set { sh = value; OnUpdate?.Invoke(); } }

        public ConvNNPoolLayerParametersVM(Func<bool> onUpdate)
        {
            FilterWidth = FilterHeight = 3;
            StepWidth = StepHeight = 1;

            OnUpdate += onUpdate;
            OnUpdate?.Invoke();
        }

        public ILayer LayerModel
        {
            get
            {
                return new PoolingLayer
                {
                    FilterWidth = this.FilterWidth,
                    FilterHeight = this.FilterHeight,
                    StrideWidth = this.StepWidth,
                    StrideHeight = this.StepHeight
                };
            }
        }

        public event Func<bool> OnUpdate;
    }

    public class ConvNNFullyConnLayerParametersVM : IConvNNLayerParametersVM
    {
        private int neuronsCount;
        private string af;

        public int NeuronsCount { get { return neuronsCount; } set { neuronsCount = value;  OnUpdate?.Invoke(); } }
        public string[] ActivationFunctionNames { get { return ActivationFunctionTypes.TypeNames; } }
        public string ActivationFunction { get { return af; } set { af = value; OnUpdate?.Invoke(); } }

        public ConvNNFullyConnLayerParametersVM(Func<bool> onUpdate)
        {
            NeuronsCount = 1;
            af = ActivationFunctionNames[0];

            OnUpdate += onUpdate;
            OnUpdate?.Invoke();
        }

        public ILayer LayerModel
        {
            get
            {
                return new FullyConnectedLayer
                {
                    NeuronsCount = this.NeuronsCount,
                    ActivationFunction = this.ActivationFunction
                };
            }
        }

        public event Func<bool> OnUpdate;
    }

    public class ConvNNLayerViewModel : ViewmodelBase
    {
        private int layerNumber;
        private string selectedType;
        private ActionHandler deleteHandler;
        private Func<bool> onUpdate;
        private IConvNNLayerParametersVM layerParams;

        public ConvNNLayerViewModel(int number, Action<ConvNNLayerViewModel> delete, Func<bool> onUpdate)
        {
            deleteHandler = new ActionHandler(()=>delete(this), e => true);
            this.onUpdate = onUpdate;
            LayerNumber = number;
        }

        public int LayerNumber
        {
            get { return layerNumber; }
            set { layerNumber = value; NotifyPropertyChanged(); }
        }

        public ICommand Delete { get { return deleteHandler; } }
        public string[] LayerTypes
        {
            get
            {
                return new string[] 
                {
                    "Сверточный",
                    "Сужающий",
                    "Полный"
                };
            }
        }
        public string SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                NotifyPropertyChanged();

                if (selectedType == "Сверточный")
                    LayerParameters = new ConvNNConvLayerParametersVM(onUpdate);
                else if (selectedType == "Сужающий")
                    LayerParameters = new ConvNNPoolLayerParametersVM(onUpdate);
                else if (selectedType == "Полный")
                    LayerParameters = new ConvNNFullyConnLayerParametersVM(onUpdate);
                else
                    LayerParameters = null;
                onUpdate();
            }
        }
        public IConvNNLayerParametersVM LayerParameters
        {
            get { return layerParams; }
            set { layerParams = value; NotifyPropertyChanged(); }
        }
    }

    public class ConvNNParametersViewModel : ViewmodelBase, ISolverParameterViewModel
    {
        private ActionHandler addLayerHandler;
        private string errorMessage;
        private int iw, ih, id;

        public ConvNNParametersViewModel()
        {
            InputWidth = InputHeight = InputDepth = 1;
            Layers = new ObservableCollection<ConvNNLayerViewModel>();
            addLayerHandler = new ActionHandler(() => 
            {
                Layers.Add(new ConvNNLayerViewModel(Layers.Count + 1, DeleteLayer, canCreateTopology));
                canCreateTopology();
            }, e => true);
            canCreateTopology();
        }

        public int InputWidth { get { return iw; } set { iw = value; canCreateTopology(); } }
        public int InputHeight { get { return ih; } set { ih = value; canCreateTopology(); } }
        public int InputDepth { get { return id; } set { id = value; canCreateTopology(); } }
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; NotifyPropertyChanged(); CanCreateChanged?.Invoke(); }
        }
        public ObservableCollection<ConvNNLayerViewModel> Layers { get; private set; }

        public ICommand AddLayer { get { return addLayerHandler; } }

        public event Action CanCreateChanged;

        public void DeleteLayer(ConvNNLayerViewModel l)
        {
            if (Layers.Contains(l))
            {
                Layers.Remove(l);
                for (int i = 0; i < Layers.Count; i++)
                {
                    Layers[i].LayerNumber = i + 1;
                }
            }
            canCreateTopology();
        }

        public bool CanCreateSolver(string name, models.Task task)
        {
            return ErrorMessage == "";
        }

        public void CreateSolver(string name, models.Task task)
        {
            var layers = new List<ILayer>();
            foreach (ConvNNLayerViewModel layerVM in Layers)
            {
                layers.Add(layerVM.LayerParameters.LayerModel);
            }

            var t = new ConvNNTopology(InputWidth, InputHeight, InputDepth, layers);
            TaskSolver ts = new TaskSolver()
            {
                Name = name,
                TypeName = "ConvNN",
                TaskID = task.ID,
                Description = t
            };

            ts.save();
        }

        private bool canCreateTopology()
        {
            var layers = new List<ILayer>();
            try
            {
                foreach (ConvNNLayerViewModel layerVM in Layers)
                {
                    layers.Add(layerVM.LayerParameters.LayerModel);
                }

                var t = new ConvNNTopology(InputWidth, InputHeight, InputDepth, layers);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }

            ErrorMessage = "";
            return true;
        }
    }
}
