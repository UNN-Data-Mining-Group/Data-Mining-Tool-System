#pragma once

namespace dms::solvers::neural_nets
{
	public ref class ICell
	{
	public:
		virtual float getOutput() = 0;
		virtual ~ICell() {}
	};
}
