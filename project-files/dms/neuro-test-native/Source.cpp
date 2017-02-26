#include "Perceptron.h"
#include "WardNN.h"
#include "ConvNN.h"
#include <ctime>
#include <random>
#include <iostream>
#include <cmath>

using namespace nnets;

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



	nnets_perceptron::Perceptron ps(neurons, af, 3);
	ps.setWeights(w);

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
		2.0f, 1.0f, 1.0f, 1.0f, 2.0f,
		0.5f, 5.0f
	};
	w[1] = l2;

	float l3[] =
	{
		1.0f, 1.0f,
		2.5f
	};
	w[2] = l3;

	nnets_ward::InputLayer input{ 2, 1 };
	std::vector<nnets_ward::Layer> layers
	{
		nnets_ward::Layer
		{	
			0, 
			std::vector<nnets_ward::NeuronsGroup>
			{
				nnets_ward::NeuronsGroup{2, false, ActivationFunctionType::BinaryStep },
				nnets_ward::NeuronsGroup{1, false, ActivationFunctionType::Identity }
			} 
		},
		nnets_ward::Layer
		{
			0,
			std::vector<nnets_ward::NeuronsGroup>
			{
				nnets_ward::NeuronsGroup{ 1, true, ActivationFunctionType::Identity },
				nnets_ward::NeuronsGroup{ 1, true, ActivationFunctionType::BinaryStep }
			}
		},
		nnets_ward::Layer
		{
			0,
			std::vector<nnets_ward::NeuronsGroup>
			{
				nnets_ward::NeuronsGroup{ 1, true, ActivationFunctionType::Identity },
			}
		}

	};

	nnets_ward::WardNN wnn(input, layers);
	wnn.setWeights(w);

	float x[] = { 1, 0 };
	float y[] = { 0 };
	wnn.solve(x, y);

	if (std::abs(y[0] - 2.0f) < 1e-6)
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
	std::vector<nnets_conv::Layer*> layers;
	layers.push_back(new nnets_conv::ConvolutionLayer{ 2, 2, 1, 1, 1, 2, ActivationFunctionType::Identity });
	layers.push_back(new nnets_conv::PoolingLayer{ 2, 2, 1, 1 });
	layers.push_back(new nnets_conv::FullyConnectedLayer{ 2, ActivationFunctionType::Identity });

	nnets_conv::ConvNN net{ 2, 2, 1, layers };
	net.setWeights(w);

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
	nnets_perceptron::Perceptron* ps = new nnets_perceptron::Perceptron(neurons, has_delay, af, layers);
	ps->setWeights(w);

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