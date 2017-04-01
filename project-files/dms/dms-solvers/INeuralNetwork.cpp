#include "INeuralNetwork.h"

using dms::solvers::neural_nets::INeuralNetwork;
using dms::solvers::ITopology;

INeuralNetwork::INeuralNetwork(ITopology^ topology) : ISolver(topology)
{
	this->topology = topology;
	init();
}

void INeuralNetwork::init()
{
	solver = topology->createNativeSolver();
	x = new float[GetInputsCount()];
	y = new float[GetOutputsCount()];
	isInitialized = true;
}

void* INeuralNetwork::getNativeSolver() 
{ 
	if (isInitialized != true)
	{
		init();
		PushNativeParameters();
	}

	return solver; 
}

array<Single>^ INeuralNetwork::Solve(array<Single>^ x)
{
	if (isInitialized != true)
	{
		init();
		PushNativeParameters();
	}

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
		y[i] = this->y[i];
	return y;
}

INeuralNetwork::~INeuralNetwork()
{
	delete[] x;
	delete[] y;
	delete solver;
}