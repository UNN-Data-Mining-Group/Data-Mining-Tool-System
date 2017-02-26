#include "WardNNManaged.h"

using namespace dms::solvers::neural_nets::ward_net;

WardNNManaged::WardNNManaged(WardNNTopology^ t) : 
	INeuralNetwork(t->GetInputsCount(), t->GetOutputsCount())
{
	this->t = t;
	initWard();
	FetchNativeParameters();
}

array<Single>^ WardNNManaged::Solve(array<Single>^ x)
{
	__int64 inputs = GetInputsCount();
	__int64 outputs = GetOutputsCount();

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

void WardNNManaged::FetchNativeParameters()
{
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

void WardNNManaged::initWard()
{
	x = new float[GetInputsCount()];
	y = new float[GetOutputsCount()];

	InputLayer^ input = t->GetInputLayer();
	List<Layer^>^ layers = t->GetLayers();

	String^ exMessage = "";
	bool isCreationSuccessfull = true;
	try
	{
		nnets_ward::InputLayer native_input
		{
			static_cast<size_t>(input->NeuronsCount),
			static_cast<size_t>(input->ForwardConnection)
		};
		std::vector<nnets_ward::Layer> native_layers;
		for (int i = 0; i < layers->Count; i++)
		{
			List<NeuronsGroup^>^ g = layers[i]->Groups;
			std::vector<nnets_ward::NeuronsGroup> groups;
			for (int j = 0; j < g->Count; j++)
			{
				groups.push_back(nnets_ward::NeuronsGroup
				{
					static_cast<size_t>(g[j]->NeuronsCount),
					g[j]->HasDelay,
					ActivationFunctionTypes::getType(g[j]->ActivationFunction)
				});
			}
			native_layers.push_back(nnets_ward::Layer{ static_cast<size_t>(layers[i]->ForwardConnection), groups });
		}

		wsolver = new nnets_ward::WardNN(native_input, native_layers);
	}
	catch (char* msg)
	{
		isCreationSuccessfull = false;
		exMessage = gcnew String(msg);
	}
	catch (...)
	{
		isCreationSuccessfull = false;
	}

	if (isCreationSuccessfull == false)
	{
		throw gcnew System::Exception(exMessage);
	}
}

void WardNNManaged::PushNativeParameters()
{
	initWard();

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

std::vector<std::string> WardNNManaged::getAttributes()
{
	return std::vector<std::string>();
}

std::map<std::string, void*> WardNNManaged::getOperations()
{
	std::map<std::string, void*> opers;
	opers["getAllWeights"]		= nnets_ward::getAllWeightsWard;
	opers["setAllWeights"]		= nnets_ward::setAllWeightsWard;
	opers["solve"]				= nnets_ward::solveWard;
	opers["getWeightsCount"]	= nnets_ward::getWeightsCountWard;
	opers["copySolver"]			= nnets_ward::copyWard;
	opers["freeSolver"]			= nnets_ward::freeWard;

	return opers;
}

void* WardNNManaged::getNativeSolver()
{
	return wsolver;
}

WardNNManaged::~WardNNManaged()
{
	delete[] x;
	delete[] y;
	delete wsolver;
}