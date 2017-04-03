#include "ActivationFunctionTypes.h"

namespace dms::solvers::neural_nets
{
	nnets::ActivationFunctionType ActivationFunctionTypes::getType(System::String^ typeName)
	{
		if (typeName == "Bent Identity")
			return nnets::ActivationFunctionType::BentIdentity;
		else if (typeName == "Binary Step")
			return nnets::ActivationFunctionType::BinaryStep;
		else if (typeName == "Identity")
			return nnets::ActivationFunctionType::Identity;
		else if (typeName == "Logistic")
			return nnets::ActivationFunctionType::Logistic;
		else if (typeName == "Softplus")
			return nnets::ActivationFunctionType::Softplus;
		else if (typeName == "Tanh")
			return nnets::ActivationFunctionType::Tanh;
		throw gcnew System::ArgumentException();
	}

	bool ActivationFunctionTypes::hasSmoothDerivative(System::String^ typeName)
	{
		return nnets::has_derivative(getType(typeName));
	}
}