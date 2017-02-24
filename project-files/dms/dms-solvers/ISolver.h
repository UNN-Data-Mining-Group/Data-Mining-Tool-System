#pragma once
#include <vector>
using namespace System;

namespace dms::solvers
{
	public struct LearningOperation
	{
		std::string name;
		void* function;

		LearningOperation(std::string name, void* function) :
			name(name), 
			function(function) 
		{}
	};

	public ref class ISolver abstract
	{
	public:
		ISolver(__int64 inputs, __int64 outputs);

		virtual array<Single>^ Solve(array<Single>^ x) = 0;
		virtual __int64 GetInputsCount();
		virtual __int64 GetOutputsCount();
		virtual std::vector<std::string> getAttributes();
		virtual std::vector<LearningOperation> getOperations();
		virtual void* getNativeSolver() = 0;

		virtual ~ISolver();
	private:
		__int64 inputsCount;
		__int64 outputsCount;
	};
}
