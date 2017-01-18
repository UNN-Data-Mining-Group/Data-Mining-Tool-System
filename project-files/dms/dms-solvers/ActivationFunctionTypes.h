#pragma once
#include "ActivationFunctions.h"

namespace dms::solvers::neural_nets
{
	public ref class ActivationFunctionTypes
	{
	public:
		static const array<System::String^>^ TypeNames = 
		{
			"Bent Identity",
			"Binary Step",
			"Identity",
			"Logistic",
			"Softplus",
			"Tanh"
		};

		static neurolib::ActivationFunctionType getType(System::String^ typeName);
	};
}
