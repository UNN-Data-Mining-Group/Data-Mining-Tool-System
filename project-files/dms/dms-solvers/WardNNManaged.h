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

		virtual array<Single>^ Solve(array<Single>^ x) override;
		virtual std::vector<std::string> getAttributes() override;
		virtual std::map<std::string, void*> getOperations() override;
		virtual void* getNativeSolver() override;

		virtual void FetchNativeParameters() override;
		virtual void PushNativeParameters() override;

		void SetWeights(array<array<float>^>^ weights);

		virtual ~WardNNManaged();

	private:
		array<array<float>^>^ weights;
		WardNNTopology^ t;

		[NonSerializedAttribute]
		float *x, *y;
		[NonSerializedAttribute]
		nnets_ward::WardNN* wsolver;

		void initWard();
	};
}