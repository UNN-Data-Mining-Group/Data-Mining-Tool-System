#include "Stdafx.h"
#include "RegularNeuron.h"

namespace dms::solvers::neural_nets
{

	float RegularNeuron::getResult(array<float>^ x)
	{
		float* w = nullptr;
		int w_size = getWeightsPointer(w);

		if ((w == nullptr) || (w_size != x->Length))
			throw gcnew System::ArgumentException("Несовпадение размерности x и w");

		w_sum = 0;
		for (int i = 0; i < x->Length; i++)
		{
			w_sum += x[i] * w[i];
		}

		return af->getResult(w_sum);
	}

	float RegularNeuron::getWeightedSum()
	{
		return w_sum;
	}
}