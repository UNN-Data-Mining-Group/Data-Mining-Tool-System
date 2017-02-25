#include "Perceptron.h"
#include "ActivationFunctions.h"
#include <ctime>
#include <iostream>
#include "mkl_cblas.h"

using nnets_perceptron::Perceptron;
using nnets::ActivationFunctionType;

size_t nnets_perceptron::getAllWeightsPerc(float* &dest, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);

	size_t dest_index = 0;
	for (int i = 0; i < p->layers - 1; i++)
		for (int j = 0; j < p->w_sizes[i]; j++)
			dest[dest_index++] = p->w[i][j];
	return dest_index;
}

void nnets_perceptron::setAllWeightsPerc(const float* src, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);

	int src_index = 0;
	for (int i = 0; i < p->layers - 1; i++)
		for (int j = 0; j < p->w_sizes[i]; j++)
			p->w[i][j] = src[src_index++];
}

size_t nnets_perceptron::solvePerc(const float* x, float* y, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);
	return p->solve(x, y);
}

size_t nnets_perceptron::getWeightsCountPerc(void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);
	size_t weights_count = 0;
	for (int i = 0; i < p->layers - 1; i++)
	{
		weights_count += p->w_sizes[i];
	}

	return weights_count;
}

void* nnets_perceptron::copyPerc(void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);
	return new Perceptron(*p);
}

void nnets_perceptron::freePerc(void* &obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);
	delete p;
	obj = nullptr;
}

Perceptron::Perceptron(Perceptron& p)
{
	init(p.neurons, p.has_delay, p.aftypes, p.layers, p.w);
}

Perceptron::Perceptron(const int* neuronsCount, 
	const ActivationFunctionType* types, int layersCount, float** weights)
{
	init(neuronsCount, nullptr, types, layersCount, weights);
}

Perceptron::Perceptron(const int* neuronsCount, 
	const bool* isDelayOnLayer, const ActivationFunctionType* types, int layersCount, float** weights)
{	
	init(neuronsCount, isDelayOnLayer, types, layersCount, weights);
}

size_t Perceptron::solve(const float* x, float* y)
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
		calc_activation_function(vec, n, aftypes[i - 1], vec);
	}

	for(int i = 0; i < outputs; i++)
	{
		y[i] = temp_res[layers-1][i];
	}

	return outputs;
}

size_t Perceptron::getInputsCount()
{
	return neurons[0];
}

size_t Perceptron::getOutputsCount()
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

	delete[] w_sizes;		w_sizes = nullptr;
	delete[] aftypes;		aftypes = nullptr;
	delete[] w;				w = nullptr;
	delete[] has_delay;		has_delay = nullptr;
	delete[] neurons;		neurons = nullptr;
}

void Perceptron::check_initializers(const int* neurons, const bool* has_delay,
	const nnets::ActivationFunctionType* afs, int layers, float** weights)
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

void Perceptron::init(const int* neuronsCount, const bool* isDelayOnLayer,
	const nnets::ActivationFunctionType* types, int layersCount, float** weights)
{
	check_initializers(neuronsCount, isDelayOnLayer, types, layersCount, weights);

	layers = layersCount;
	neurons = new int[layers];
	for(int i = 0; i < layers; i++)
		this->neurons[i] = neuronsCount[i];

	has_delay = new bool[layers - 1];
	if (isDelayOnLayer != nullptr)
		for(int i = 0; i < layers - 1; i++)
			has_delay[i] = isDelayOnLayer[i];
	else
		for(int i = 0; i < layers - 1; i++)
			has_delay[i] = false;

	temp_res = new float*[layers];
	for(int i = 0; i < layers - 1; i++)
	{
		temp_res[i] = new float[neurons[i] + this->has_delay[i]];

		if (has_delay[i])
			temp_res[i][neurons[i]] = -1.0f;
	}
	temp_res[layers - 1] = new float[neurons[layers - 1]];

	aftypes = new ActivationFunctionType[layers - 1];
	w = new float*[layers - 1];
	w_sizes = new size_t[layers - 1];
	for(int i = 0; i < layers - 1; i++)
	{
		aftypes[i] = types[i];
		w_sizes[i] = (static_cast<size_t>(neurons[i]) + has_delay[i]) * neurons[i+1];

		w[i] = new float[w_sizes[i]];
		for(int j = 0; j < w_sizes[i]; j++)
			w[i][j] = weights[i][j];
	}
}
