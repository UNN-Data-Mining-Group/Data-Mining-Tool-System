#pragma once
#include "ISolver.h"
#include "PerceptronTopology.h"
#include "Perceptron.h"

namespace dms::solvers::neural_nets::perceptron
{
	public ref class PerceptronManaged : public ISolver
	{
	public:
		PerceptronManaged(PerceptronTopology^ t, array<array<float>^>^ weights);

		virtual array<Single>^ Solve(array<Single>^ x) override;
		virtual std::vector<std::string> getAttributes() override;
		virtual std::vector<LearningOperation> getOperations() override;
		virtual void* getNativeSolver() override;

		virtual ~PerceptronManaged();
	private:

		float* x;
		float* y;

		nnets_perceptron::Perceptron* psolver;
		array<array<float>^>^ weights;
	};
}
