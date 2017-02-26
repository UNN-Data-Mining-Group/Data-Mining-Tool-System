#pragma once
#include "NeuralNetwork.h"

namespace nnets_perceptron
{
	//Functions for genetic algorithm
	size_t getAllWeightsPerc(float* dest, void* obj);
	void setAllWeightsPerc(const float* src, void* obj);
	size_t getWeightsCountPerc(void* obj);
	void* copyPerc(void* obj);
	void freePerc(void* &obj);

	//Functions for backpropagation
	//Iterations means layers taking part in backprop
	//For perceptron it is all layers (for convnet - all except pooling layers)
	int getIterationsCount(void* obj);
	int getIterationSizes(size_t* sizes, void* obj);
	size_t getWeightsVectors(float** w, void* obj);
	int getWeightsVectorsCount(void* obj);
	size_t getWeightsVectorSize(int vectorIndex, void* obj);
	size_t getIterationDerivatives(float* dest, int iterationIndex, void* obj);
	size_t getIterationValues(float* dest, int iterationIndex, void* obj);
	size_t setWeightsVector(const float* vector, int vectorIndex, void* obj);

	//Common used functions
	size_t solvePerc(const float* x, float* y, void* obj);


	class Perceptron : public nnets::NeuralNetwork
	{
	public:
		Perceptron(Perceptron& p);
		Perceptron(const int* neuronsCount, 
			const nnets::ActivationFunctionType* types, int layersCount);
		/*
		neuronsCount : array [0..layersCount) - number of neurons in layer
		isDelayOnLayer : array [0..layersCount-1) - does neurons in layer have bias-neuron connection or not
		types : array [0..layersCount-1) - activation function of neurons in layer
		layersCount : from 2 to inf - number of layers in net
		*/
		Perceptron(const int* neuronsCount, const bool* isDelayOnLayer, 
			const nnets::ActivationFunctionType* types, int layersCount);

		/*
		x: array[0..inputs count) - allocated vector of inputs.
		y: array[0..outputs count) - allocated vector of outputs. After methods execution contains values of output neurons 
		return value: outputs count
		*/
		size_t solve(const float* x, float* y) override;
		void setWeights(float** weights);
		size_t getInputsCount() override;
		size_t getOutputsCount() override;

		~Perceptron();
	private:
		void check_initializers(const int* neurons, const bool* has_delay, 
			const nnets::ActivationFunctionType* afs, int layers);
		void init(const int* neuronsCount, const bool* isDelayOnLayer, 
			const nnets::ActivationFunctionType* types, int layersCount);

		int layers;
		int* neurons;
		bool* has_delay;
		nnets::ActivationFunctionType* aftypes;
		size_t* w_sizes;
		float** w;
		float** temp_res;

		friend size_t getAllWeightsPerc(float* dest, void* obj);
		friend void setAllWeightsPerc(const float* src, void* obj);
		friend size_t getWeightsCountPerc(void* obj);

		friend int getIterationsCount(void* obj);
		friend int getIterationSizes(size_t* sizes, void* obj);
		friend size_t getWeightsVectors(float** w, void* obj);
		friend int getWeightsVectorsCount(void* obj);
		friend size_t getWeightsVectorSize(int vectorIndex, void* obj);
		friend size_t getIterationDerivatives(float* dest, int iterationIndex, void* obj);
		friend size_t getIterationValues(float* dest, int iterationIndex, void* obj);
		friend size_t setWeightsVector(const float* vector, int vectorIndex, void* obj);
	};
}