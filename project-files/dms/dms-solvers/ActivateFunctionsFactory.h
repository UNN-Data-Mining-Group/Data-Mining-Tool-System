#pragma once
#include "IActivateFunction.h"

namespace dms::solvers::neural_nets
{
	public ref class ActivateFunctionsFactory
	{
	public:
		static array<System::String^>^ GetAllTypeNames();
		static IActivateFunction^ CreateActivateFunction(System::String^ name);
	};
}
