#include "ISolver.h"

namespace dms::solvers
{
	ISolver::ISolver(__int64 inputs, __int64 outputs)
	{
		inputsCount = inputs;
		outputsCount = outputs;
	}

	__int64 ISolver::GetInputsCount()
	{
		return inputsCount;
	}

	__int64 ISolver::GetOutputsCount()
	{
		return outputsCount;
	}

	std::vector<std::string> ISolver::getAttributes()
	{
		return std::vector<std::string>();
	}

	std::vector<LearningOperation> ISolver::getOperations()
	{
		return std::vector<LearningOperation>();
	}

	ISolver::~ISolver()
	{
	}
}