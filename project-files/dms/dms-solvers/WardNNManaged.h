#pragma once
#include "INeuralNetwork.h"
#include "WardNNTopology.h"
#include "WardNN.h"

namespace dms::solvers::neural_nets::ward_net
{
	[SerializableAttribute]
	public ref class WardNNManaged : public INeuralNetwork
	{
	public:
		WardNNManaged(WardNNTopology^ t);
		WardNNManaged(WardNNManaged^ w);

		virtual void* getAttributes() override;
		virtual void* getOperations() override;

		virtual void FetchNativeParameters() override;
		virtual void PushNativeParameters() override;

		virtual ISolver^ Copy() override;

		void SetWeights(array<array<float>^>^ weights);
	private:
		array<array<float>^>^ weights;
	};
}