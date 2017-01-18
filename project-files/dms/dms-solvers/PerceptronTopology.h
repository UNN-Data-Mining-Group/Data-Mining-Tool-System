#pragma once
#include "ISolverDescription.h"
#include "ActivationFunctionTypes.h"

using namespace System;
using namespace neurolib;

namespace dms::solvers::neural_nets
{
	public ref class PerceptronTopology : public ISolverDescription
	{
	public:
		PerceptronTopology(int layers, array<int>^ neurons,
			array<bool>^ delays, array<String^>^ afs);
		int GetLayersCount();
		array<int>^ GetNeuronsInLayersCount();
		array<bool>^ HasLayersDelayWeight();
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
