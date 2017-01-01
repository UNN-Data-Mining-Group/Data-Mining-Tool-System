#include "stdafx.h"
#include "ISolver.h"

namespace dms::solvers
{
	ISolver::ISolver(Int32 inputs, Int32 outputs)
	{
		inputsCount = inputs;
		outputsCount = outputs;
	}

	int ISolver::getInputsCount()
	{
		return inputsCount;
	}

	int ISolver::getOutputsCount()
	{
		return outputsCount;
	}

	std::vector<std::string>* ISolver::getAttributes()
	{
		return new std::vector<std::string>();
	}

	std::vector<LearningOperation>* ISolver::getOperations()
	{
		return new std::vector<LearningOperation>();
	}

	ISolver::~ISolver()
	{
	}
}