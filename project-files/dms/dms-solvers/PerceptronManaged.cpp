#include "PerceptronManaged.h"

using dms::solvers::LearningOperation;
using namespace dms::solvers::neural_nets;
using namespace System::Runtime::InteropServices;

PerceptronManaged::PerceptronManaged(PerceptronTopology^ t, array<array<float>^>^ weights) :
	ISolver(t->GetInputsCount(), t->GetOutputsCount())
{
	int layers = t->GetLayersCount();

	auto ns = t->GetNeuronsInLayersCount();
	auto hds = t->HasLayersDelayWeight();

	ActivationFunctionType* afs = new ActivationFunctionType[layers - 1];
	t->GetLayersActivateFunctionsTypes(afs);

	this->weights = gcnew array<array<float>^>(layers - 1);
	for (int i = 0; i < layers - 1; i++)
	{
		this->weights[i] = gcnew array<float>(ns[i + 1] * (ns[i] + hds[i]));
		for (int j = 0; j < this->weights[i]->Length; j++)
		{
			this->weights[i][j] = weights[i][j];
		}
	}

	int* neurons = new int[layers];
	bool* delays = new bool[layers - 1];

	float** w = new float*[layers - 1];
	for (int i = 0; i < layers - 1; i++)
	{
		int dim = ns[i + 1] * (ns[i] + hds[i]);
		w[i] = new float[dim];
		for (int j = 0; j < dim; j++)
			w[i][j] = this->weights[i][j];
	}

	for (int i = 0; i < layers; i++)
		neurons[i] = ns[i];
	for (int i = 0; i < layers - 1; i++)
	{
		delays[i] = hds[i];
	}

	psolver = new neurolib::Perceptron(neurons, delays, afs, layers, w);
	x = new float[GetInputsCount()];
	y = new float[GetOutputsCount()];

	delete[] neurons;
	delete[] delays;

	for (int i = 0; i < layers - 1; i++)
	{
		delete[] w[i];
	}
	delete[] afs;
	delete[] w;
}

array<Single>^ PerceptronManaged::Solve(array<Single>^ x)
{
	int inputs = GetInputsCount();
	int outputs = GetOutputsCount();

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

std::vector<std::string> PerceptronManaged::getAttributes()
{
	return std::vector<std::string>();
}

std::vector<LearningOperation> PerceptronManaged::getOperations()
{
	std::vector<LearningOperation> opers;
	opers.push_back(LearningOperation{ "getAllWeights", getAllWeightsPerc });
	opers.push_back(LearningOperation{ "setAllWeights", setAllWeightsPerc });
	opers.push_back(LearningOperation{ "solve", solvePerc });
	opers.push_back(LearningOperation{ "getWeightsCount", getWeightsCountPerc });
	opers.push_back(LearningOperation{ "copySolver", copyPerc });
	opers.push_back(LearningOperation{ "freeSolver", freePerc });

	return opers;
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