#include "Perceptron.h"
#include "ActivationFunctions.h"
#include <ctime>
#include <iostream>
#include <mkl_cblas.h>

using namespace neurolib;

Perceptron::Perceptron(Perceptron& p)
{
	init(p.neurons, p.has_delay, p.aftypes, p.layers, p.w);
}

Perceptron::Perceptron(int* neuronsCount, ActivationFunctionType* types, int layersCount, float** weights)
{
	init(neuronsCount, nullptr, types, layersCount, weights);
}

Perceptron::Perceptron(int* neuronsCount, bool* isDelayOnLayer, ActivationFunctionType* types, int layersCount, float** weights)
{	
	init(neuronsCount, isDelayOnLayer, types, layersCount, weights);
}

int Perceptron::solve(float* x, float* y)
{
	int inputs = neurons[0];
	int outputs = neurons[layers-1];

	for(int i = 0; i < inputs; i++)
	{
		temp_res[0][i] = x[i];
	}
	
	for (int i = 1; i < layers; i++)
	{
		cblas_sgemv(CblasRowMajor, CblasNoTrans, neurons[i], neurons[i - 1] + has_delay[i - 1], 1.0f, w[i-1], neurons[i - 1] + has_delay[i - 1], temp_res[i - 1], 1, 0.0f, temp_res[i], 1);
		const int n = neurons[i];
		float* vec = temp_res[i];
		get_activation_function_for_layer(aftypes[i - 1], vec, n);
	}

	for(int i = 0; i < outputs; i++)
	{
		y[i] = temp_res[layers-1][i];
	}

	return outputs;
}

int Perceptron::getInputsCount()
{
	return neurons[0];
}

int Perceptron::getOutputsCount()
{
	return neurons[layers - 1];
}

Perceptron::~Perceptron()
{
	for(int i = 0; i < layers; i++)
	{
		delete[] temp_res[i];
	}
	delete[] temp_res;		temp_res = nullptr;

	for(int i = 0; i < layers - 1; i++)
	{
		delete[] w[i];
	}
	delete[] aftypes;		aftypes = nullptr;
	delete[] w;				w = nullptr;
	delete[] has_delay;		has_delay = nullptr;
	delete[] neurons;		neurons = nullptr;
}

void Perceptron::check_initializers(int* neurons, bool* has_delay, ActivationFunctionType* afs, int layers, float** weights)
{
	if (layers < 2)
		throw "too little layers";

	if ((afs == nullptr) ||
		(weights == nullptr) ||
		(neurons == nullptr))
		throw "memory is not allocated";

	for (int i = 0; i < layers; i++)
		if (neurons[i] <= 0)
			throw "illegal (negative) neurons count";

	for (int i = 0; i < layers - 1; i++)
		if (weights[i] == nullptr)
			throw "weights memory is not allocated";
}

void Perceptron::init(int* neurons, bool* has_delay, ActivationFunctionType* types, int layers, float** weights)
{
	check_initializers(neurons, has_delay, types, layers, weights);

	this->layers = layers;
	this->neurons = new int[layers];
	for(int i = 0; i < layers; i++)
		this->neurons[i] = neurons[i];

	this->has_delay = new bool[layers - 1];
	if (has_delay != nullptr)
		for(int i = 0; i < layers - 1; i++)
			this->has_delay[i] = has_delay[i];
	else
		for(int i = 0; i < layers - 1; i++)
			this->has_delay[i] = false;

	temp_res = new float*[layers];
	for(int i = 0; i < layers - 1; i++)
	{
		temp_res[i] = new float[neurons[i] + this->has_delay[i]];

		if (this->has_delay[i])
			temp_res[i][neurons[i]] = -1.0f;
	}
	temp_res[layers - 1] = new float[neurons[layers - 1]];

	aftypes = new ActivationFunctionType[layers - 1];
	w = new float*[layers - 1];
	for(int i = 0; i < layers - 1; i++)
	{
		aftypes[i] = types[i];
		int dim = (neurons[i] + this->has_delay[i]) * neurons[i+1];

		w[i] = new float[dim];
		for(int j = 0; j < dim; j++)
			w[i][j] = weights[i][j];
	}
}
