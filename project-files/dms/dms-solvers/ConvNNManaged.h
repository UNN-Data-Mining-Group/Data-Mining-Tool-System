#pragma once
#include "ISolver.h"
#include "ConvNNTopology.h"
#include "ConvNN.h"

namespace dms::solvers::neural_nets::conv_net
{
	public ref class ConvNNManaged : public ISolver
	{
	public:
		ConvNNManaged(ConvNNTopology^ t, array<array<float>^>^ weights);

		virtual array<float>^ Solve(array<float>^ x) override;
		virtual std::vector<std::string> getAttributes() override;
		virtual std::map<std::string, void*> getOperations() override;
		virtual void* getNativeSolver() override;

		virtual ~ConvNNManaged();
	private:

		float *x;
		float *y;

		nnets_conv::ConvNN* solver;
		array<array<float>^>^ weights;
	};
}
