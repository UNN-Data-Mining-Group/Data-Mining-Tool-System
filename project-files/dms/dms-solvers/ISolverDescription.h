#pragma once

namespace dms::solvers
{
	public interface class ISolverDescription
	{
	public:
		virtual System::Int64 GetInputsCount() = 0;
		virtual System::Int64 GetOutputsCount() = 0;
	};
}