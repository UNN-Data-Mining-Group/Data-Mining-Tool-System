#include "KohonenNNManaged.h"
using namespace dms::solvers::neural_nets::kohonen;

KohonenManaged::KohonenManaged(KohonenNNTopology ^ t) : 
	INeuralNetwork(t)
{
	this->t = t;
	FetchNativeParameters();
}

void * dms::solvers::neural_nets::kohonen::KohonenManaged::getAttributes()
{
	return _attr;
}

void* KohonenManaged::getOperations()
{
	(*_opers)["addmultWeights"] = nnets_kohonen::addmultWeights;
	(*_opers)["disableNeurons"] = nnets_kohonen::disableNeurons;
	(*_opers)["getDistance"] = nnets_kohonen::getDistance;
	(*_opers)["getMaxNeuronIndex"] = nnets_kohonen::getMaxNeuronIndex;
	(*_opers)["getNeuronWeighs"] = nnets_kohonen::getWeights;
	(*_opers)["getWeightsCount"] = nnets_kohonen::getWeightsMatrixSize;
	(*_opers)["getWinner"] = nnets_kohonen::getWinner;
	(*_opers)["setUseNormalization"] = nnets_kohonen::setUseNormalization;
	(*_opers)["setAllWeights"] = nnets_kohonen::setWeights;
	(*_opers)["setY"] = nnets_kohonen::setY;
	(*_opers)["solve"] = nnets_kohonen::solve;
	return _opers;
}

void KohonenManaged::FetchNativeParameters()
{
	auto p_solver = static_cast<nnets_kohonen::KohonenNet*>(getNativeSolver());

	int y_size = GetOutputsCount();
	int w_size = p_solver->getWeightsMatrixSize();
	float* w = new float[w_size];
	p_solver->getWeights(w);
	weights = gcnew array<float>(w_size);
	for (int i = 0; i < w_size; i++)
		weights[i] = w[i];
	delete[] w;

	std::vector<nnets_kohonen::NeuronIndex> ns = p_solver->getNeurons();
	neurons = gcnew List<Tuple<int, int>^>();

	float** cls = new float*[ns.size()];
	for (int i = 0; i < ns.size(); i++)
	{
		neurons->Add(gcnew Tuple<int, int>(ns[i].even_r.x, ns[i].even_r.y));
		cls[i] = new float[y_size];
	}

	p_solver->getClasses(cls);
	classes = gcnew array<array<float>^>(ns.size());
	for (int i = 0; i < ns.size(); i++)
	{
		classes[i] = gcnew array<float>(y_size);
		for (int j = 0; j < y_size; j++)
			classes[i][j] = cls[i][j];
		delete[] cls[i];
	}
	delete[] cls;

	use_normalization = p_solver->getUseNormalization();
}

void KohonenManaged::PushNativeParameters()
{
	auto p_solver = static_cast<nnets_kohonen::KohonenNet*>(getNativeSolver());

	int y_size = GetOutputsCount();
	int w_size = p_solver->getWeightsMatrixSize();
	float* w = new float[w_size];
	for (int i = 0; i < w_size; i++)
		w[i] = weights[i];
	p_solver->setWeights(w);
	delete[] w;

	std::vector<nnets_kohonen::NeuronIndex> ns;

	float** cls = new float*[ns.size()];
	for (int i = 0; i < ns.size(); i++)
	{
		Tuple<int, int>^ current = neurons[i];
		ns.push_back(
			nnets_kohonen::NeuronIndex(
				nnets_kohonen::int2d{ current->Item1, current->Item2 }));
		cls[i] = new float[y_size];
		for (int j = 0; j < y_size; j++)
			cls[i][j] = classes[i][j];
	}
	p_solver->setNeurons(ns);
	p_solver->setClasses(cls);
	p_solver->setUseNormalization(use_normalization);

	for (int i = 0; i < ns.size(); i++)
		delete[] cls[i];
	delete[] cls;
}

array<List<Tuple<int2d^, double>^>^>^ KohonenManaged::GetVisualData()
{
	array<List<Tuple<int2d^, double>^>^>^ res =
		gcnew array<List<Tuple<int2d^, double>^>^>(GetInputsCount() + GetOutputsCount());
	for (int i = 0; i < GetInputsCount(); i++)
	{
		res[i] = gcnew List<Tuple<int2d^, double>^>();
		for (int j = 0; j < neurons->Count; j++)
		{
			res[i]->Add(gcnew Tuple<int2d^, double>(
				gcnew int2d(neurons[i]->Item1, neurons[i]->Item2),
				weights[j * GetInputsCount() + i]));
		}
	}

	int offset = GetInputsCount();
	for (int i = 0; i < GetOutputsCount(); i++)
	{
		res[i + offset] = gcnew List<Tuple<int2d^, double>^>();
		for (int j = 0; j < neurons->Count; j++)
		{
			res[i]->Add(gcnew Tuple<int2d^, double>(
				gcnew int2d(neurons[i]->Item1, neurons[i]->Item2),
				classes[j][i]));
		}
	}
	return res;
}
