#pragma once
#include <vector>
#include <map>
using namespace System;

namespace dms::solvers
{
	[SerializableAttribute]
	public ref class ISolver abstract
	{
	public:
		ISolver(__int64 inputs, __int64 outputs);

		virtual array<Single>^ Solve(array<Single>^ x) = 0;
		virtual __int64 GetInputsCount();
		virtual __int64 GetOutputsCount();

		virtual void* /* exactly std::vector<std::string>* */  getAttributes();
		virtual void* /* exactly std::map<std::string, void*>* */ getOperations();
		virtual void* getNativeSolver() = 0;
		virtual std::vector<std::string>* getH() { return nullptr; }

		virtual ~ISolver();
	private:
		__int64 inputsCount;
		__int64 outputsCount;
	protected:
		std::vector<std::string>* _attr;
		std::map<std::string, void*>* _opers;
	};
}
