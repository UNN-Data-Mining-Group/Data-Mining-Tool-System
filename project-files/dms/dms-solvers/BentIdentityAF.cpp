#include "BentIdentityAF.h"
#include <cmath>

namespace dms::solvers::neural_nets
{
	float BentIdentityAF::getResult(float x)
	{
		float sqr = std::sqrt(x*x + 1.0f);
		return (sqr - 1.0f) / 2.0f + x;
	}

	float BentIdentityAF::getDerivative(float x)
	{
		float sqr = std::sqrt(x*x + 1.0f);
		return x / (2.0f * sqr) + 1.0f;
	}
}