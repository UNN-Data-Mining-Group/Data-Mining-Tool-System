#include "WardNNManaged.h"

using namespace dms::solvers::neural_nets::ward_net;

WardNNManaged::WardNNManaged(WardNNTopology^ t, array<array<float>^>^ weights) : 
	ISolver(t->GetInputsCount(), t->GetOutputsCount())
{
	x = new float[GetInputsCount()];
	y = new float[GetOutputsCount()];
	
	InputLayer^ input = t->GetInputLayer();
	List<Layer^>^ layers = t->GetLayers();

	this->weights = weights;
	float** w = new float*[weights->GetLength(0)];
	for (int i = 0; i < weights->GetLength(0); i++)
	{
		w[i] = new float[weights[i]->Length];
		for (int j = 0; j < weights[i]->Length; j++)
		{
			w[i][j] = weights[i][j];
		}
	}

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
	
		wsolver = new nnets_ward::WardNN(native_input, native_layers, w);
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
	finally
	{
		for (int i = 0; i < weights->GetLength(0); i++)
			delete[] w[i];
		delete[] w;
	}

	if (isCreationSuccessfull == false)
	{
		throw gcnew System::Exception(exMessage);
	}
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