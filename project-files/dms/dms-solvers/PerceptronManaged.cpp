#include "PerceptronManaged.h"

using namespace dms::solvers::neural_nets;
using namespace System::Runtime::InteropServices;

PerceptronManaged::PerceptronManaged(PerceptronTopology^ t) : 
	ISolver(t->GetInputsCount(), t->GetOutputsCount())
{
	int layers = t->GetLayersCount();

	auto ns = t->GetNeuronsInLayersCount();
	auto hds = t->HasLayersDelayWeight();
	auto afs = t->GetLayersActivateFunctions();

	activateFunctions = gcnew array<oper_af^>(layers - 1);
	Random^ r = gcnew Random();
	weights = gcnew array<array<float>^>(layers - 1);
	for (int i = 0; i < layers - 1; i++)
	{
		weights[i] = gcnew array<float>(ns[i + 1] * (ns[i] + hds[i]));
		for (int j = 0; j < weights[i]->Length; j++)
		{
			weights[i][j] = (float)r->NextDouble() * 2.0f - 1.0f;
		}
	}

	int* neurons = new int[layers];
	bool* delays = new bool[layers - 1];
	neurolib::oper_af* ptr_actfunc = new neurolib::oper_af[layers - 1];

	float** w = new float*[layers - 1];
	for (int i = 0; i < layers - 1; i++)
	{
		int dim = ns[i + 1] * (ns[i] + hds[i]);
		w[i] = new float[dim];
		for (int j = 0; j < dim; j++)
			w[i][j] = weights[i][j];
	}

	for (int i = 0; i < layers; i++)
		neurons[i] = ns[i];
	for (int i = 0; i < layers - 1; i++)
	{
		delays[i] = hds[i];

		activateFunctions[i] = gcnew oper_af(afs[i], &IActivateFunction::getResult);
		IntPtr p = Marshal::GetFunctionPointerForDelegate(activateFunctions[i]);
		ptr_actfunc[i] = static_cast<neurolib::oper_af>(p.ToPointer());
	}

	psolver = new neurolib::Perceptron(neurons, delays, ptr_actfunc, layers, w);
	x = new float[GetInputsCount()];
	y = new float[GetOutputsCount()];

	delete[] neurons;
	delete[] delays;
	delete[] ptr_actfunc;

	for (int i = 0; i < layers - 1; i++)
		delete[] w[i];
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

PerceptronManaged::~PerceptronManaged()
{
	delete[] x;
	delete[] y;

	delete psolver;
	psolver = nullptr;
}