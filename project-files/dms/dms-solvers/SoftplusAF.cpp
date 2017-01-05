#include "stdafx.h"
#include "SoftplusAF.h"
#include <cmath>

namespace dms::solvers::neural_nets
{
	float SoftplusAF::getResult(float x)
	{
		float exp = std::exp(x);
		return std::log(1.0f + exp);
	}

	float SoftplusAF::getDerivative(float x)
	{
		float exp = std::exp(-x);
		return 1.0f / (1.0f + exp);
	}
}
