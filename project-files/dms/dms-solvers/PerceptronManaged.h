#pragma once
#include "ISolver.h"
#include "PerceptronTopology.h"
#include "../nn-native/Perceptron.h"

namespace dms::solvers::neural_nets
{
	public ref class PerceptronManaged : public ISolver
	{
	public:
		PerceptronManaged(PerceptronTopology^ t);

		virtual array<Single>^ Solve(array<Single>^ x) override;

		virtual ~PerceptronManaged();
	private:
		float* x;
		float* y;

		neurolib::Perceptron* psolver;
		array<array<float>^>^ weights;

		delegate float oper_af(float);
	};
}
