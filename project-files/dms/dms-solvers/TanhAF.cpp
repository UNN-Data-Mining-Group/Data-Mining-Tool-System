#include "TanhAF.h"
#include <cmath>

namespace dms::solvers::neural_nets
{
	float TanhAF::getResult(float x)
	{
		float exp2 = std::exp(-2.0f*x);
		return 2.0f / (1.0f + exp2) - 1.0f;
	}

	float TanhAF::getDerivative(float x)
	{
		return 1.0f - std::pow(getResult(x), 2);
	}
}
