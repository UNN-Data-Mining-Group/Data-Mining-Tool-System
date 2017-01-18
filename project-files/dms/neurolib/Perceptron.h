#pragma once
#include "ActivationFunctions.h"

namespace neurolib
{
	class Perceptron
	{
	public:
		Perceptron(Perceptron& p);
		Perceptron(int* neuronsCount, ActivationFunctionType* types, int layersCount, float** weights);
		Perceptron(int* neuronsCount, bool* isDelayOnLayer, ActivationFunctionType* types, int layersCount, float** weights);

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
	};
}