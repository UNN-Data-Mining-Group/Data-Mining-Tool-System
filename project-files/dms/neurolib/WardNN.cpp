#include "WardNN.h"
#include <mkl_cblas.h>
#include <cmath>

using namespace neurolib;

WardNN::WardNN(const WardNN& wnn)
{
	inputsCount = wnn.inputsCount;
	layersCount = wnn.layersCount;
	outputsCount = wnn.outputsCount;

	groupsCount = new int[layersCount - 1];
	additionalNeurons = new int[layersCount - 1];
	additionalLayerConnection = new int[layersCount - 1];
	hasDelay = new bool*[layersCount - 1];
	af_types = new ActivationFunctionType*[layersCount - 1];

	for (int i = 0; i < layersCount - 1; i++)
	{
		groupsCount[i] = wnn.groupsCount[i];
		additionalNeurons[i] = wnn.additionalNeurons[i];
		additionalLayerConnection[i] = wnn.additionalLayerConnection[i];

		this->af_types[i] = new ActivationFunctionType[groupsCount[i]];
		hasDelay[i] = new bool[groupsCount[i]];
		for (int j = 0; j < groupsCount[i]; j++)
		{
			this->af_types[i][j] = wnn.af_types[i][j];
			hasDelay[i][j] = wnn.hasDelay[i][j];
		}
	}

	neuronsInLayer = new int[layersCount];
	neuronsInGroup = new int*[layersCount];
	for (int i = 0; i < layersCount; i++)
		neuronsInLayer[i] = wnn.neuronsInLayer[i];

	neuronsInGroup[0] = new int[1]; neuronsInGroup[0][0] = wnn.neuronsInGroup[0][0];
	for (int i = 1; i < layersCount; i++)
	{
		neuronsInGroup[i] = new int[groupsCount[i - 1]];
		for (int j = 0; j < groupsCount[i - 1]; j++)
			neuronsInGroup[i][j] = wnn.neuronsInGroup[i][j];
	}

	w = new float*[layersCount - 1];
	for (int i = 0; i < layersCount - 1; i++)
	{
		int s = additionalNeurons[i];
		int n = neuronsInLayer[i];
		int m = neuronsInLayer[i + 1];
		int k = 0;
		for (int j = 0; j < groupsCount[i]; j++)
		{
			k += hasDelay[i][j];
		}
		int len = s*m + n* (m + k);
		w[i] = new float[len];
		for (int j = 0; j < len; j++)
		{
			w[i][j] = wnn.w[i][j];
		}
	}

	init_calc_vars();
}

WardNN::WardNN(int** neuronsCount, bool** isDelayOnGroup, ActivationFunctionType** af_types, int* groupsCount, int* additionalLayerConnection, int layersCount, float** weights)
{
	this->layersCount = layersCount;
	this->groupsCount = new int[layersCount-1];
	neuronsInLayer = new int[layersCount];
	neuronsInGroup = new int*[layersCount];
	
	neuronsInGroup[0] = new int[1];
	neuronsInGroup[0][0] = neuronsCount[0][0];
	neuronsInLayer[0] = neuronsCount[0][0];
	for (int i = 1; i < layersCount; i++)
	{
		this->groupsCount[i-1] = groupsCount[i-1];
		int sumNeurons = 0;
		neuronsInGroup[i] = new int[groupsCount[i-1]];
		for (int j = 0; j < groupsCount[i-1]; j++)
		{
			neuronsInGroup[i][j] = neuronsCount[i][j];
			sumNeurons += neuronsCount[i][j];
		}
		neuronsInLayer[i] = sumNeurons;
	}
	inputsCount = neuronsInLayer[0];
	outputsCount = neuronsInLayer[layersCount - 1];

	this->additionalLayerConnection = new int[layersCount - 1];
	additionalNeurons = new int[layersCount - 1];
	for (int i = 0; i < layersCount - 1; i++)
	{
		additionalNeurons[i] = 0;
		this->additionalLayerConnection[i] = -1;
	}
	for (int i = 0; i < layersCount - 2; i++)
	{
		int conn = additionalLayerConnection[i];
		if (conn > 0)
		{
			this->additionalLayerConnection[i + conn] = i;
			additionalNeurons[i + conn] += neuronsInLayer[i];
		}
	}
	hasDelay = new bool*[layersCount - 1];
	this->af_types = new ActivationFunctionType*[layersCount - 1];
	for (int i = 0; i < layersCount - 1; i++)
	{
		this->af_types[i] = new ActivationFunctionType[groupsCount[i]];
		hasDelay[i] = new bool[groupsCount[i]];
		for (int j = 0; j < groupsCount[i]; j++)
		{
			this->af_types[i][j] = af_types[i][j];
			hasDelay[i][j] = isDelayOnGroup[i][j];
		}
	}

	w = new float*[layersCount - 1];
	for (int i = 0; i < layersCount - 1; i++)
	{
		int s = additionalNeurons[i];
		int n = neuronsInLayer[i];
		int m = neuronsInLayer[i + 1];
		int k = 0;
		for (int j = 0; j < groupsCount[i]; j++)
		{
			k += hasDelay[i][j];
		}
		int len = s*m + n* (m + k);
		w[i] = new float[len];
		for (int j = 0; j < len; j++)
		{
			w[i][j] = weights[i][j];
		}
	}

	init_calc_vars();
}

int WardNN::solve(float* x, float* y)
{
	for (int i = 0; i < inputsCount; i++)
		temp_res[0][i] = x[i];

	for (int i = 1; i < layersCount; i++)
	{
		int k = 0;
		int d = 0;
		for (int j = 0; j < groupsCount[i-1]; j++)
		{
			const int cols = neuronsInLayer[i - 1] + hasDelay[i - 1][j] + additionalNeurons[i - 1];
			const int n = neuronsInGroup[i][j];

			int l = 0;
			for (; l < neuronsInLayer[i - 1]; l++)
			{
				buf_x[l] = temp_res[i - 1][l];
			}
			if (hasDelay[i - 1][j])
			{
				buf_x[l] = -1.0f;
				l++;
			}
			const int li = additionalLayerConnection[i - 1];
			for (int r = 0; l < cols; r++)
			{
				buf_x[l] = temp_res[li][r];
				l++;
			}
			float* x_cur = temp_res[i] + d;
			cblas_sgemv(CblasRowMajor, CblasNoTrans, n, cols, 1.0f, w[i - 1] + k, cols, buf_x, 1, 0.0f, x_cur, 1);
			calc_activation_function(x_cur, n, af_types[i - 1][j], x_cur);
			k += n * cols;
			d += n;
		}
	}

	for (int i = 0; i < outputsCount; i++)
	{
		y[i] = temp_res[layersCount - 1][i];
	}

	return outputsCount;
}

int WardNN::getInputsCount()
{
	return inputsCount;
}

int WardNN::getOutputsCount()
{
	return outputsCount;
}

WardNN::~WardNN()
{
	delete[] buf_x;
	for (int i = 0; i < layersCount; i++)
	{
		delete[] temp_res[i];
		delete[] neuronsInGroup[i];
	}
	delete[] temp_res;
	delete[] neuronsInGroup;

	for (int i = 0; i < layersCount - 1; i++)
	{
		delete[] af_types[i];
		delete[] w[i];
		delete[] hasDelay[i];
	}
	delete[] af_types;
	delete[] w;
	delete[] hasDelay;
	delete[] neuronsInLayer;
	delete[] additionalNeurons;
	delete[] additionalLayerConnection;
	delete[] groupsCount;
}

void WardNN::init_calc_vars()
{
	temp_res = new float*[layersCount];
	for (int i = 0; i < layersCount; i++)
	{
		temp_res[i] = new float[neuronsInLayer[i]];
	}

	int buf_x_len = 0;
	for (int i = 1; i < layersCount; i++)
	{
		for (int j = 0; j < groupsCount[i - 1]; j++)
		{
			int l = neuronsInLayer[i - 1] + hasDelay[i - 1][j] + additionalNeurons[i - 1];
			buf_x_len = buf_x_len > l ? buf_x_len : l;
		}
	}
	buf_x = new float[buf_x_len];
}