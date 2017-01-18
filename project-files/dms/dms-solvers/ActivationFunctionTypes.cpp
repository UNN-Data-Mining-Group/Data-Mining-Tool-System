#include "ActivationFunctionTypes.h"

namespace dms::solvers::neural_nets
{
	neurolib::ActivationFunctionType ActivationFunctionTypes::getType(System::String^ typeName)
	{
		if (typeName == "Bent Identity")
			return neurolib::ActivationFunctionType::BentIdentity;
		else if (typeName == "Binary Step")
			return neurolib::ActivationFunctionType::BinaryStep;
		else if (typeName == "Identity")
			return neurolib::ActivationFunctionType::Identity;
		else if (typeName == "Logistic")
			return neurolib::ActivationFunctionType::Logistic;
		else if (typeName == "Softplus")
			return neurolib::ActivationFunctionType::Softplus;
		else if (typeName == "Tanh")
			return neurolib::ActivationFunctionType::Tanh;
		throw gcnew System::ArgumentException();
	}
}