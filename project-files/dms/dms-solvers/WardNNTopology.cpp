#include "WardNNTopology.h"

using namespace dms::solvers::neural_nets;

WardNNTopology::WardNNTopology(array<array<int>^>^ neurons, array<array<bool>^>^ delays, array<array<String^>^>^ afs,
	array<int>^ groups, array<int>^ additionalConnections, int layers)
{
	if ((neurons->Length != layers)  || 
		(delays->Length != (layers - 1)) ||
		(afs->Length != (layers - 1)) ||
		(groups->Length != (layers - 1)) ||
		(additionalConnections->Length != (layers - 2)) ||
		(layers < 2))
	{
		throw gcnew ArgumentException();
	}

	for (int i = 0; i < layers - 2; i++)
	{
		if (additionalConnections[i] > layers - 2 - i)
			throw gcnew ArgumentException();
	}

	for (int i = 0; i < layers - 1; i++)
	{
		if (groups[i] < 1)
			throw gcnew ArgumentException();
		if (delays[i]->Length != groups[i])
			throw gcnew ArgumentException();
		if (afs[i]->Length != groups[i])
			throw gcnew ArgumentException();
		if (groups[i] < 1)
			throw gcnew ArgumentException();
	}

	if (neurons[0]->Length != 1)
		throw gcnew ArgumentException();

	for (int i = 1; i < layers; i++)
	{
		if (neurons[i]->Length != groups[i-1])
			throw gcnew ArgumentException();

		for (int j = 0; j < groups[i-1]; j++)
		{
			if (neurons[i][j] < 1)
				throw gcnew ArgumentException();
		}
	}

	layersCount = layers;
	this->neurons = gcnew array<array<int>^>(layers);
	this->neurons[0] = gcnew array<int>(1);
	this->neurons[0][0] = neurons[0][0];
	for (int i = 1; i < layers; i++)
	{
		this->neurons[i] = gcnew array<int>(groups[i-1]);
		for (int j = 0; j < groups[i-1]; j++)
		{
			this->neurons[i][j] = neurons[i][j];
		}
	}

	this->delays = gcnew array<array<bool>^>(layers-1);
	this->afs = gcnew array<array<String^>^>(layers - 1);
	this->groupsCount = gcnew array<int>(layers - 1);
	for (int i = 0; i < layers - 1; i++)
	{
		this->delays[i] = gcnew array<bool>(groups[i]);
		this->afs[i] = gcnew array<String^>(groups[i]);
		this->groupsCount[i] = groups[i];
		for (int j = 0; j < groups[i]; j++)
		{
			this->delays[i][j] = delays[i][j];
			this->afs[i][j] = afs[i][j];
		}
	}
	this->addCons = gcnew array<int>(layers - 2);
	for (int i = 0; i < layers - 2; i++)
	{
		this->addCons[i] = additionalConnections[i];
	}
}

int WardNNTopology::getLayersCount()
{
	return layersCount;
}

int WardNNTopology::getAdditionalConnections(int* src)
{
	for (int i = 0; i < layersCount - 2; i++)
		src[i] = addCons[i];
	return layersCount - 2;
}

int WardNNTopology::getGroupsCount(int* src)
{
	for (int i = 0; i < layersCount - 1; i++)
		src[i] = groupsCount[i];
	return layersCount - 1;
}

int WardNNTopology::getActivateFunctionsTypes(ActivationFunctionType** src)
{
	int k = 0;
	for (int i = 0; i < layersCount - 1; i++)
	{
		for (int j = 0; j < groupsCount[i]; j++)
		{
			src[i][j] = ActivationFunctionTypes::getType(afs[i][j]);
		}
		k += groupsCount[i];
	}
	return k;
}

int WardNNTopology::getDelays(bool** src)
{
	int k = 0;
	for (int i = 0; i < layersCount - 1; i++)
	{
		for (int j = 0; j < groupsCount[i]; j++)
		{
			src[i][j] = delays[i][j];
		}
		k += groupsCount[i];
	}
	return k;
}

int WardNNTopology::getNeuronsCount(int** src)
{
	int k = 0;
	src[0][0] = neurons[0][0];
	for (int i = 1; i < layersCount; i++)
	{
		for (int j = 0; j < groupsCount[i-1]; j++)
		{
			src[i][j] = neurons[i][j];
		}
		k += groupsCount[i-1];
	}
	return k;
}

int WardNNTopology::getInputsCount()
{
	return neurons[0][0];
}

int WardNNTopology::getOutputsCount()
{
	int res = 0;
	for (int i = 0; i < groupsCount[layersCount - 2]; i++)
		res += neurons[layersCount - 1][i];
	return res;
}