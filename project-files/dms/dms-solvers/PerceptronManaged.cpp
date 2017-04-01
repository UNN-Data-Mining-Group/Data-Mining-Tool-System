#include "PerceptronManaged.h"

using dms::solvers::neural_nets::perceptron::PerceptronManaged;
using dms::solvers::neural_nets::perceptron::PerceptronTopology;

PerceptronManaged::PerceptronManaged(PerceptronTopology^ t) :
	INeuralNetwork(t)
{
	this->t = t;
	FetchNativeParameters();
}

void PerceptronManaged::SetWeights(array<array<float>^>^ weights)
{
	for (int i = 0; i < this->weights->Length; i++)
		for (int j = 0; j < this->weights[i]->Length; j++)
			this->weights[i][j] = weights[i][j];
	PushNativeParameters();
}

void PerceptronManaged::FetchNativeParameters()
{
	auto psolver =
		static_cast<nnets_perceptron::Perceptron*>(getNativeSolver());

	int wlen = psolver->getWeightsMatricesCount();

	float** w = new float*[wlen];
	weights = gcnew array<array<float>^>(wlen);
	for (int i = 0; i < wlen; i++)
	{
		size_t curlen = psolver->getWeightsMatrixSize(i);
		w[i] = new float[curlen];
		weights[i] = gcnew array<float>(curlen);
	}
	psolver->getWeights(w);

	for (int i = 0; i < weights->Length; i++)
		for (int j = 0; j < weights[i]->Length; j++)
			weights[i][j] = w[i][j];

	for (int i = 0; i < psolver->getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

void PerceptronManaged::PushNativeParameters()
{
	auto psolver =
		static_cast<nnets_perceptron::Perceptron*>(getNativeSolver());

	float** w = new float*[psolver->getWeightsMatricesCount()];
	for (int i = 0; i < psolver->getWeightsMatricesCount(); i++)
		w[i] = new float[psolver->getWeightsMatrixSize(i)];

	for (int i = 0; i < weights->Length; i++)
		for (int j = 0; j < weights[i]->Length; j++)
			w[i][j] = weights[i][j];
	psolver->setWeights(w);

	for (int i = 0; i < psolver->getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

void* PerceptronManaged::getAttributes()
{
	return _attr;
}

void* PerceptronManaged::getOperations()
{
	(*_opers)["getAllWeights"]		= nnets_perceptron::getAllWeightsPerc;
	(*_opers)["setAllWeights"]		= nnets_perceptron::setAllWeightsPerc;
	(*_opers)["solve"]				= nnets_perceptron::solvePerc;
	(*_opers)["getWeightsCount"]	= nnets_perceptron::getWeightsCountPerc;
	(*_opers)["copySolver"]			= nnets_perceptron::copyPerc;
	(*_opers)["freeSolver"]			= nnets_perceptron::freePerc;

	if (t->HasSmoothAfs() == true)
	{
		(*_opers)["getIterationsCount"]			= nnets_perceptron::getIterationsCount;
		(*_opers)["getIterationSizes"]			= nnets_perceptron::getIterationSizes;
		(*_opers)["getWeightsVectors"]			= nnets_perceptron::getWeightsVectors;
		(*_opers)["getWeightsVectorsCount"]		= nnets_perceptron::getWeightsVectorsCount;
		(*_opers)["getWeightsVectorSize"]		= nnets_perceptron::getWeightsVectorSize;
		(*_opers)["getIterationDerivatives"]	= nnets_perceptron::getIterationDerivatives;
		(*_opers)["getIterationValues"]			= nnets_perceptron::getIterationValues;
		(*_opers)["setWeightsVector"]			= nnets_perceptron::setWeightsVector;
	}
	return _opers;
}