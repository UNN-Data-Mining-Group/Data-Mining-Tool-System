#include "Perceptron.h"
#include "WardNN.h"
#include "ConvNN.h"
#include "KohonenLearning.h"
#include "KohonenNet.h"
#include <ctime>
#include <random>
#include <iostream>
#include <cmath>
#include <fstream>

#define EPS 1e-5

using namespace nnets;

void performance_test_perc();
void performance_test_ward();
void performance_test_conv();
void performance_test_af();

int accuracy_test_perc();
int accuracy_test_ward();
int accuracy_test_conv();
int accuracy_test_koh();

void kohonen_learning();

void main()
{
	std::cout << "=== Accuracy tests ===" << std::endl;
	std::cout << "->Perceptron: " << (accuracy_test_perc() == 0 ? "Success" : "Fail") << std::endl;
	std::cout << "->Ward: " << (accuracy_test_ward() == 0 ? "Success" : "Fail") << std::endl;
	std::cout << "->Convolutional: " << (accuracy_test_conv() == 0 ? "Success" : "Fail") << std::endl;
	std::cout << "->Kohonen: " << (accuracy_test_koh() == 0 ? "Success" : "Fail") << std::endl;

	std::cout << std::endl << "=== Performance tests ===" << std::endl;
	/*performance_test_perc();
	performance_test_ward();
	performance_test_conv();
	performance_test_af();*/
	kohonen_learning();
}

int accuracy_test_koh()
{
	nnets_kohonen::KohonenNet* kn1 = new nnets_kohonen::KohonenNet(3, 1, 2, 2, EPS, 
		nnets_kohonen::KohonenNet::ClassInitializer::Statistical);
	float w[] =
	{
		1, 0, 0,
		0, 2, 0,
		0, 0, 3,
		-2, 0, 0
	};
	float** classes = new float*[4];
	for (int i = 0; i < 4; i++)
		classes[i] = new float[1];
	classes[0][0] = 0;
	classes[1][0] = 1;
	classes[2][0] = 1;
	classes[3][0] = 0;

	float y1[] = { 0 };
	kn1->setWeights(w);
	kn1->setClasses(classes);

	int res = 0;
	for (int i = 0; i < 4; i++)
	{
		kn1->solve(&w[i*3], y1);
		if (abs(y1[0] - classes[i][0]) >= EPS)
		{
			res = 1;
			break;
		}
	}

	for (int i = 0; i < 4; i++)
		delete[] classes[i];
	delete[] classes;
	delete kn1;

	return res;
}

void kohonen_learning()
{
	using namespace nnets_kohonen_learning;

	std::ifstream input_file("iris_mod.data");
	int rowsCount = 150;
	int inputParams = 4;
	float** x = new float*[rowsCount];
	float** y = new float*[rowsCount];

	char row[256];
	char buf[10];

	for (int i = 0; i < rowsCount; i++)
	{
		x[i] = new float[inputParams];
		y[i] = new float[1];
		input_file.getline(row, 256);
		int k = 0;
		for (int j = 0; j < inputParams; j++)
		{
			int l = 0;
			while (row[k] != ',')
				buf[l++] = row[k++];
			k++;
			buf[l] = '\0';
			x[i][j] = atof(buf);
		}

		int l = 0;
		while (row[k] != '\0')
			buf[l++] = row[k++];
		buf[l] = '\0';
		y[i][0] = atof(buf);
	}

	input_file.close();

	OperatorList opers;
	opers.addmultWeights = nnets_kohonen::addmultWeights;
	opers.disableNeurons = nnets_kohonen::disableNeurons;
	opers.getDistance = nnets_kohonen::getDistance;
	opers.getMaxNeuronIndex = nnets_kohonen::getMaxNeuronIndex;
	opers.getWeights = nnets_kohonen::getWeights;
	opers.getWeightsMatrixSize = nnets_kohonen::getWeightsMatrixSize;
	opers.getWinner = nnets_kohonen::getWinner;
	opers.setUseNormalization = nnets_kohonen::setUseNormalization;
	opers.setWeights = nnets_kohonen::setWeights;
	opers.setY = nnets_kohonen::setY;
	opers.solve = nnets_kohonen::solve;

	nnets_kohonen::KohonenNet* kn = new nnets_kohonen::KohonenNet(inputParams, 1, 5, 5, EPS,
		nnets_kohonen::KohonenNet::ClassInitializer::Statistical);
	KohonenSelfOrganizer *selfOrg = new KohonenSelfOrganizer(opers, 50, 27, 1.5f, 0.1f, 1e-7f, EPS, true);

	KohonenClassifier* cl = new KohonenClassifier(opers, true
		, EPS, 50, 27, 0.01f, 1.0f, true);
	Selection s(x, y, rowsCount, inputParams, 1);
	//selfOrg->selfOrganize(s, kn);
	kn->setClasses(s.y, s.rowsCount);
	cl->train(s, kn);
	delete cl;
	delete selfOrg;
	delete kn;

	for (int i = 0; i < rowsCount; i++)
	{
		delete[] x[i];
		delete[] y[i];
	}
	delete[] x; 
	delete[] y;
}

void generate_w(float* w, size_t size, int seed)
{
	std::srand(static_cast<unsigned int>(seed));
	for (size_t j = 0; j < size; j++)
		w[j] = static_cast<float>(std::rand()) / RAND_MAX;
}

int accuracy_test_perc()
{
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

	int result = 0;
	if ((std::abs(y[0] - answer[0]) >= EPS) || (std::abs(y[1] - answer[1]) >= EPS))
		result = 1;

	delete[] w[0];
	delete[] w[1];

	return result;
}

int accuracy_test_ward()
{
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

	int result = 0;
	if (std::abs(y[0] - 2.0f) >= EPS)
		result = 1;
	return result;
}

int accuracy_test_conv()
{
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

	int result = 0;
	if ((std::abs(y[0] - answer[0]) >= EPS) || (std::abs(y[1] - answer[1]) >= EPS))
		result = 1;

	for (int i = 0; i < layers.size(); i++)
		delete layers[i];
	return result;
}

void performance_test_perc()
{
	std::cout << std::endl << "Perceptron solve time test" << std::endl;

	int neurons[] = { 5, 55, 70, 100, 60, 15 , 2 };
	bool has_delay[] = { 1,1,1,1,1,0 };
	int layers = 7;
	ActivationFunctionType t = ActivationFunctionType::Logistic;
	ActivationFunctionType af[] =
	{
		t,t,t,t,t,t
	};
	float** w = new float*[layers - 1];
	for (int i = 0; i < layers - 1; i++)
	{
		int size = (neurons[i] + has_delay[i]) * neurons[i + 1];
		w[i] = new float[size];
		generate_w(w[i], size, 27 + i);
	}

	int start = std::clock();

	float x[] = { 0.2f, 2.6f };
	float y[] = { 0, 0 };

	int Nmin = 100000;
	int Nmax = 500000;
	int Nstep = 100000;
	nnets_perceptron::Perceptron ps = nnets_perceptron::Perceptron(neurons, has_delay, af, layers);
	ps.setWeights(w);

	for (int N = Nmin; N <= Nmax; N += Nstep)
	{
		int start = start = std::clock();
		for (int i = 0; i < N; i++)
			ps.solve(x, y);
		int finish = std::clock();
		std::cout << N << ":" << finish - start << "ms" << std::endl;
	}

	for (int i = 0; i < layers - 1; i++)
		delete[] w[i];
	delete[] w;
}

void performance_test_ward()
{
	std::cout << std::endl << "Ward solve time test" << std::endl;

	nnets_ward::InputLayer input{ 10, 1 };
	std::vector<nnets_ward::Layer> layers
	{
		nnets_ward::Layer
	{
		2,
		std::vector<nnets_ward::NeuronsGroup>
	{
		nnets_ward::NeuronsGroup{ 5, 0, nnets::ActivationFunctionType::Logistic },
			nnets_ward::NeuronsGroup{ 10, 0, nnets::ActivationFunctionType::Softplus }
	}
	},
		nnets_ward::Layer
	{
		0,
		std::vector<nnets_ward::NeuronsGroup>
	{
		nnets_ward::NeuronsGroup{ 10, 1, nnets::ActivationFunctionType::BinaryStep },
			nnets_ward::NeuronsGroup{ 10, 0, nnets::ActivationFunctionType::Softplus },
			nnets_ward::NeuronsGroup{ 10, 0, nnets::ActivationFunctionType::Logistic }
	}
	},
		nnets_ward::Layer
	{
		1,
		std::vector<nnets_ward::NeuronsGroup>
	{
		nnets_ward::NeuronsGroup{ 20, 0, nnets::ActivationFunctionType::BentIdentity },
			nnets_ward::NeuronsGroup{ 25, 1, nnets::ActivationFunctionType::Tanh }
	}
	},
		nnets_ward::Layer
	{
		0,
		std::vector<nnets_ward::NeuronsGroup>
	{
		nnets_ward::NeuronsGroup{ 10, 1, nnets::ActivationFunctionType::Logistic }
	}
	},
		nnets_ward::Layer
	{
		0,
		std::vector<nnets_ward::NeuronsGroup>
	{
		nnets_ward::NeuronsGroup{ 2, 1, nnets::ActivationFunctionType::BinaryStep }
	}
	}
	};
	
	nnets_ward::WardNN wnn(input, layers);
	float **w = new float*[wnn.getWeightsMatricesCount()];
	for (int i = 0; i < wnn.getWeightsMatricesCount(); i++)
	{
		w[i] = new float[wnn.getWeightsMatrixSize(i)];
		generate_w(w[i], wnn.getWeightsMatrixSize(i), 27 + i);
	}
	wnn.setWeights(w);

	float x[] = { 0.2f, 2.6f, 5.4f, -7.5f, 2.0f, 4.0f, 0.0f, -1.0f, 3.14, 0.09 };
	float y[] = { 0, 0 };

	int Nmin = 100000;
	int Nmax = 500000;
	int Nstep = 100000;

	for (int N = Nmin; N <= Nmax; N += Nstep)
	{
		int start = start = std::clock();
		for (int i = 0; i < N; i++)
			wnn.solve(x, y);
		int finish = std::clock();
		std::cout << N << ":" << finish - start << "ms" << std::endl;
	}

	for (int i = 0; i < wnn.getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

void performance_test_conv()
{
	std::cout << std::endl << "Conv solve time test" << std::endl;

	std::vector<nnets_conv::Layer*> layers
	{
		new nnets_conv::ConvolutionLayer{3, 3, 1, 1, 1, 10, ActivationFunctionType::Softplus},
		new nnets_conv::ConvolutionLayer{4, 4, 2, 2, 0, 8, ActivationFunctionType::BentIdentity },
		new nnets_conv::PoolingLayer{3, 3, 1, 1},
		new nnets_conv::ConvolutionLayer{4, 4, 1, 1, 0, 5, ActivationFunctionType::Logistic},
		new nnets_conv::PoolingLayer{4, 4, 2, 2},
		new nnets_conv::FullyConnectedLayer{50, ActivationFunctionType::Tanh},
		new nnets_conv::FullyConnectedLayer{3, ActivationFunctionType::BinaryStep}
	};
	nnets_conv::ConvNN cnn { 512, 512, 3, layers };
	float** w = new float*[cnn.getWeightsMatricesCount()];
	for (int i = 0; i < cnn.getWeightsMatricesCount(); i++)
	{
		w[i] = new float[cnn.getWeightsMatrixSize(i)];
		generate_w(w[i], cnn.getWeightsMatrixSize(i), 27 + i);
	}
	cnn.setWeights(w);

	float* x = new float[512 * 512 * 3];
	float y[3];
	generate_w(x, 512 * 512 * 3, 15);

	int N[] = { 10, 100, 200 };
	for (int j = 0; j < 3; j++)
	{
		int start = start = std::clock();
		for (int i = 0; i < N[j]; i++)
			cnn.solve(x, y);
		int finish = std::clock();

		std::cout << N[j] << ":" << finish - start << "ms" << std::endl;
#ifdef DEBUG
		std::cout << "im2col time: " << cnn.tim2col.getTickCount() << " items: " << cnn.tim2col.getCountIterations() << std::endl;
		std::cout << "sgemm time: " << cnn.tsgemm.getTickCount() << " items: " << cnn.tsgemm.getCountIterations() << std::endl;
		std::cout << "pool time: " << cnn.tpool.getTickCount() << " items: " << cnn.tpool.getCountIterations() << std::endl;
		std::cout << "act time: " << cnn.tact.getTickCount() << " items: " << cnn.tact.getCountIterations() << std::endl;
		std::cout << std::endl;

		cnn.tact.reset();
		cnn.tim2col.reset();
		cnn.tpool.reset();
		cnn.tsgemm.reset();
#endif // DEBUG
	}

	delete[] x;

	for (int i = 0; i < cnn.getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;

	for (int i = 0; i < layers.size(); i++)
		delete layers[i];
}

void for_all_afs(int repeats, float *x, float *y, int size)
{
	int startTime;
	int sumTime[] = { 0, 0, 0, 0, 0, 0 };
	for (int i = 0; i < repeats; i++)
	{
		startTime = std::clock();
		calc_activation_function(x, size, ActivationFunctionType::BentIdentity, y);
		sumTime[0] += std::clock() - startTime;

		startTime = std::clock();
		calc_activation_function(x, size, ActivationFunctionType::BinaryStep, y);
		sumTime[1] += std::clock() - startTime;

		startTime = std::clock();
		calc_activation_function(x, size, ActivationFunctionType::Identity, y);
		sumTime[2] += std::clock() - startTime;

		startTime = std::clock();
		calc_activation_function(x, size, ActivationFunctionType::Logistic, y);
		sumTime[3] += std::clock() - startTime;

		startTime = std::clock();
		calc_activation_function(x, size, ActivationFunctionType::Softplus, y);
		sumTime[4] += std::clock() - startTime;

		startTime = std::clock();
		calc_activation_function(x, size, ActivationFunctionType::Tanh, y);
		sumTime[5] += std::clock() - startTime;
	}

	std::cout << "----BentIdentity: " << sumTime[0] << std::endl;
	std::cout << "----BinaryStep: " << sumTime[1] << std::endl;
	std::cout << "----Identity: " << sumTime[2] << std::endl;
	std::cout << "----Logistic: " << sumTime[3] << std::endl;
	std::cout << "----Softplus: " << sumTime[4] << std::endl;
	std::cout << "----Tanh: " << sumTime[5] << std::endl;
}

void performance_test_af()
{
	std::cout << std::endl << "Activation functions performance test" << std::endl;

	int Nmax = 1000000;
	float* x = new float[Nmax];
	float* y = new float[Nmax];
	generate_w(x, Nmax, 27);

	std::cout << "500 repeats:" << std::endl;
	for_all_afs(500, x, y, Nmax);

	delete[] y;
	delete[] x;
}
