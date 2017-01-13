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
		ISolver(Int32 inputs, Int32 outputs);

		virtual array<Single>^ Solve(array<Single>^ x) = 0;
		virtual Int32 GetInputsCount();
		virtual Int32 GetOutputsCount();
		virtual std::vector<std::string> getAttributes();
		virtual std::vector<LearningOperation> getOperations();

		virtual ~ISolver();
	private:
		int inputsCount;
		int outputsCount;
	};
}
