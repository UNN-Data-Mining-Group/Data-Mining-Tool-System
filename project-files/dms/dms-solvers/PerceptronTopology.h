#pragma once
#include "ISolverDescription.h"
#include "ActivationFunctionTypes.h"

namespace dms::solvers::neural_nets::perceptron
{
	[System::SerializableAttribute]
	public ref class PerceptronTopology : public ISolverDescription
	{
	public:
		PerceptronTopology(int layers, array<int>^ neurons,
			array<bool>^ delays, array<System::String^>^ afs);
		int GetLayersCount();
		array<int>^ GetNeuronsInLayersCount();
		array<bool>^ HasLayersDelayWeight();
		array<System::String^>^ GetActivationFunctionsNames();
		int GetLayersActivateFunctionsTypes(nnets::ActivationFunctionType* src);
		int GetInputsCount();
		int GetOutputsCount();
	private:
		int layersCount;
		array<int>^ neuronsInLayers;
		array<bool>^ hasLayerDelay;
		array<System::String^>^ afs;
	};
}
