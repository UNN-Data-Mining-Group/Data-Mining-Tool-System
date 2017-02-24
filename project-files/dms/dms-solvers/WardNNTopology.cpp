#include "WardNNTopology.h"

using dms::solvers::neural_nets::ward_net::WardNNTopology;
using dms::solvers::neural_nets::ward_net::Layer;
using dms::solvers::neural_nets::ward_net::InputLayer;
using System::String;
using System::ArgumentException;

WardNNTopology::WardNNTopology(InputLayer^ input, List<Layer^>^ layers)
{
	inputLayer = input;
	this->layers = layers;
}

System::Int64 WardNNTopology::GetInputsCount()
{
	return inputLayer->NeuronsCount;
}

System::Int64 WardNNTopology::GetOutputsCount()
{
	List<NeuronsGroup^>^ groups = layers[layers->Count - 1]->Groups;
	__int64 res = 0;
	for (int i = 0; i < groups->Count; i++)
		res += groups[i]->NeuronsCount;
	return res;
}

InputLayer^ WardNNTopology::GetInputLayer()
{
	return inputLayer;
}

List<Layer^>^ WardNNTopology::GetLayers()
{
	return layers;
}