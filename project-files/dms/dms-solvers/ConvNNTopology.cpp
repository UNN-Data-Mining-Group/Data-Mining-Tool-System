#include "ConvNNTopology.h"

using namespace dms::solvers::neural_nets;

ConvNNTopology::ConvNNTopology(int input_weight, int input_heigth, int input_depth, List<IConvNNLayer^>^ layers)
{
	w = input_weight;
	h = input_heigth;
	d = input_depth;

	if (layers->Count == 0)
		throw gcnew System::ArgumentException("В сети нет слоев");

	this->layers = gcnew List<IConvNNLayer^>();
	layerDimentions = gcnew List<VolumeDimention^>();

	int bw = w;
	int bh = h;
	int bd = d;
	for (int i = 0; i < layers->Count; i++)
	{
		this->layers->Add(layers[i]);
		auto d = gcnew VolumeDimention();
		if (layers[i]->GetType() == ConvNNFullyConnectedLayer::typeid)
		{
			auto l = (ConvNNFullyConnectedLayer^)layers[i];
			bw = d->Width = 1;	
			bh = d->Heigth = 1;	
			bd = d->Depth = l->NeuronsCount;
		}
		else if (layers[i]->GetType() == ConvNNConvolutionLayer::typeid)
		{
			auto l = (ConvNNConvolutionLayer^)layers[i];
			bw = d->Width = calcOutputVolume(bw, l->FilterWidth, l->Padding, l->StrideWidth);	
			bh = d->Heigth = calcOutputVolume(bh, l->FilterHeight, l->Padding, l->StrideHeight);
			bd = d->Depth = l->CountFilters;
		}
		else if (layers[i]->GetType() == ConvNNPoolingLayer::typeid)
		{
			auto l = (ConvNNPoolingLayer^)layers[i];
			bw = d->Width = calcOutputVolume(bw, l->FilterWidth, 0, l->StrideWidth);
			bh = d->Heigth = calcOutputVolume(bh, l->FilterHeight, 0, l->StrideHeight);
			d->Depth = bd;
		}
		layerDimentions->Add(d);
	}
}

int ConvNNTopology::calcOutputVolume(int input, int filter, int padding, int stride)
{
	int temp = input - filter + 2 * padding;
	if ((temp < 0) || ((temp % stride) != 0))
	{
		throw gcnew System::ArgumentOutOfRangeException("Ошибка размерности");
	}
	return temp / stride + 1;
}

int ConvNNTopology::GetInputWidth() { return w; }
int ConvNNTopology::GetInputHeigth() { return h; }
int ConvNNTopology::GetInputDepth() { return d; }
int ConvNNTopology::GetInputsCount() { return w*h*d; }
int ConvNNTopology::GetOutputsCount()
{
	VolumeDimention^ l = layerDimentions[layerDimentions->Count - 1];
	return l->Width * l->Heigth * l->Depth;
}
List<IConvNNLayer^>^ ConvNNTopology::GetLayers() { return layers; }

List<ConvNNTopology::VolumeDimention^>^ ConvNNTopology::GetVolumeDimentions() { return layerDimentions; }