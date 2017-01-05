#pragma once

#include "ISoftAF.h"

namespace dms::solvers::neural_nets
{
	public ref class BentIdentityAF : public ISoftAF
	{
	public:
		virtual float getResult(float x);
		virtual float getDerivative(float x);
	};
}