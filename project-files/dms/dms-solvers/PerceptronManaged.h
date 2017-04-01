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
		PerceptronManaged(PerceptronManaged^ p);

		virtual void* getAttributes() override;
		virtual void* getOperations() override;

		virtual void FetchNativeParameters() override;
		virtual void PushNativeParameters() override;

		virtual ISolver^ Copy() override;

		void SetWeights(array<array<float>^>^ weights);

		virtual ~PerceptronManaged() {}
	private:
		array<array<float>^>^ weights;
	};
}
