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

	public ref class ISolver
	{
	public:
		ISolver(Int32 inputs, Int32 outputs);

		virtual array<Single>^ solve(array<Single>^ x) = 0;
		virtual Int32 getInputsCount();
		virtual Int32 getOutputsCount();
		virtual std::vector<std::string>* getAttributes();
		virtual std::vector<LearningOperation>* getOperations();

		virtual ~ISolver();
	private:
		int inputsCount;
		int outputsCount;
	};
}
