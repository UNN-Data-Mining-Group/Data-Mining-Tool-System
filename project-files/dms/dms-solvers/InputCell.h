#pragma once
#include "ICell.h"

namespace dms::solvers::neural_nets
{
	public ref class InputCell : public ICell
	{
	public:
		InputCell(float input);
		void setInput(float input);
		virtual float getOutput() override;
	private:
		float input;
	};
}
