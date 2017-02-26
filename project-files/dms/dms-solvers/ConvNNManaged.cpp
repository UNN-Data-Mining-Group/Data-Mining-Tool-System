#include "ConvNNManaged.h"

using namespace dms::solvers::neural_nets::conv_net;

ConvNNManaged::ConvNNManaged(ConvNNTopology^ t) :
	INeuralNetwork(t->GetInputsCount(), t->GetOutputsCount())
{
	this->t = t;
	initConv();
	FetchNativeParameters();
}

void ConvNNManaged::SetWeights(array<array<float>^>^ weights)
{
	for (int i = 0; i < this->weights->Length; i++)
		for (int j = 0; j < this->weights[i]->Length; j++)
			this->weights[i][j] = weights[i][j];
	PushNativeParameters();
}

array<float>^ ConvNNManaged::Solve(array<float>^ x)
{
	size_t inputs = GetInputsCount();
	size_t outputs = GetOutputsCount();

	if (x->Length != inputs)
		throw gcnew System::ArgumentException();

	for (size_t i = 0; i < inputs; i++)
		this->x[i] = x[i];

	if (outputs != solver->solve(this->x, this->y))
		throw gcnew System::IndexOutOfRangeException();

	array<Single>^ y = gcnew array<Single>(outputs);
	for (size_t i = 0; i < outputs; i++)
	{
		y[i] = this->y[i];
	}
	return y;
}

void ConvNNManaged::FetchNativeParameters()
{
	int wlen = solver->getWeightsMatricesCount();

	float** w = new float*[wlen];
	weights = gcnew array<array<float>^>(wlen);
	for (int i = 0; i < wlen; i++)
	{
		size_t curlen = solver->getWeightsMatrixSize(i);
		w[i] = new float[curlen];
		weights[i] = gcnew array<float>(curlen);
	}
	solver->getWeights(w);

	for (int i = 0; i < weights->Length; i++)
		for (int j = 0; j < weights[i]->Length; j++)
			weights[i][j] = w[i][j];

	for (int i = 0; i < solver->getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

void ConvNNManaged::initConv()
{
	std::vector<nnets_conv::Layer*> layers;
	auto ls = t->GetLayers();
	for (int i = 0; i < ls->Count; i++)
	{
		if (ls[i]->GetType() == FullyConnectedLayer::typeid)
		{
			auto l = (FullyConnectedLayer^)ls[i];
			layers.push_back(
				new nnets_conv::FullyConnectedLayer(l->NeuronsCount,
					ActivationFunctionTypes::getType(l->ActivationFunction)));
		}
		else if (ls[i]->GetType() == ConvolutionLayer::typeid)
		{
			auto l = (ConvolutionLayer^)ls[i];
			layers.push_back(
				new nnets_conv::ConvolutionLayer(l->FilterWidth, l->FilterHeight,
					l->StrideWidth, l->StrideHeight,
					l->Padding, l->CountFilters,
					ActivationFunctionTypes::getType(l->ActivationFunction)));
		}
		else if (ls[i]->GetType() == PoolingLayer::typeid)
		{
			auto l = (PoolingLayer^)ls[i];
			layers.push_back(
				new nnets_conv::PoolingLayer(l->FilterWidth, l->FilterHeight, l->StrideWidth, l->StrideHeight));
		}
	}

	solver = new nnets_conv::ConvNN(t->GetInputWidth(), t->GetInputHeigth(), t->GetInputDepth(), layers);
	x = new float[t->GetInputsCount()];
	y = new float[t->GetOutputsCount()];

	for (int i = 0; i < ls->Count; i++)
		delete layers[i];
}

void ConvNNManaged::PushNativeParameters()
{
	initConv();

	float** w = new float*[solver->getWeightsMatricesCount()];
	for (int i = 0; i < solver->getWeightsMatricesCount(); i++)
		w[i] = new float[solver->getWeightsMatrixSize(i)];

	for (int i = 0; i < weights->Length; i++)
		for (int j = 0; j < weights[i]->Length; j++)
			w[i][j] = weights[i][j];
	solver->setWeights(w);

	for (int i = 0; i < solver->getWeightsMatricesCount(); i++)
		delete[] w[i];
	delete[] w;
}

std::vector<std::string> ConvNNManaged::getAttributes()
{
	return std::vector<std::string>();
}

std::map<std::string, void*> ConvNNManaged::getOperations()
{
	std::map<std::string, void*> opers;
	opers["getAllWeights"]		= nnets_conv::getAllWeightsConv;
	opers["setAllWeights"]		= nnets_conv::setAllWeightsConv;
	opers["solve"]				= nnets_conv::solveConv;
	opers["getWeightsCount"]	= nnets_conv::getWeightsCountConv;
	opers["copySolver"]			= nnets_conv::copyConv;
	opers["freeSolver"]			= nnets_conv::freeConv;

	return opers;
}

void* ConvNNManaged::getNativeSolver()
{
	return solver;
}

ConvNNManaged::~ConvNNManaged()
{
	delete[] x;
	delete[] y;

	delete solver;
}