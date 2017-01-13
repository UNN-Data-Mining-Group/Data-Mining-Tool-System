#pragma once
#include "ISolverDescription.h"
#include "IActivateFunction.h"

using namespace System;

namespace dms::solvers::neural_nets
{
	public ref class PerceptronTopology : public ISolverDescription
	{
	public:
		PerceptronTopology(int layers, array<int>^ neurons,
			array<bool>^ delays, array<IActivateFunction^>^ afs);
		int GetLayersCount();
		array<int>^ GetNeuronsInLayersCount();
		array<bool>^ HasLayersDelayWeight();
		array<IActivateFunction^>^ GetLayersActivateFunctions();
		int GetInputsCount();
		int GetOutputsCount();
	private:
		int layersCount;
		array<int>^ neuronsInLayers;
		array<bool>^ hasLayerDelay;
		array<IActivateFunction^>^ afs;
	};
}
