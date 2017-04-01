#include "ConvNNManaged.h"

using namespace dms::solvers::neural_nets::conv_net;

ConvNNManaged::ConvNNManaged(ConvNNTopology^ t) :
	INeuralNetwork(t)
{
	this->t = t;
	FetchNativeParameters();
}

void ConvNNManaged::SetWeights(array<array<float>^>^ weights)
{
	for (int i = 0; i < this->weights->Length; i++)
		for (int j = 0; j < this->weights[i]->Length; j++)
			this->weights[i][j] = weights[i][j];
	PushNativeParameters();
}

void ConvNNManaged::FetchNativeParameters()
{
	auto p_solver = static_cast<nnets_conv::ConvNN*>(getNativeSolver());
	int wlen = p_solver->getWeightsMatricesCount();

	float** w = new float*[wlen];
	weights = gcnew array<array<float>^>(wlen);
	for (int i = 0; i < wlen; i++)
	{
		size_t curlen = p_solver->getWeightsMatrixSize(i);
		w[i] = new float[curlen];
		weights[i] = gcnew array<float>(curlen);
	}
	p_solver->getWeights(w);

	for (int i = 0; i < weights->Length; i++)
		for (int j = 0; j < weights[i]->Length; j++)
			weights[i][j] = w[i][j];

	for (int i = 0; i < p_solver->getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

void ConvNNManaged::PushNativeParameters()
{
	auto p_solver = static_cast<nnets_conv::ConvNN*>(getNativeSolver());
	float** w = new float*[p_solver->getWeightsMatricesCount()];
	for (int i = 0; i < p_solver->getWeightsMatricesCount(); i++)
		w[i] = new float[p_solver->getWeightsMatrixSize(i)];

	for (int i = 0; i < weights->Length; i++)
		for (int j = 0; j < weights[i]->Length; j++)
			w[i][j] = weights[i][j];
	p_solver->setWeights(w);

	for (int i = 0; i < p_solver->getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

void* ConvNNManaged::getAttributes()
{
	return _attr;
}

void* ConvNNManaged::getOperations()
{
	(*_opers)["getAllWeights"]		= nnets_conv::getAllWeightsConv;
	(*_opers)["setAllWeights"]		= nnets_conv::setAllWeightsConv;
	(*_opers)["solve"]				= nnets_conv::solveConv;
	(*_opers)["getWeightsCount"]	= nnets_conv::getWeightsCountConv;
	(*_opers)["copySolver"]			= nnets_conv::copyConv;
	(*_opers)["freeSolver"]			= nnets_conv::freeConv;

	return _opers;
}