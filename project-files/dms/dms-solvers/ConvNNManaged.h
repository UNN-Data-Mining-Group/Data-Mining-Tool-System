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

		virtual array<float>^ Solve(array<float>^ x) override;
		virtual void* getAttributes() override;
		virtual void* getOperations() override;
		virtual void* getNativeSolver() override;

		virtual void FetchNativeParameters() override;
		virtual void PushNativeParameters() override;

		void SetWeights(array<array<float>^>^ weights);

		virtual ~ConvNNManaged();
	private:
		array<array<float>^>^ weights;
		ConvNNTopology^ t;

		[NonSerializedAttribute]
		float *x;
		[NonSerializedAttribute]
		float *y;
		[NonSerializedAttribute]
		nnets_conv::ConvNN* solver;

		void initConv();
	};
}
