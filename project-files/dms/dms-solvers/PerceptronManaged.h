#pragma once
#include "INeuralNetwork.h"
#include "PerceptronTopology.h"
#include "Perceptron.h"

namespace dms::solvers::neural_nets::perceptron
{
	[SerializableAttribute]
	public ref class PerceptronManaged : public INeuralNetwork
	{
	public:
		PerceptronManaged(PerceptronTopology^ t);

		virtual array<Single>^ Solve(array<Single>^ x) override;
		virtual std::vector<std::string> getAttributes() override;
		virtual std::map<std::string, void*> getOperations() override;
		virtual void* getNativeSolver() override;

		virtual void FetchNativeParameters() override;
		virtual void PushNativeParameters() override;

		void SetWeights(array<array<float>^>^ weights);

		virtual ~PerceptronManaged();
	private:
		array<array<float>^>^ weights;
		PerceptronTopology^ t;

		[NonSerializedAttribute]
		float* x;
		[NonSerializedAttribute]
		float* y;
		[NonSerializedAttribute]
		nnets_perceptron::Perceptron* psolver;

		bool hasSmoothAfs;

		void initPerceptron();
	};
}
