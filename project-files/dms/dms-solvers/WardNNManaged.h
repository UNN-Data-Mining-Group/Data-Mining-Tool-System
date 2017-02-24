#pragma once
#include "ISolver.h"
#include "WardNNTopology.h"
#include "WardNN.h"

namespace dms::solvers::neural_nets::ward_net
{
	public ref class WardNNManaged : public ISolver
	{
	public:
		WardNNManaged(WardNNTopology^ t, array<array<float>^>^ weights);
		virtual array<Single>^ Solve(array<Single>^ x) override;
		virtual void* getNativeSolver() override;
		virtual ~WardNNManaged();

	private:
		float *x, *y;

		nnets_ward::WardNN* wsolver;
		array<array<float>^>^ weights;
	};
}