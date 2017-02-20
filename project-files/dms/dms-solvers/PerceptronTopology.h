#pragma once
#include "ISolverDescription.h"
#include "ActivationFunctionTypes.h"

using namespace System;
using namespace neurolib;

namespace dms::solvers::neural_nets
{
	[SerializableAttribute]
	public ref class PerceptronTopology : public ISolverDescription
	{
	public:
		PerceptronTopology(int layers, array<int>^ neurons,
			array<bool>^ delays, array<String^>^ afs);
		int GetLayersCount();
		array<int>^ GetNeuronsInLayersCount();
		array<bool>^ HasLayersDelayWeight();
		array<String^>^ GetActivationFunctionsNames();
		int GetLayersActivateFunctionsTypes(ActivationFunctionType* src);
		int GetInputsCount();
		int GetOutputsCount();
	private:
		int layersCount;
		array<int>^ neuronsInLayers;
		array<bool>^ hasLayerDelay;
		array<String^>^ afs;
	};
}
