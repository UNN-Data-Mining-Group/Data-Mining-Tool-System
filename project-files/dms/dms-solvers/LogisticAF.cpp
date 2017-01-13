#include "LogisticAF.h"
#include <cmath>

namespace dms::solvers::neural_nets
{
	float LogisticAF::getResult(float x)
	{
		float exp = std::exp(-x);
		return 1.0f / (1.0f + exp);
	}

	float LogisticAF::getDerivative(float x)
	{
		float f = getResult(x);
		return f * (1.0f - f);
	}
}