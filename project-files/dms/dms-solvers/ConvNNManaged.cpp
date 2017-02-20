#include "ConvNNManaged.h"

using namespace dms::solvers::neural_nets;

ConvNNManaged::ConvNNManaged(ConvNNTopology^ t, array<array<float>^>^ weights) :
	ISolver(t->GetInputsCount(), t->GetOutputsCount())
{
	float** w = new float*[weights->Length];
	this->weights = gcnew array<array<float>^>(weights->Length);
	for (int i = 0; i < weights->Length; i++)
	{
		w[i] = new float[weights[i]->Length];
		this->weights[i] = gcnew array<float>(weights[i]->Length);
		for (int j = 0; j < weights[i]->Length; j++)
		{
			this->weights[i][j] = weights[i][j];
			w[i][j] = weights[i][j];
		}
	}

	std::vector<neurolib::ConvNNLayer*> layers;
	auto ls = t->GetLayers();
	for (int i = 0; i < ls->Count; i++)
	{
		if (ls[i]->GetType() == ConvNNFullyConnectedLayer::typeid)
		{
			auto l = (ConvNNFullyConnectedLayer^)ls[i];
			layers.push_back(
				new neurolib::ConvNNFullyConnectedLayer(l->NeuronsCount, 
				 ActivationFunctionTypes::getType(l->ActivationFunction)));
		}
		else if (ls[i]->GetType() == ConvNNConvolutionLayer::typeid)
		{
			auto l = (ConvNNConvolutionLayer^)ls[i];
			layers.push_back(
				new neurolib::ConvNNConvolutionLayer(l->FilterWidth, l->FilterHeight,
					l->StrideWidth, l->StrideHeight,
					l->Padding, l->CountFilters,
					ActivationFunctionTypes::getType(l->ActivationFunction)));
		}
		else if (ls[i]->GetType() == ConvNNPoolingLayer::typeid)
		{
			auto l = (ConvNNPoolingLayer^)ls[i];
			layers.push_back(
			new neurolib::ConvNNPoolingLayer(l->FilterWidth, l->FilterHeight, l->StrideWidth, l->StrideHeight));
		}
	}

	solver = new neurolib::ConvNN(t->GetInputWidth(), t->GetInputHeigth(), t->GetInputDepth(), layers, w);
	x = new float[t->GetInputsCount()];
	y = new float[t->GetOutputsCount()];

	for (int i = 0; i < ls->Count; i++)
		delete layers[i];
	for (int i = 0; i < weights->Length; i++)
		delete[] w[i];
	delete[] w;
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

ConvNNManaged::~ConvNNManaged()
{
	delete[] x;
	delete[] y;

	delete solver;
}