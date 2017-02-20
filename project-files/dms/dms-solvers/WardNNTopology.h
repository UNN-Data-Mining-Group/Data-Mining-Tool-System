#pragma once
#include "ISolverDescription.h"
#include "ActivationFunctionTypes.h"

using namespace System;
using namespace neurolib;

namespace dms::solvers::neural_nets
{
	[SerializableAttribute]
	public ref class WardNNTopology : public ISolverDescription
	{
	public:
		WardNNTopology(array<array<int>^>^ neurons, array<array<bool>^>^ delays, array<array<String^>^>^ afs,
			array<int>^ groups, array<int>^ additionalConnections, int layers);
		int getInputsCount();
		int getOutputsCount();
		int getLayersCount();
		int getAdditionalConnections(int* src);
		int getGroupsCount(int* src);
		int getActivateFunctionsTypes(ActivationFunctionType** src);
		int getDelays(bool** src);
		int getNeuronsCount(int** src);

		array<int>^ GetAdditionalConnections();
		array<int>^ GetGroupsCount();
		array<array<String^>^>^ GetActivationFunctions();
		array<array<bool>^>^ GetDelays();
		array<array<int>^>^ GetNeuronsCount();
	private:
		int layersCount;
		array<int>^ addCons;
		array<int>^ groupsCount;
		array<array<String^>^>^ afs;
		array<array<bool>^>^ delays;
		array<array<int>^>^ neurons;
	};
}