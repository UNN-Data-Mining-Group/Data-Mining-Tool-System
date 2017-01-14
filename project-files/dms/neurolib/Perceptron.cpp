#include "Perceptron.h"
#include <ctime>
#include <iostream>

using namespace neurolib;

Perceptron::Perceptron(Perceptron& p)
{
	init(p.neurons, p.has_delay, p.afs, p.layers, p.w);
}

Perceptron::Perceptron(int* neurons, oper_af* afs, int layers, float** weights)
{
	init(neurons, nullptr, afs, layers, weights);
}

Perceptron::Perceptron(int* neurons, bool* has_delay, oper_af* afs, int layers, float** weights)
{
	init(neurons, has_delay, afs, layers, weights);
}


int Perceptron::solve(float* x, float* y)
{
	if ((is_initialised == false) ||
		(x == nullptr) ||
		(y == nullptr))
		return 0;

	int inputs = neurons[0];
	int outputs = neurons[layers-1];

	for(int i = 0; i < inputs; i++)
	{
		temp_res[0][i] = x[i];
	}
	
	for (int i = 1; i < layers; i++)
	{
		mult(w[i-1], temp_res[i-1], temp_res[i], neurons[i-1] + has_delay[i-1], neurons[i]);
		
		for(int j = 0; j < neurons[i]; j++)
		{
			temp_res[i][j] = afs[i-1](temp_res[i][j]);
		}
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
	delete[] w;				w = nullptr;

	delete[] afs;			afs = nullptr;
	delete[] has_delay;		has_delay = nullptr;
	delete[] neurons;		neurons = nullptr;
}

bool Perceptron::can_init(int* neurons, bool* has_delay, oper_af* afs, int layers, float** weights)
{
	if (layers < 2)
		return false;

	if ((afs == nullptr) || 
		(weights == nullptr) ||
		(neurons == nullptr))
		return false;

	for(int i = 0; i < layers; i++)
		if (neurons[i] <= 0)
			return false;

	for (int i = 0; i < layers - 1; i++)
		if (afs[i] == nullptr)
			return false;

	for(int i = 0; i < layers - 1; i++)
		if (weights[i] == nullptr)
			return false;

	return true;
}

void Perceptron::init(int* neurons, bool* has_delay, oper_af* afs, int layers, float** weights)
{
	is_initialised = false;

	bool c = can_init(neurons, has_delay, afs, layers, weights);
	if (c == true)
	{
		this->layers = layers;

		this->neurons = new int[layers];
		this->has_delay = new bool[layers - 1];
		for(int i = 0; i < layers; i++)
			this->neurons[i] = neurons[i];

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

		this->afs = new oper_af[layers - 1];
		w = new float*[layers - 1];
		for(int i = 0; i < layers - 1; i++)
		{
			this->afs[i] = afs[i];

			int dim = (neurons[i] + this->has_delay[i]) * neurons[i+1];

			w[i] = new float[dim];
			for(int j = 0; j < dim; j++)
				w[i][j] = weights[i][j];
		}
		is_initialised = true;
	}
}

inline int Perceptron::mult(float* w, float* v, float* dest, int cols, int rows)
{
	for(int i = 0; i < rows; i++)
	{
		dest[i] = 0.0f;
		for(int j = 0; j < cols; j++)
		{
			dest[i] += w[i * cols + j] * v[j];
		}
	}
	return rows;
}
