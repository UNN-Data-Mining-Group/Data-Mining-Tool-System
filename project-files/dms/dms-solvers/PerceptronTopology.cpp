#include "PerceptronTopology.h"

using System::IntPtr;
using System::Runtime::InteropServices::Marshal;

namespace dms::solvers::neural_nets
{
	PerceptronTopology::PerceptronTopology(int layers, array<int>^ neurons,
		array<bool>^ delays, array<String^>^ afs)
	{
		layersCount = layers;
		if ((neurons->Length != layers) ||
			(delays->Length != (layers - 1)) ||
			(afs->Length != (layers - 1)) ||
			(layersCount < 2))
		{
			throw gcnew System::ArgumentException();
		}

		neuronsInLayers = gcnew array<int>(layers);
		hasLayerDelay = gcnew array<bool>(layers - 1);
		this->afs = gcnew array<String^>(layers - 1);

		for (int i = 0; i < layers; i++)
		{
			neuronsInLayers[i] = neurons[i];
		}

		for (int i = 0; i < layers - 1; i++)
		{
			hasLayerDelay[i] = delays[i];
			this->afs[i] = afs[i];
		}
	}

	int PerceptronTopology::GetLayersCount()
	{
		return layersCount;
	}
	array<int>^ PerceptronTopology::GetNeuronsInLayersCount()
	{
		return neuronsInLayers;
	}
	array<bool>^ PerceptronTopology::HasLayersDelayWeight()
	{
		return hasLayerDelay;
	}
	array<String^>^ PerceptronTopology::GetActivationFunctionsNames()
	{
		return afs;
	}
	int PerceptronTopology::GetLayersActivateFunctionsTypes(ActivationFunctionType* src)
	{
		for (int i = 0; i < layersCount - 1; i++)
		{
			src[i] = ActivationFunctionTypes::getType(afs[i]);
		}
		return layersCount - 1;
	}
	int PerceptronTopology::GetInputsCount()
	{
		return neuronsInLayers[0];
	}
	int PerceptronTopology::GetOutputsCount()
	{
		return neuronsInLayers[layersCount - 1];
	}
}