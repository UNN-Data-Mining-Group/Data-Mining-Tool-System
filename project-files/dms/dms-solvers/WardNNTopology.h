#pragma once
#include "ISolverDescription.h"
#include "ActivationFunctionTypes.h"

using System::Collections::Generic::List;

namespace dms::solvers::neural_nets::ward_net
{
	[System::SerializableAttribute]
	public ref class InputLayer
	{
	public: 
		System::Int64 NeuronsCount;
		System::Int64 ForwardConnection;
	};

	[System::SerializableAttribute]
	public ref class NeuronsGroup
	{
	public:
		System::Int64 NeuronsCount;
		bool HasDelay;
		System::String^ ActivationFunction;
	};

	[System::SerializableAttribute]
	public ref class Layer
	{
	public:
		List<NeuronsGroup^>^ Groups;
		System::Int64 ForwardConnection;
	};

	[System::SerializableAttribute]
	public ref class WardNNTopology : public ISolverDescription
	{
	public:
		WardNNTopology(InputLayer^ input, List<Layer^>^ layers);
		System::Int64 GetInputsCount();
		System::Int64 GetOutputsCount();
		InputLayer^ GetInputLayer();
		List<Layer^>^ GetLayers();
	private:
		InputLayer^ inputLayer;
		List<Layer^>^ layers;
	};
}