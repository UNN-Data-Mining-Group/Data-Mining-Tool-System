#include "Stdafx.h"
#include "INeuron.h"

namespace dms::solvers::neural_nets
{
	void INeuron::setWeigthsSource(float* src, int wcount)
	{
		weights_src = src;
		weights_src_size = wcount;
	}

	void INeuron::setWeights(array<float>^ w)
	{
		if ((weights_src == nullptr) || (weights_src_size != w->Length))
			return;

		for (int i = 0; i < w->Length; i++)
			weights_src[i] = w[i];
	}

	array<float>^ INeuron::getWeights()
	{
		array<float>^ res = gcnew array<float>(weights_src_size);
		for (int i = 0; i < weights_src_size; i++)
			res[i] = weights_src[i];
		return res;
	}

	void INeuron::setInitialState()
	{
		weights_src = nullptr;
		weights_src_size = 0;
	}

	int INeuron::getWeightsPointer(float* &dest)
	{
		dest = weights_src;
		return weights_src_size;
	}
}