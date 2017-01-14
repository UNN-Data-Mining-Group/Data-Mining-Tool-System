#pragma once
#include "ISolver.h"
#include "PerceptronTopology.h"
#include "Perceptron.h"

namespace dms::solvers::neural_nets
{
	public ref class PerceptronManaged : public ISolver
	{
	public:
		PerceptronManaged(PerceptronTopology^ t);

		virtual array<Single>^ Solve(array<Single>^ x) override;

		virtual ~PerceptronManaged();
	private:
		delegate float oper_af(float);

		float* x;
		float* y;

		neurolib::Perceptron* psolver;
		array<array<float>^>^ weights;
		array<oper_af^>^ activateFunctions;
	};
}
