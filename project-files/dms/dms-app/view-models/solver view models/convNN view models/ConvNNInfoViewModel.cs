using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.solvers.neural_nets.conv_net;

namespace dms.view_models
{
    public class ConvNNLayer
    {
        public int Number { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
    }

    public class ConvNNInfoViewModel : ViewmodelBase
    {
        public string TaskName { get; }
        public string Name { get; }
        public string InputDimention { get; }
        public ConvNNLayer[] Layers { get; }
        public ConvNNLayer SelectedLayer
        {
            get { return sel_layer; }
            set
            {
                sel_layer = value;
                int lindex = -1;
                for(int i = 0; i < Layers.Length; i++)
                {
                    if (Layers[i] == sel_layer)
                    {
                        lindex = i;
                        break;
                    }
                }
                LayerParameters = ilayers[lindex];
                SelectedLayerType = sel_layer.Type;
            }
        }
        public string SelectedLayerType
        {
            get { return selectedLayerType; }
            private set
            {
                selectedLayerType = value;
                NotifyPropertyChanged();
            }
        }
        public ILayer LayerParameters
        {
            get { return layerParameters; }
            private set
            {
                layerParameters = value;
                NotifyPropertyChanged();
            }
        }

        public ConvNNInfoViewModel(models.Task task, models.TaskSolver solver)
        {
            TaskName = task.Name;
            Name = solver.Name;

            var t = solver.Description as ConvNNTopology;
            InputDimention = String.Format("{0}x{1}x{2}", t.GetInputWidth(), t.GetInputHeigth(), t.GetInputDepth());

            ilayers = t.GetLayers().ToArray();
            var volumes = t.GetVolumeDimentions();

            Layers = new ConvNNLayer[ilayers.Length];
            for(int i = 0; i < ilayers.Length; i++)
            {
                if (ilayers[i] is FullyConnectedLayer)
                {
                    Layers[i] = new ConvNNLayer
                    {
                        Number = i + 1,
                        Type = "FC",
                        Width = volumes[i].Width,
                        Height = volumes[i].Heigth,
                        Depth = volumes[i].Depth
                    };
                }
                else if (ilayers[i] is ConvolutionLayer)
                {
                    Layers[i] = new ConvNNLayer
                    {
                        Number = i + 1,
                        Type = "Conv",
                        Width = volumes[i].Width,
                        Height = volumes[i].Heigth,
                        Depth = volumes[i].Depth
                    };
                }
                else if (ilayers[i] is PoolingLayer)
                {
                    Layers[i] = new ConvNNLayer
                    {
                        Number = i + 1,
                        Type = "Pool",
                        Width = volumes[i].Width,
                        Height = volumes[i].Heigth,
                        Depth = volumes[i].Depth
                    };
                }
            }
        }

        private ILayer[] ilayers;
        private ConvNNLayer sel_layer;
        private string selectedLayerType;
        private ILayer layerParameters;
    }
}
