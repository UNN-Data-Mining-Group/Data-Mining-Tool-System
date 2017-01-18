#include "Perceptron.h"
#include <ctime>
#include <random>
#include <iostream>
#include <mkl_cblas.h>

float** generate_w(int* neurons, bool* has_delay, int layers)
{
	std::srand(static_cast<unsigned int>(/*std::time(0)*/27));
	float** w = new float*[layers - 1];
	for (int i = 0; i < layers - 1; i++)
	{
		int dim = (neurons[i] + has_delay[i]) * neurons[i + 1];

		w[i] = new float[dim];
		for (int j = 0; j < dim; j++)
		{
			w[i][j] = static_cast<float>(std::rand()) / RAND_MAX;
		}
	}
	return w;
}

using namespace neurolib;

void main()
{
	int neurons[] = { 5, 55, 70, 100, 60, 15 , 2 };
	bool has_delay[] = { 1,1,1,1,1,0 };
	int layers = 7;
	ActivationFunctionType t = ActivationFunctionType::Logistic;
	ActivationFunctionType af[] =
	{
		t,t,t,t,t,t
	};
	float** w = generate_w(neurons, has_delay, layers);

	int start = std::clock();

	float x[] = { 0.2f, 2.6f };
	float y[] = { 0, 0 };

	int N = 1000000;
	Perceptron* ps = new Perceptron(neurons, has_delay, af, layers, w);

	int finish = std::clock();
	std::cout << "creation time: " << finish - start << std::endl;

	start = std::clock();

	for (int i = 0; i < N; i++)
	{
		ps->solve(x, y);
	}
	finish = std::clock();

	std::cout << "solve time: " << finish - start << std::endl;

	delete ps;

	for (int i = 0; i < layers - 1; i++)
	{
		delete[] w[i];
	}
	delete[] w;
}