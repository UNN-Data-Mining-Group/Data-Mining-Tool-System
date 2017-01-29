#pragma once
#include "ISolver.h"
#include "PerceptronTopology.h"
#include "Perceptron.h"

namespace dms::solvers::neural_nets
{
	public ref class PerceptronManaged : public ISolver
	{
	public:
		PerceptronManaged(PerceptronTopology^ t, array<array<float>^>^ weights);

		virtual array<Single>^ Solve(array<Single>^ x) override;

		virtual ~PerceptronManaged();
	private:

		float* x;
		float* y;

		neurolib::Perceptron* psolver;
		array<array<float>^>^ weights;
	};
}
