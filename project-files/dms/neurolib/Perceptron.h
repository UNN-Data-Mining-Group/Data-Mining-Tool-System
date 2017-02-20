#pragma once
#include "ActivationFunctions.h"

namespace neurolib
{
	struct NeuronsLayer
	{
		int NeuronsCount;
		ActivationFunctionType AfType;
	};

	int getAllWeightsPerc(float* &dest, void* obj);
	void setAllWeightsPerc(const float* src, void* obj);
	int solvePerc(float* x, float* y, void* obj);
	int getWeightsCountPerc(void* obj);
	void* copyPerc(void* obj);
	void freePerc(void* obj);

	class Perceptron
	{
	public:
		Perceptron(Perceptron& p);
		Perceptron(int* neuronsCount, ActivationFunctionType* types, int layersCount, float** weights);
		/*
		neuronsCount : array [0..layersCount) - number of neurons in layer
		isDelayOnLayer : array [0..layersCount-1) - does neurons in layer have bias-neuron connection or not
		types : array [0..layersCount-1) - activation function of neurons in layer
		layersCount : from 2 to inf - number of layers in net
		weights : array [0..layersCount-1) of array [n*(m+k)], n - neurons in previous layer, 
			m - neurons in current layer, k - 1, if current layer has bias-neuron connection and 0 otherwise
		*/
		Perceptron(int* neuronsCount, bool* isDelayOnLayer, ActivationFunctionType* types, int layersCount, float** weights);

		/*
		x: array[0..inputs count) - allocated vector of inputs.
		y: array[0..outputs count) - allocated vector of outputs. After methods execution contains values of output neurons 
		return value: outputs count
		*/
		int solve(float* x, float* y);
		int getInputsCount();
		int getOutputsCount();

		~Perceptron();
	private:
		void check_initializers(int* neurons, bool* has_delay, ActivationFunctionType* afs, int layers, float** weights);
		void init(int* neuronsCount, bool* isDelayOnLayer, ActivationFunctionType* types, int layersCount, float** weights);

		int layers;
		int* neurons;
		bool* has_delay;
		ActivationFunctionType* aftypes;
		float** w;
		float** temp_res;

		friend int getAllWeightsPerc(float* &dest, void* obj);
		friend void setAllWeightsPerc(const float* src, void* obj);
		friend int solvePerc(float* x, float* y, void* obj);
		friend int getWeightsCountPerc(void* obj);
		friend void* copyPerc(void* obj);
		friend void freePerc(void* obj);
	};
}