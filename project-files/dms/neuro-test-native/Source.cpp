#include "Perceptron.h"
#include "WardNN.h"
#include "ConvNN.h"
#include <ctime>
#include <random>
#include <iostream>
#include <cmath>

using namespace neurolib;

void performance_test();
void accuracy_test_perc();
void accuracy_test_ward();
void accuracy_test_conv();

void main()
{
	accuracy_test_perc();
	accuracy_test_ward();
	accuracy_test_conv();
	performance_test();
}

float** generate_w(int* neurons, bool* has_delay, int layers)
{
	std::srand(static_cast<unsigned int>(std::time(0)));
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

void accuracy_test_perc()
{
	std::cout << "Accuracy test perc:";
	float* w[2];
	w[0] = new float[3 * 4];
	w[0][0] = 1.f / 2;		w[0][1] = 1.f / 3;		w[0][2] = -1.f / 5;
	w[0][3] = 1;			w[0][4] = 1.f / 2;		w[0][5] = -1.f / 4;
	w[0][6] = 1.f / 2;		w[0][7] = -1.f;			w[0][8] = 1.f / 2;
	w[0][9] = -1;			w[0][10] = 1.f / 4;		w[0][11] = 1.f / 3;

	w[1] = new float[2 * 4];
	w[1][0] = 1.f / 2;		w[1][1] = -1.f / 3;		w[1][2] = 1.f / 4;		w[1][3] = -1;
	w[1][4] = 1;			w[1][5] = -1.f / 2;		w[1][6] = -1.f / 3;		w[1][7] = 1.f / 4;

	int neurons[] = { 3,4,2 };
	ActivationFunctionType af[] = { ActivationFunctionType::BinaryStep, ActivationFunctionType::Identity };

	Perceptron ps(neurons, af, 3, w);
	float x[3] = { 1, 0, 1 };
	float y[2];
	float answer[2] = { 5.f / 12, 1.f / 6 };

	ps.solve(x, y);

	float EPS = 1e-5;
	if ((std::abs(y[0] - answer[0]) > EPS) || (std::abs(y[1] - answer[1]) > EPS))
	{
		std::cout << "FAIL" << std::endl;
		std::cout << "answer = (" << answer[0] << "," << answer[1] << ")" << std::endl;
		std::cout << "y = (" << y[0] << "," << y[1] << ")" << std::endl;
	}
	else
	{
		std::cout << "PASS" << std::endl;
	}

	delete[] w[0];
	delete[] w[1];
}

void accuracy_test_ward()
{
	std::cout << "Accuracy test ward:";
	int* neurons[4];
	int g1[] = { 2 };		neurons[0] = g1;
	int g2[] = { 2,1 };		neurons[1] = g2;
	int g3[] = { 1,1 };		neurons[2] = g3;
	int g4[] = { 1 };		neurons[3] = g4;

	bool* delays[3];
	bool d2[] = { false, false };		delays[0] = d2;
	bool d3[] = { false, false };		delays[1] = d3;
	bool d4[] = { false };				delays[2] = d4;
	ActivationFunctionType* afs[3];
	ActivationFunctionType a2[] = { ActivationFunctionType::BinaryStep, ActivationFunctionType::Identity };		afs[0] = a2;
	ActivationFunctionType a3[] = { ActivationFunctionType::Identity, ActivationFunctionType::BinaryStep };		afs[1] = a3;
	ActivationFunctionType a4[] = { ActivationFunctionType::Identity };	afs[2] = a4;

	int groups[] = { 2,2,1 };
	int conns[] = { 1, 0 };

	float* w[3];
	float l1[] = 
	{ 
		1.0f, 0.5f, 
		2.0f, 0.25f, 
		0.5f, 1.0f 
	};	
	w[0] = l1;

	float l2[] =
	{
		1.0f, 2.0f, 2.0f, 1.0f, 2.0f,
		2.0f, 1.0f, 1.0f, 1.0f, 2.0f
	};
	w[1] = l2;

	float l3[] =
	{
		1.0f, 1.0f
	};
	w[2] = l3;

	WardNN wnn(neurons, delays,afs, groups, conns, 4, w);
	float x[] = { 1, 0 };
	float y[] = { 0 };
	wnn.solve(x, y);

	if (std::abs(y[0] - 6.0f) < 1e-6)
		std::cout << "PASS";
	else
		std::cout << "FAIL";
	std::cout << std::endl;
}

void accuracy_test_conv()
{
	std::cout << "Accuracy test conv:";
	float w1[] = 
	{ 
		-1, 0, 0, 1, 
		0, 1, -1, 0
	};
	float w2[] =
	{
		1, 0, 0, -1,
		-0.5, 0, 1, 0,

		0, -1, 1, 0,
		0, 0, -0.5, 0
	};
	float* w[] = { w1, w2 };
	std::vector<ConvNNLayer*> layers;
	layers.push_back(new ConvNNConvolutionLayer{ 2, 2, 1, 1, 1, 2, ActivationFunctionType::Identity });
	layers.push_back(new ConvNNPoolingLayer{ 2, 2, 1, 1 });
	layers.push_back(new ConvNNFullyConnectedLayer{ 2, ActivationFunctionType::Identity });

	ConvNN net{ 2, 2, 1, layers, w };

	float x[] = { 0.5, 0.3, -0.6, -0.2 };
	float y[2];
	float answer[] = { 0.35, -0.15 };
	net.solve(x, y);

	float EPS = 1e-5;
	if ((std::abs(y[0] - answer[0]) > EPS) || (std::abs(y[1] - answer[1]) > EPS))
	{
		std::cout << "FAIL" << std::endl;
		std::cout << "answer = (" << answer[0] << "," << answer[1] << ")" << std::endl;
		std::cout << "y = (" << y[0] << "," << y[1] << ")" << std::endl;
	}
	else
	{
		std::cout << "PASS" << std::endl;
	}

	for (int i = 0; i < layers.size(); i++)
		delete layers[i];
}

void performance_test()
{
	std::cout << "Performance test:" << std::endl;
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