#pragma once
#include "NeuralNetwork.h"

namespace nnets_perceptron
{
	size_t getAllWeightsPerc(float* &dest, void* obj);
	void setAllWeightsPerc(const float* src, void* obj);
	size_t solvePerc(float* x, float* y, void* obj);
	size_t getWeightsCountPerc(void* obj);
	void* copyPerc(void* obj);
	void freePerc(void* &obj);

	class Perceptron : public nnets::NeuralNetwork
	{
	public:
		Perceptron(Perceptron& p);
		Perceptron(const int* neuronsCount, 
			const nnets::ActivationFunctionType* types, int layersCount, float** weights);
		/*
		neuronsCount : array [0..layersCount) - number of neurons in layer
		isDelayOnLayer : array [0..layersCount-1) - does neurons in layer have bias-neuron connection or not
		types : array [0..layersCount-1) - activation function of neurons in layer
		layersCount : from 2 to inf - number of layers in net
		weights : array [0..layersCount-1) of array [n*(m+k)], n - neurons in previous layer, 
			m - neurons in current layer, k - 1, if current layer has bias-neuron connection and 0 otherwise
		*/
		Perceptron(const int* neuronsCount, const bool* isDelayOnLayer, 
			const nnets::ActivationFunctionType* types, int layersCount, float** weights);

		/*
		x: array[0..inputs count) - allocated vector of inputs.
		y: array[0..outputs count) - allocated vector of outputs. After methods execution contains values of output neurons 
		return value: outputs count
		*/
		size_t solve(const float* x, float* y) override;
		size_t getInputsCount() override;
		size_t getOutputsCount() override;

		~Perceptron();
	private:
		void check_initializers(const int* neurons, const bool* has_delay, 
			const nnets::ActivationFunctionType* afs, int layers, float** weights);
		void init(const int* neuronsCount, const bool* isDelayOnLayer, 
			const nnets::ActivationFunctionType* types, int layersCount, float** weights);

		int layers;
		int* neurons;
		bool* has_delay;
		nnets::ActivationFunctionType* aftypes;
		size_t* w_sizes;
		float** w;
		float** temp_res;

		friend size_t getAllWeightsPerc(float* &dest, void* obj);
		friend void setAllWeightsPerc(const float* src, void* obj);
		friend size_t solvePerc(float* x, float* y, void* obj);
		friend size_t getWeightsCountPerc(void* obj);
	};
}