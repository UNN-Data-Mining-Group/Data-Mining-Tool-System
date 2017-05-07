#include "KohonenNNManaged.h"
using namespace dms::solvers::neural_nets::kohonen;

KohonenManaged::KohonenManaged(KohonenNNTopology ^ t) : 
	INeuralNetwork(t)
{
	FetchNativeParameters();
}

KohonenManaged::KohonenManaged(KohonenManaged ^ k) :
	INeuralNetwork(k->topology)
{
	k->FetchNativeParameters();

	weights = gcnew array<float>(k->weights->Length);
	for (int i = 0; i < weights->Length; i++)
		weights[i] = k->weights[i];

	classes = gcnew array<array<float>^>(k->classes->Length);
	for (int i = 0; i < weights->Length; i++)
	{
		classes[i] = gcnew array<float>(k->classes[i]->Length);
		for (int j = 0; j < classes[i]->Length; j++)
			classes[i][j] = k->classes[i][j];
	}

	neurons = gcnew List<Tuple<int, int>^>();
	for (int i = 0; i < k->neurons->Count; i++)
		neurons->Add(gcnew Tuple<int, int>(k->neurons[i]->Item1, k->neurons[i]->Item2));

	use_normalization = k->use_normalization;
	PushNativeParameters();
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
	(*_opers)["getAllWeights"] = nnets_kohonen::getAllWeights;
	(*_opers)["copySolver"] = nnets_kohonen::copyKohonen;
	(*_opers)["freeSolver"] = nnets_kohonen::freeKohonen;
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

	float** cls = new float*[neurons->Count];
	for (int i = 0; i < neurons->Count; i++)
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

dms::solvers::ISolver ^ dms::solvers::neural_nets::kohonen::KohonenManaged::Copy()
{
	return gcnew KohonenManaged(this);
}

void KohonenManaged::setClasses(array<array<float>^>^ outputs)
{
	auto kn = static_cast<nnets_kohonen::KohonenNet*>(getNativeSolver());
	float** y = new float*[outputs->Length];
	for (int i = 0; i < outputs->Length; i++)
	{
		y[i] = new float[outputs[i]->Length];
		for (int j = 0; j < outputs[i]->Length; j++)
			y[i][j] = outputs[i][j];
	}

	kn->setClasses(y, outputs->Length);

	for (int i = 0; i < outputs->Length; i++)
		delete[] y[i];
	delete[] y;
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
				gcnew int2d(neurons[j]->Item1, neurons[j]->Item2),
				weights[j * GetInputsCount() + i]));
		}
	}

	int offset = GetInputsCount();
	for (int i = 0; i < GetOutputsCount(); i++)
	{
		res[i + offset] = gcnew List<Tuple<int2d^, double>^>();
		for (int j = 0; j < neurons->Count; j++)
		{
			res[i + offset]->Add(gcnew Tuple<int2d^, double>(
				gcnew int2d(neurons[j]->Item1, neurons[j]->Item2),
				classes[j][i]));
		}
	}
	return res;
}
