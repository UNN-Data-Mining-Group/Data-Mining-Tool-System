#pragma once

namespace dms::solvers::neural_nets
{
	public interface class ICell
	{
	public:
		virtual float getOutput() = 0;
	};
}
