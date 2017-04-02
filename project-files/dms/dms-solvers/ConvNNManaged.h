#pragma once
#include "INeuralNetwork.h"
#include "ConvNNTopology.h"
#include "ConvNN.h"

namespace dms::solvers::neural_nets::conv_net
{
	[SerializableAttribute]
	public ref class ConvNNManaged : public INeuralNetwork
	{
	public:
		ConvNNManaged(ConvNNTopology^ t);
		ConvNNManaged(ConvNNManaged^ p);

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
