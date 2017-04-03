#pragma once
#include "mkl_types.h"	//must be included before using #include <mkl.h>
#include "ActivationFunctions.h"

namespace nnets
{
	class NeuralNetwork
	{
	public:
		virtual size_t solve(const float* x, float* y) = 0;
		virtual size_t getInputsCount() = 0;
		virtual size_t getOutputsCount() = 0;
		virtual ~NeuralNetwork() {}
	};
}
