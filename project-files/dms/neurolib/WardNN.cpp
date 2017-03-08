#include "WardNN.h"
#include <mkl_cblas.h>
#include <cmath>

using nnets_ward::WardNN;
using nnets::ActivationFunctionType;

size_t nnets_ward::getAllWeightsWard(float* dest, void* obj)
{
	WardNN* wnn = static_cast<WardNN*>(obj);

	size_t destIndex = 0;
	for (int i = 0; i < wnn->layers.size() - 1; i++)
		for (size_t j = 0; j < wnn->w_sizes[i]; j++)
			dest[destIndex++] = wnn->w[i][j];

	return destIndex;
}

void nnets_ward::setAllWeightsWard(const float* src, void* obj)
{
	WardNN* wnn = static_cast<WardNN*>(obj);

	size_t srcIndex = 0;
	for (int i = 0; i < wnn->layers.size() - 1; i++)
		for (size_t j = 0; j < wnn->w_sizes[i]; j++)
			wnn->w[i][j] = src[srcIndex++];
}

size_t nnets_ward::solveWard(const float* x, float* y, void* obj)
{
	WardNN* wnn = static_cast<WardNN*>(obj);
	return wnn->solve(x, y);
}

size_t nnets_ward::getWeightsCountWard(void* obj)
{
	WardNN* wnn = static_cast<WardNN*>(obj);
	size_t res = 0;
	for (int i = 0; i < wnn->layers.size() - 1; i++)
		res += wnn->w_sizes[i];

	return res;
}

void* nnets_ward::copyWard(void* obj)
{
	WardNN* wnn = static_cast<WardNN*>(obj);
	return new WardNN(*wnn);
}

void nnets_ward::freeWard(void* &obj)
{
	WardNN* wnn = static_cast<WardNN*>(obj);
	delete wnn;
	obj = nullptr;
}

struct WardNN::Layer
{
	std::vector<NeuronsGroup> Groups;
	const Layer* BackwardConnector;
	float* Values;			//values after applying activations
	float* WeightedSums;	//values before activations applied
	size_t ValuesSize;

	//Constructor for input layer (without groups and activations)
	Layer(size_t neurons_count) :
		BackwardConnector(nullptr), ValuesSize(neurons_count)
	{
		Groups = std::vector<NeuronsGroup>();
		Values = new float[ValuesSize];
		WeightedSums = nullptr;
	}

	Layer(const Layer& l)
	{
		Values = WeightedSums = nullptr;

		Groups = l.Groups;
		BackwardConnector = l.BackwardConnector;
		ValuesSize = l.ValuesSize;

		if (l.Values != nullptr)
		{
			Values = new float[ValuesSize];
			for (int i = 0; i < ValuesSize; i++)
				Values[i] = l.Values[i];
		}

		if (l.WeightedSums != nullptr)
		{
			WeightedSums = new float[ValuesSize];
			for (int i = 0; i < ValuesSize; i++)
				WeightedSums[i] = l.WeightedSums[i];
		}
	}

	Layer(const std::vector<NeuronsGroup> &groups, const Layer* connector) :
		BackwardConnector(connector), Groups(groups)
	{
		ValuesSize = 0;
		for (int group = 0; group < Groups.size(); group++)
		{
			ValuesSize += groups[group].NeuronsCount;
		}
		Values = new float[ValuesSize];
		WeightedSums = new float[ValuesSize];
	}

	~Layer()
	{
		ValuesSize = 0;

		if (Values != nullptr)
			delete[] Values;
		if (WeightedSums != nullptr)
			delete[] WeightedSums;

		Values = WeightedSums = nullptr;
	}
};

WardNN::WardNN(const WardNN& wnn)
{
	layers = wnn.layers;
	alloc_data();
	setWeights(wnn.w);
}

WardNN::WardNN(nnets_ward::InputLayer input, const std::vector<nnets_ward::Layer> layers)
{
	if (input.NeuronsCount < 0)
		throw "Invalid number of inputs";

	this->layers.push_back(Layer{ input.NeuronsCount }); //input layer, without biases and activations
	for (int i = 0; i < layers.size(); i++)
		this->layers.push_back(Layer{ layers[i].Groups, nullptr });

	if (input.ForwardConnection > 0)
		this->layers[input.ForwardConnection + 1].BackwardConnector = &this->layers[0];
	for (int i = 0; i < layers.size(); i++)
	{
		size_t forward_index = layers[i].ForwardConnection;
		if (forward_index > 0)
			this->layers[i + forward_index + 1].BackwardConnector = &this->layers[i];
	}

	alloc_data();
}

void WardNN::alloc_data()
{
	size_t buf_x_len = 0;
	w = new float*[this->layers.size() - 1];
	w_sizes = new size_t[this->layers.size() - 1];

	for (int i = 1; i < this->layers.size(); i++)
	{
		size_t temp = this->layers[i - 1].ValuesSize;
		if (this->layers[i].BackwardConnector != nullptr)
			temp += this->layers[i].BackwardConnector->ValuesSize;

		buf_x_len = buf_x_len < temp ? temp : buf_x_len;

		size_t back_size = 0;
		if (this->layers[i].BackwardConnector != nullptr)
			back_size = this->layers[i].BackwardConnector->ValuesSize;
		size_t len = this->layers[i].ValuesSize * (this->layers[i - 1].ValuesSize + back_size);

		for (auto group = this->layers[i].Groups.begin();
			group != this->layers[i].Groups.end(); group++)
		{
			if (group->HasDelay == true)
				len += group->NeuronsCount;
		}

		w_sizes[i - 1] = len;
		w[i - 1] = new float[len];
		for (int j = 0; j < len; j++)
			w[i - 1][j] = 0.0f;
	}
	buf_x = new float[buf_x_len];
}

size_t WardNN::solve(const float* x, float* y)
{
	Layer* first = &layers[0];
	Layer* last = &layers[layers.size() - 1];

	for (int i = 0; i < getInputsCount(); i++)
		first->Values[i] = x[i];

	int w_index = 0;
	auto prevLayer = layers.begin();
	auto curLayer = prevLayer + 1;
	for (; curLayer != layers.end(); curLayer++, prevLayer++, w_index++)
	{
		//Computing weighted sum without biases
		size_t rows = curLayer->ValuesSize;
		size_t cols = prevLayer->ValuesSize;

		const Layer* backConnector = curLayer->BackwardConnector;
		size_t x_index = 0;
		for (size_t prevItem = 0; prevItem < prevLayer->ValuesSize; prevItem++)
			buf_x[x_index++] = prevLayer->Values[prevItem];

		if (backConnector != nullptr)
		{
			for (size_t backItem = 0; backItem < backConnector->ValuesSize; backItem++)
				buf_x[x_index++] = backConnector->Values[backItem];

			cols += backConnector->ValuesSize;
		}

		cblas_sgemv(CblasRowMajor, CblasNoTrans,
			rows, cols, 1.0f, w[w_index],
			cols, buf_x, 1, 0.0f, curLayer->WeightedSums, 1);

		//Applying biases and activations in groups
		float* group_values = curLayer->Values;
		float* group_wsums = curLayer->WeightedSums;
		float* bias_weights = w[w_index] + rows * cols;
		for (auto group = curLayer->Groups.begin(); group != curLayer->Groups.end(); group++)
		{
			if (group->HasDelay == true)
			{
				for (int i = 0; i < group->NeuronsCount; i++)
					group_wsums[i] -= bias_weights[i];

				bias_weights += group->NeuronsCount;
			}
			nnets::calc_activation_function(group_wsums, 
				group->NeuronsCount, group->ActivationFunction, group_values);
			group_values += group->NeuronsCount;
			group_wsums += group->NeuronsCount;
		}

	}

	for (int i = 0; i < getOutputsCount(); i++)
		y[i] = last->Values[i];

	return getOutputsCount();
}

size_t WardNN::getWeights(float** weights)
{
	size_t allSize = 0;
	for (int i = 0; i < layers.size() - 1; i++)
	{
		for (size_t j = 0; j < w_sizes[i]; j++)
		{
			weights[i][j] = w[i][j];
			allSize++;
		}
	}

	return allSize;
}

int WardNN::getWeightsMatricesCount()
{
	return layers.size() - 1;
}

size_t WardNN::getWeightsMatrixSize(int matrixIndex)
{
	if ((matrixIndex < 0) || (matrixIndex > (layers.size() - 2)))
		return 0;
	return w_sizes[matrixIndex];
}

void WardNN::setWeights(float ** weights)
{
	for (int i = 0; i < this->layers.size() - 1; i++)
	{
		for (size_t j = 0; j < w_sizes[i]; j++)
			w[i][j] = weights[i][j];
	}
}

size_t WardNN::getInputsCount()
{
	return layers[0].ValuesSize;
}

size_t WardNN::getOutputsCount()
{
	return layers[layers.size() - 1].ValuesSize;
}

WardNN::~WardNN()
{
	delete[] buf_x;
	for (int i = 0; i < layers.size() - 1; i++)
		delete[] w[i];
	delete[] w;
}