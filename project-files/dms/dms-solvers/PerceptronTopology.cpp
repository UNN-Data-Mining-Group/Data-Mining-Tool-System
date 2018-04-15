#include "PerceptronTopology.h"
#include "Perceptron.h"

namespace dms::solvers::neural_nets::perceptron
{
	PerceptronTopology::PerceptronTopology(int layers, array<int>^ neurons,
		array<bool>^ delays, array<System::String^>^ afs, int start, int end)
	{
		this->start = start;
		this->end = end;
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
		this->afs = gcnew array<System::String^>(layers - 1);

		for (int i = 0; i < layers; i++)
		{
			neuronsInLayers[i] = neurons[i];
		}

		for (int i = 0; i < layers - 1; i++)
		{
			hasLayerDelay[i] = delays[i];
			this->afs[i] = afs[i];
		}

		hasSmoothAfs = true;
		for (int i = 0; i < layersCount - 1; i++)
		{
			if (ActivationFunctionTypes::hasSmoothDerivative(afs[i]) == false)
			{
				hasSmoothAfs = false;
				break;
			}
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
	array<System::String^>^ PerceptronTopology::GetActivationFunctionsNames()
	{
		return afs;
	}
	int PerceptronTopology::GetLayersActivateFunctionsTypes(nnets::ActivationFunctionType* src)
	{
		for (int i = 0; i < layersCount - 1; i++)
		{
			src[i] = ActivationFunctionTypes::getType(afs[i]);
		}
		return layersCount - 1;
	}
	System::Int64 PerceptronTopology::GetInputsCount()
	{
		return neuronsInLayers[0];
	}
	System::Int64 PerceptronTopology::GetOutputsCount()
	{
		return neuronsInLayers[layersCount - 1];
	}
	nnets::NeuralNetwork* PerceptronTopology::createNativeSolver()
	{
		nnets::ActivationFunctionType* afs = 
			new nnets::ActivationFunctionType[layersCount - 1];
		for (int i = 0; i < layersCount - 1; i++)
			afs[i] = ActivationFunctionTypes::getType(this->afs[i]);

		int* neurons = new int[layersCount];
		bool* delays = new bool[layersCount - 1];

		for (int i = 0; i < layersCount; i++)
			neurons[i] = neuronsInLayers[i];
		for (int i = 0; i < layersCount - 1; i++)
		{
			delays[i] = hasLayerDelay[i];
		}

		nnets_perceptron::Perceptron* psolver =
			new nnets_perceptron::Perceptron(neurons, delays, afs, layersCount, start, end);

		delete[] neurons;
		delete[] delays;
		delete[] afs;

		return psolver;
	}
}