#pragma once

#include "IActivateFunction.h"

namespace dms::solvers::neural_nets
{
	public ref class BinaryStepAF : public IActivateFunction
	{
	public:
		virtual float getResult(float x);
	};
}
