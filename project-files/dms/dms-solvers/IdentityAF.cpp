#include "Stdafx.h"
#include "IdentityAF.h"

namespace dms::solvers::neural_nets
{
	float IdentityAF::getResult(float x)
	{
		return x;
	}
	float IdentityAF::getDerivative(float x)
	{
		return 1.0f;
	}
}