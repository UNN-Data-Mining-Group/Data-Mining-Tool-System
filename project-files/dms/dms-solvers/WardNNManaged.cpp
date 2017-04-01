#include "WardNNManaged.h"

using namespace dms::solvers::neural_nets::ward_net;

WardNNManaged::WardNNManaged(WardNNTopology^ t) : 
	INeuralNetwork(t)
{
	this->t = t;
	FetchNativeParameters();
}

void WardNNManaged::FetchNativeParameters()
{
	auto wsolver =
		static_cast<nnets_ward::WardNN*>(getNativeSolver());
	int wlen = wsolver->getWeightsMatricesCount();

	float** w = new float*[wlen];
	weights = gcnew array<array<float>^>(wlen);
	for (int i = 0; i < wlen; i++)
	{
		size_t curlen = wsolver->getWeightsMatrixSize(i);
		w[i] = new float[curlen];
		weights[i] = gcnew array<float>(curlen);
	}
	wsolver->getWeights(w);

	for (int i = 0; i < weights->Length; i++)
		for (int j = 0; j < weights[i]->Length; j++)
			weights[i][j] = w[i][j];

	for (int i = 0; i < wsolver->getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

void WardNNManaged::SetWeights(array<array<float>^>^ weights)
{
	for (int i = 0; i < this->weights->Length; i++)
		for (int j = 0; j < this->weights[i]->Length; j++)
			this->weights[i][j] = weights[i][j];
	PushNativeParameters();
}

void WardNNManaged::PushNativeParameters()
{
	auto wsolver = 
		static_cast<nnets_ward::WardNN*>(getNativeSolver());

	float** w = new float*[wsolver->getWeightsMatricesCount()];
	for (int i = 0; i < wsolver->getWeightsMatricesCount(); i++)
		w[i] = new float[wsolver->getWeightsMatrixSize(i)];

	for (int i = 0; i < weights->Length; i++)
		for (int j = 0; j < weights[i]->Length; j++)
			w[i][j] = weights[i][j];
	wsolver->setWeights(w);

	for (int i = 0; i < wsolver->getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

void* WardNNManaged::getAttributes()
{
	return _attr;
}

void* WardNNManaged::getOperations()
{
	(*_opers)["getAllWeights"]		= nnets_ward::getAllWeightsWard;
	(*_opers)["setAllWeights"]		= nnets_ward::setAllWeightsWard;
	(*_opers)["solve"]				= nnets_ward::solveWard;
	(*_opers)["getWeightsCount"]	= nnets_ward::getWeightsCountWard;
	(*_opers)["copySolver"]			= nnets_ward::copyWard;
	(*_opers)["freeSolver"]			= nnets_ward::freeWard;

	return _opers;
}