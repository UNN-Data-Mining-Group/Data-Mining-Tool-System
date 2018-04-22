#pragma once
#include "ITopology.h"
#include "ActivationFunctionTypes.h"

namespace dms::solvers::neural_nets::perceptron
{
	[System::SerializableAttribute]
	public ref class PerceptronTopology : public ITopology
	{
	public:
		PerceptronTopology(int layers, array<int>^ neurons,
			array<bool>^ delays, array<System::String^>^ afsint,int start, int end);
		int GetLayersCount();
		array<int>^ GetNeuronsInLayersCount();
		array<bool>^ HasLayersDelayWeight();
		array<System::String^>^ GetActivationFunctionsNames();
		int GetLayersActivateFunctionsTypes(nnets::ActivationFunctionType* src);
		bool HasSmoothAfs() { return hasSmoothAfs; }
		virtual System::Int64 GetInputsCount();
		virtual System::Int64 GetOutputsCount();
		virtual nnets::NeuralNetwork * createNativeSolver();
	private:
		int start;
		int end;
		int layersCount;
		bool hasSmoothAfs;
		array<int>^ neuronsInLayers;
		array<bool>^ hasLayerDelay;
		array<System::String^>^ afs;
	};
}
