#include "WardNNTopology.h"
#include "WardNN.h"

using dms::solvers::neural_nets::ward_net::WardNNTopology;
using dms::solvers::neural_nets::ward_net::Layer;
using dms::solvers::neural_nets::ward_net::InputLayer;
using System::String;
using System::ArgumentException;

WardNNTopology::WardNNTopology(InputLayer^ input, List<Layer^>^ layers)
{
	inputLayer = input;
	this->layers = layers;

	hasSmoothAfs = true;
	for (int i = 0; i < layers->Count; i++)
	{
		for (int j = 0; j < layers[i]->Groups->Count; j++)
		{
			if (ActivationFunctionTypes::hasSmoothDerivative(
				layers[i]->
				Groups[j]->
				ActivationFunction) == false)
			{
				hasSmoothAfs = false;
				break;
			}
		}
	}
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

nnets::NeuralNetwork * WardNNTopology::createNativeSolver()
{
	String^ exMessage = "";
	bool isCreationSuccessfull = true;
	nnets_ward::WardNN* wsolver = nullptr;

	try
	{
		nnets_ward::InputLayer native_input
		{
			static_cast<size_t>(inputLayer->NeuronsCount),
			static_cast<size_t>(inputLayer->ForwardConnection)
		};
		std::vector<nnets_ward::Layer> native_layers;
		for (int i = 0; i < layers->Count; i++)
		{
			List<NeuronsGroup^>^ g = layers[i]->Groups;
			std::vector<nnets_ward::NeuronsGroup> groups;
			for (int j = 0; j < g->Count; j++)
			{
				groups.push_back(nnets_ward::NeuronsGroup
				{
					static_cast<size_t>(g[j]->NeuronsCount),
					g[j]->HasDelay,
					ActivationFunctionTypes::getType(g[j]->ActivationFunction)
				});
			}
			native_layers.push_back(nnets_ward::Layer{ static_cast<size_t>(layers[i]->ForwardConnection), groups });
		}

		wsolver = new nnets_ward::WardNN(native_input, native_layers);
	}
	catch (char* msg)
	{
		isCreationSuccessfull = false;
		exMessage = gcnew String(msg);
	}
	catch (...)
	{
		isCreationSuccessfull = false;
	}

	if (isCreationSuccessfull == false)
	{
		throw gcnew System::Exception(exMessage);
	}

	return wsolver;
}

InputLayer^ WardNNTopology::GetInputLayer()
{
	return inputLayer;
}

List<Layer^>^ WardNNTopology::GetLayers()
{
	return layers;
}