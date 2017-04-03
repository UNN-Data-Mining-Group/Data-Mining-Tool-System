#pragma once
#include "ISolverDescription.h"
#include "NeuralNetwork.h"

namespace dms::solvers
{
	public interface class ITopology : ISolverDescription
	{
	public:
		virtual nnets::NeuralNetwork* createNativeSolver() = 0;
	};
}
