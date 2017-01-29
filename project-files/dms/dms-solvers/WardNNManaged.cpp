#include "WardNNManaged.h"

using namespace dms::solvers::neural_nets;

WardNNManaged::WardNNManaged(WardNNTopology^ t, array<array<float>^>^ weights) : 
	ISolver(t->getInputsCount(), t->getOutputsCount())
{
	x = new float[GetInputsCount()];
	y = new float[GetOutputsCount()];

	int layers = t->getLayersCount();
	int* groups = new int[layers - 1];
	t->getGroupsCount(groups);

	int* addCons = new int[layers - 2];
	t->getAdditionalConnections(addCons);

	int** neurons = new int*[layers];
	neurons[0] = new int[1];
	for (int i = 1; i < layers; i++)
	{
		neurons[i] = new int[groups[i-1]];
	}
	t->getNeuronsCount(neurons);

	array<int>^ neuronsInLayers = gcnew array<int>(layers);
	neuronsInLayers[0] = neurons[0][0];
	for (int i = 1; i < layers; i++)
	{
		int sum = 0;
		for (int j = 0; j < groups[i - 1]; j++)
			sum += neurons[i][j];
		neuronsInLayers[i] = sum;
	}
	array<int>^ addNeurons = gcnew array<int>(layers - 1);
	for (int i = 0; i < layers - 1; i++)
	{
		addNeurons[i] = 0;
	}
	for (int i = 0; i < layers - 2; i++)
	{
		int conn = addCons[i];
		if (conn > 0)
		{
			addNeurons[i + conn] += neuronsInLayers[i];
		}
	}

	bool** delays = new bool*[layers - 1];
	ActivationFunctionType** afs = new ActivationFunctionType*[layers - 1];
	for (int i = 0; i < layers - 1; i++)
	{
		delays[i] = new bool[groups[i]];
		afs[i] = new ActivationFunctionType[groups[i]];
	}
	t->getDelays(delays);
	t->getActivateFunctionsTypes(afs);
	
	this->weights = gcnew array<array<float>^>(layers - 1);
	float** w = new float*[layers - 1];
	for (int i = 0; i < layers - 1; i++)
	{
		int s = addNeurons[i];
		int n = neuronsInLayers[i];
		int m = neuronsInLayers[i + 1];
		int k = 0;
		for (int j = 0; j < groups[i]; j++)
		{
			k += System::Convert::ToInt32(delays[i][j]);
		}
		int len = s*m + n* (m + k);
		this->weights[i] = gcnew array<float>(len);
		w[i] = new float[len];
		for (int j = 0; j < len; j++)
		{
			w[i][j] = this->weights[i][j] = weights[i][j];
		}
	}

	wsolver = new neurolib::WardNN(neurons, delays, afs, groups, addCons, layers, w);

	for (int i = 0; i < layers - 1; i++)
	{
		delete[] w[i];
		delete[] afs[i];
		delete[] delays[i];
	}
	delete[] w;
	delete[] afs;
	delete[] delays;

	for (int i = 0; i < layers; i++)
		delete[] neurons[i];
	delete[] neurons;
	delete[] groups;
	delete[] addCons;
}

array<Single>^ WardNNManaged::Solve(array<Single>^ x)
{
	int inputs = GetInputsCount();
	int outputs = GetOutputsCount();

	if (x->Length != inputs)
		throw gcnew System::ArgumentException();

	for (int i = 0; i < inputs; i++)
		this->x[i] = x[i];

	if (outputs != wsolver->solve(this->x, this->y))
		throw gcnew System::IndexOutOfRangeException();

	array<Single>^ y = gcnew array<Single>(outputs);
	for (int i = 0; i < outputs; i++)
	{
		y[i] = this->y[i];
	}
	return y;
}

WardNNManaged::~WardNNManaged()
{
	delete[] x;
	delete[] y;
	delete wsolver;
}