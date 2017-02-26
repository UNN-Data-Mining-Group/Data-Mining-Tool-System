#include "Perceptron.h"
#include "ActivationFunctions.h"
#include <ctime>
#include <iostream>
#include "mkl_cblas.h"

using nnets_perceptron::Perceptron;
using nnets::ActivationFunctionType;

size_t nnets_perceptron::getAllWeightsPerc(float* dest, void* obj)
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

int nnets_perceptron::getIterationsCount(void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);
	return p->layers;
}

int nnets_perceptron::getIterationSizes(size_t* sizes, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);
	for (int i = 0; i < p->layers; i++)
		sizes[i] = p->neurons[i];
	return p->layers;
}

size_t nnets_perceptron::getWeightsVectors(float** w, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);

	size_t allSize = 0;
	for (int i = 0; i < p->layers - 1; i++)
	{
		for (size_t j = 0; j < p->w_sizes[i]; j++)
		{
			w[i][j] = p->w[i][j];
			allSize++;
		}
	}

	return allSize;
}

int nnets_perceptron::getWeightsVectorsCount(void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);
	return p->layers - 1;
}

size_t nnets_perceptron::getWeightsVectorSize(int vectorIndex, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);
	if ((vectorIndex < 0) || (vectorIndex > (p->layers - 2)))
		return 0;
	return p->w_sizes[vectorIndex];
}

size_t nnets_perceptron::getIterationDerivatives(float* dest, int iterationIndex, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);

	if ((iterationIndex < 1) || (iterationIndex > (p->layers - 1)))
		return 0;

	nnets::calc_activation_derivatives(p->temp_res[2 * iterationIndex - 1],
		p->neurons[iterationIndex], p->aftypes[iterationIndex - 1], dest);
	return p->neurons[iterationIndex];
}

size_t nnets_perceptron::getIterationValues(float* dest, int iterationIndex, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);

	if ((iterationIndex < 0) || (iterationIndex >(p->layers - 1)))
		return 0;

	float* af_values = p->temp_res[2 * iterationIndex];
	for (int i = 0; i < p->neurons[iterationIndex]; i++)
		dest[i] = af_values[i];

	return p->neurons[iterationIndex];
}

size_t nnets_perceptron::setWeightsVector(const float* vector, int vectorIndex, void* obj)
{
	Perceptron* p = static_cast<Perceptron*>(obj);

	if ((vectorIndex < 0) || (vectorIndex > p->layers - 2))
		return 0;

	for (size_t i = 0; i < p->w_sizes[vectorIndex]; i++)
		p->w[vectorIndex][i] = vector[i];
	return p->w_sizes[vectorIndex];
}

Perceptron::Perceptron(Perceptron& p)
{
	init(p.neurons, p.has_delay, p.aftypes, p.layers);
	setWeights(p.w);
}

Perceptron::Perceptron(const int* neuronsCount, 
	const ActivationFunctionType* types, int layersCount)
{
	init(neuronsCount, nullptr, types, layersCount);
}

Perceptron::Perceptron(const int* neuronsCount, 
	const bool* isDelayOnLayer, const ActivationFunctionType* types, int layersCount)
{	
	init(neuronsCount, isDelayOnLayer, types, layersCount);
}

size_t Perceptron::solve(const float* x, float* y)
{
	int inputs = neurons[0];
	int outputs = neurons[layers-1];

	for(int i = 0; i < inputs; i++)
		temp_res[0][i] = x[i];
	
	int temp_index = 0;
	for (int i = 1; i < layers; i++)
	{
		cblas_sgemv(CblasRowMajor, CblasNoTrans, neurons[i], neurons[i - 1] + has_delay[i - 1], 1.0f, w[i-1], neurons[i - 1] + has_delay[i - 1], temp_res[temp_index], 1, 0.0f, temp_res[temp_index + 1], 1);
		const int n = neurons[i];
		calc_activation_function(temp_res[temp_index + 1], n, aftypes[i - 1], temp_res[temp_index + 2]);
		temp_index += 2;
	}

	for (int i = 0; i < outputs; i++)
		y[i] = temp_res[2 * layers - 2][i];

	return outputs;
}

void Perceptron::setWeights(float** weights)
{
	for (int i = 0; i < layers - 1; i++)
	{
		if (weights[i] == nullptr)
			throw "weights memory is not allocated";
		for (int j = 0; j < w_sizes[i]; j++)
			w[i][j] = weights[i][j];
	}
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
	for(int i = 0; i < 2 * layers - 1; i++)
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
	const nnets::ActivationFunctionType* afs, int layers)
{
	if (layers < 2)
		throw "too little layers";

	if ((afs == nullptr) ||
		(neurons == nullptr))
		throw "memory is not allocated";

	for (int i = 0; i < layers; i++)
		if (neurons[i] <= 0)
			throw "illegal (negative) neurons count";
}

void Perceptron::init(const int* neuronsCount, const bool* isDelayOnLayer,
	const nnets::ActivationFunctionType* types, int layersCount)
{
	check_initializers(neuronsCount, isDelayOnLayer, types, layersCount);

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

	temp_res = new float*[2 * layers - 1];
	int temp_index = 0;
	for(int i = 0; i < layers - 1; i++)
	{
		temp_res[temp_index] = new float[neurons[i] + this->has_delay[i]];
		temp_res[temp_index + 1] = new float[neurons[i + 1]];

		if (has_delay[i])
			temp_res[temp_index][neurons[i]] = -1.0f;

		temp_index += 2;
	}
	temp_res[2*layers - 2] = new float[neurons[layers - 1]];

	aftypes = new ActivationFunctionType[layers - 1];
	w = new float*[layers - 1];
	w_sizes = new size_t[layers - 1];
	for(int i = 0; i < layers - 1; i++)
	{
		aftypes[i] = types[i];
		w_sizes[i] = (static_cast<size_t>(neurons[i]) + has_delay[i]) * neurons[i+1];

		w[i] = new float[w_sizes[i]];
		for(int j = 0; j < w_sizes[i]; j++)
			w[i][j] = 0.0f;
	}
}
