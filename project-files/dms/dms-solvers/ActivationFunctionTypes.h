#pragma once
#include "ActivationFunctions.h"

namespace dms::solvers::neural_nets
{
	public ref class ActivationFunctionTypes
	{
	public:
		static array<System::String^>^ TypeNames = 
		{
			"Bent Identity",
			"Binary Step",
			"Identity",
			"Logistic",
			"Softplus",
			"Tanh"
		};

		static nnets::ActivationFunctionType getType(System::String^ typeName);
	};
}
