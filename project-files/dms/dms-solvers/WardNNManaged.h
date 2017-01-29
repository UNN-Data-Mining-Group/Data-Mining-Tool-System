#pragma once
#include "ISolver.h"
#include "WardNNTopology.h"
#include "WardNN.h"

namespace dms::solvers::neural_nets
{
	public ref class WardNNManaged : public ISolver
	{
	public:
		WardNNManaged(WardNNTopology^ t, array<array<float>^>^ weights);
		virtual array<Single>^ Solve(array<Single>^ x) override;
		virtual ~WardNNManaged();

	private:
		float *x, *y;

		neurolib::WardNN* wsolver;
		array<array<float>^>^ weights;
	};
}