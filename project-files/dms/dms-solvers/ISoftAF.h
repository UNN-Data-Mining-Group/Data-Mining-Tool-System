#pragma once

#include "IActivateFunction.h"

namespace dms::solvers::neural_nets
{
	public interface class ISoftAF : public IActivateFunction
	{
	public:
		virtual float getDerivative(float x) = 0;
	};
}
