#include "PerceptronManaged.h"

using dms::solvers::neural_nets::perceptron::PerceptronManaged;
using dms::solvers::neural_nets::perceptron::PerceptronTopology;

PerceptronManaged::PerceptronManaged(PerceptronTopology^ t) :
	INeuralNetwork(t->GetInputsCount(), t->GetOutputsCount())
{
	this->t = t;
	initPerceptron();
	FetchNativeParameters();
}

void PerceptronManaged::SetWeights(array<array<float>^>^ weights)
{
	for (int i = 0; i < this->weights->Length; i++)
		for (int j = 0; j < this->weights[i]->Length; j++)
			this->weights[i][j] = weights[i][j];
	PushNativeParameters();
}

array<Single>^ PerceptronManaged::Solve(array<Single>^ x)
{
	__int64 inputs = GetInputsCount();
	__int64 outputs = GetOutputsCount();

	if (x->Length != inputs)
		throw gcnew System::ArgumentException();

	for (int i = 0; i < inputs; i++)
		this->x[i] = x[i];

	if (outputs != psolver->solve(this->x, this->y))
		throw gcnew System::IndexOutOfRangeException();
	
	array<Single>^ y = gcnew array<Single>(outputs);
	for (int i = 0; i < outputs; i++)
	{
		y[i] = this->y[i];
	}
	return y;
}

void PerceptronManaged::FetchNativeParameters()
{
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

void PerceptronManaged::initPerceptron()
{
	int layers = t->GetLayersCount();

	auto ns = t->GetNeuronsInLayersCount();
	auto hds = t->HasLayersDelayWeight();

	nnets::ActivationFunctionType* afs = new nnets::ActivationFunctionType[layers - 1];
	t->GetLayersActivateFunctionsTypes(afs);

	hasSmoothAfs = true;
	for (int i = 0; i < layers - 1; i++)
	{
		if (nnets::has_derivative(afs[i]) == false)
		{
			hasSmoothAfs = false;
			break;
		}
	}

	int* neurons = new int[layers];
	bool* delays = new bool[layers - 1];

	for (int i = 0; i < layers; i++)
		neurons[i] = ns[i];
	for (int i = 0; i < layers - 1; i++)
	{
		delays[i] = hds[i];
	}

	psolver = new nnets_perceptron::Perceptron(neurons, delays, afs, layers);
	x = new float[GetInputsCount()];
	y = new float[GetOutputsCount()];

	delete[] neurons;
	delete[] delays;
	delete[] afs;
}

void PerceptronManaged::PushNativeParameters()
{
	initPerceptron();

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

	if (hasSmoothAfs == true)
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

void* PerceptronManaged::getNativeSolver()
{
	return psolver;
}

PerceptronManaged::~PerceptronManaged()
{
	delete[] x;
	delete[] y;

	delete psolver;
	psolver = nullptr;
}