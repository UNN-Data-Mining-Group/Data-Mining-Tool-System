#pragma once
#include "ITopology.h"
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
	public ref class WardNNTopology : public ITopology
	{
	public:
		WardNNTopology(InputLayer^ input, List<Layer^>^ layers);
		InputLayer^ GetInputLayer();
		List<Layer^>^ GetLayers();
		bool HasSmoothAfs() { return hasSmoothAfs; }
		virtual System::Int64 GetInputsCount();
		virtual System::Int64 GetOutputsCount();
		virtual nnets::NeuralNetwork * createNativeSolver();
	private:
		bool hasSmoothAfs;
		InputLayer^ inputLayer;
		List<Layer^>^ layers;
	};
}