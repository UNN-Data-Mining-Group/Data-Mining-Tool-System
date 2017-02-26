#pragma once
#include "ISolver.h"

namespace dms::solvers::neural_nets
{
	[SerializableAttribute]
	public ref class INeuralNetwork abstract : public ISolver
	{
	public:
		INeuralNetwork(__int64 inputs, __int64 outputs) : ISolver(inputs, outputs) {}

		//copy native weights to managed for serializing and saving in database
		virtual void FetchNativeParameters() = 0;

		//copy managed weights to native to start working with net
		virtual void PushNativeParameters() = 0;
	};
}
