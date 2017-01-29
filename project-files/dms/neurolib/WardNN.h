#pragma once
#include "ActivationFunctions.h"
#include <vector>

namespace neurolib
{
	class WardNN
	{
	public:
		WardNN(const WardNN& wnn);
		/*
		neuronsCount : array [0..layersCount) of array[0..groups count in layer) - number of neurons in group
		isDelayOnGroup : array [0..layersCount-1) of array[0..groups count in layer) - does neurons in group have bias-neuron connection or not
		af_types : array [0..layersCount-1) of array[0..groups count in layer) - activation function of neurons in group
		groupsCount : array[0..layersCount-1) - number of groups in layer. First layer is omitted, it always contains one group
		additionalLayerConnection : array[0..layersCount-2) - this value indicates, that current layer has connection to layer with number (currentNumber + 1 + value)
		layersCount : from 2 to inf - number of layers in net
		weights : array [0..layersCount-1) of array [s*m + n*(m+k)], n - neurons in previous layer,
		m - neurons in current layer, k(j) = 1, if j-th group has bias-neuron connection and 0 otherwise; k = sum(k(j))
		s - neurons in grand-previous layer, if this has additional connection to current
		*/
		WardNN(int** neuronsCount, bool** isDelayOnGroup, ActivationFunctionType** af_types, int* groupsCount, int* additionalLayerConnection, int layersCount, float** weights);

		/*
		x: array[0..inputs count) - allocated vector of inputs.
		y: array[0..outputs count) - allocated vector of outputs. After methods execution contains values of output neurons
		return value: outputs count
		*/
		int solve(float* x, float* y);
		int getInputsCount();
		int getOutputsCount();

		~WardNN();
	private:
		int inputsCount, layersCount, outputsCount;
		int* groupsCount;
		int* neuronsInLayer, *additionalNeurons, *additionalLayerConnection;
		int** neuronsInGroup;
		bool** hasDelay;
		float** w;
		ActivationFunctionType** af_types;

		float** temp_res;
		float* buf_x;

		void init_calc_vars();
	};
}
