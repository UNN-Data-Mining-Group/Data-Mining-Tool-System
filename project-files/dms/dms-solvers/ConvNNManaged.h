#pragma once
#include "ISolver.h"
#include "ConvNNTopology.h"
#include "ConvNN.h"

namespace dms::solvers::neural_nets
{
	public ref class ConvNNManaged : public ISolver
	{
	public:
		ConvNNManaged(ConvNNTopology^ t, array<array<float>^>^ weights);

		virtual array<float>^ Solve(array<float>^ x) override;

		virtual ~ConvNNManaged();
	private:

		float *x;
		float *y;

		neurolib::ConvNN* solver;
		array<array<float>^>^ weights;
	};
}
