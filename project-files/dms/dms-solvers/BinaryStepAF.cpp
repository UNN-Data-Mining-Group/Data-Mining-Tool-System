#include "Stdafx.h"
#include "BinaryStepAF.h"

namespace dms::solvers::neural_nets
{
	float BinaryStepAF::getResult(float x)
	{
		return x >= 0.0f ? 1.0f : 0.0f;
	}
}