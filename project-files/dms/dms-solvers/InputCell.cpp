#include "Stdafx.h"
#include "InputCell.h"

namespace dms::solvers::neural_nets
{
	InputCell::InputCell(float input)
	{
		setInput(input);
	}

	void InputCell::setInput(float input)
	{
		this->input = input;
	}

	float InputCell::getOutput()
	{
		return input;
	}
}