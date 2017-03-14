#include "KohonenNet.h"
#include <vector>
#include <map>

using namespace nnets_kohonen;

inline bool is_equal(float* v1, float* v2, int size, float eps)
{
	for (int i = 0; i < size; i++)
		if (std::abs(v1[i] - v2[i]) > eps)
			return false;
	return true;
}

std::vector<std::vector<float>> makeSelfOrganizationDistribution(float** x, float** y, int rowsCount, int xSize, int ySize,
	KohonenNet* kn, int seed, int maxIterations, float sigma0, float l0,
	float minLearningRate, float eps)
{
	//Set random weights
	float* w = new float[kn->getWeightsMatrixSize()];
	std::srand(seed);
	for (int i = 0; i < kn->getWeightsMatrixSize(); i++)
		w[i] = (float)std::rand() / RAND_MAX - 0.5f;
	kn->setWeights(w);
	delete[] w;

	//Mix rows in selection
	std::vector<int> sel_numbers;
	std::vector<int> mixed_sel;
	for (int i = 0; i < rowsCount; i++)
		sel_numbers.push_back(i);
	for (int i = 0; i < rowsCount; i++)
	{
		int index = (float)std::rand() / RAND_MAX * (rowsCount - i - 1);
		int number = sel_numbers[index];
		mixed_sel.push_back(number);
		sel_numbers.erase(sel_numbers.begin() + index);
	}

	int2d mapDim = getNetDimention(kn);

	//Counting number of classes
	int countClasses = 1;
	std::map<int, int> classes = { {0, 0} };	//key - number of y in selection, value - number of class
	for (int i = 1; i < rowsCount; i++)
	{
		int class_number = -1;
		for (auto it = classes.begin(); it != classes.end(); it++)
		{
			if (is_equal(y[it->first], y[i], ySize, eps) == false)
			{
				class_number = it->second;
				break;
			}
		}
		if (class_number == -1)
			classes.insert({ i, countClasses++ });
		else
			classes.insert({ i, class_number });
	}

	//Set up class distribution for kohonen layers
	std::vector<std::vector<float>> classDistribution;
	for (int i = 0; i < mapDim.x * mapDim.y; i++)
		classDistribution.push_back(std::vector<float>(countClasses, 0.0f));

	float lambda = static_cast<float>(maxIterations) / std::fmaxf(mapDim.x, mapDim.y);

	//Clasterization
	for (int iteration = 0; iteration < maxIterations; iteration++)
	{
		float et = std::expf(static_cast<float>(-iteration) / lambda);
		for (int row = 0; row < rowsCount; row++)
		{
			int rowIndex = mixed_sel[row];
			NeuronIndex winner = getWinner(x[rowIndex], kn);
			for (int i = 0; i < mapDim.x; i++)
			{
				for (int j = 0; j < mapDim.y; j++)
				{
					NeuronIndex curNeuron(int2d(i, j));
					float distBMU = winner.distanceTo(curNeuron);	
					float distBMU2 = distBMU * distBMU;
					
					float theta = std::expf(-distBMU2 / (2.0f * sigma0 * sigma0 * et * et));
					float l = l0 * et;
					float learning_rate = l * theta;

					if (learning_rate >= minLearningRate)
					{
						addmultWeights(curNeuron, 1.0f - l*theta, l*theta, x[rowIndex], kn);
						int classIndex = classes[row];
						classDistribution[curNeuron.even_r.x + curNeuron.even_r.y * mapDim.x]
							[classIndex] += std::expf(-distBMU / std::fmaxf(mapDim.x, mapDim.y));
					}
				}
			}
		}
	}
	return classDistribution;
}

void selfOrganization(float** x, float** y, int rowsCount, int xSize, int ySize,
	KohonenNet* kn, int seed, int maxIterations, float sigma0, float l0,
	float minLearningRate, float eps)
{
	auto distr = makeSelfOrganizationDistribution(x, y, rowsCount, xSize, ySize, 
		kn, seed, maxIterations, sigma0, l0, minLearningRate, eps);
	int2d dim = getNetDimention(kn);
	for (int x = 0; x < dim.x; x++)
	{
		for (int y = 0; y < dim.y; y++)
		{
			int classesExists = 0;
			auto distrCurNeuron = distr[y*dim.x + x];
			for (int i = 0; i < distrCurNeuron.size(); i++)
				if (std::abs(distrCurNeuron[i]) >= eps)
					classesExists++;
		}
	}
}