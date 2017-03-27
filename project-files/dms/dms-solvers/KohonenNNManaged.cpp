#include "KohonenNNManaged.h"
using namespace dms::solvers::neural_nets::kohonen;

KohonenManaged::KohonenManaged(KohonenNNTopology ^ t) : 
	INeuralNetwork(t->GetInputsCount(), t->GetOutputsCount())
{
	this->t = t;
	nnets_kohonen::KohonenNet::Metric m;
	String^ m_str = t->GetMetric();
	if (m_str->Equals("Default"))
		m = nnets_kohonen::KohonenNet::Default;
	else if (m_str->Equals("Euclidean"))
		m = nnets_kohonen::KohonenNet::Euclidean;
	else throw gcnew System::ArgumentException();

	solver = new nnets_kohonen::KohonenNet(t->GetInputsCount(),
		t->GetOutputsCount(), t->GetLayerWidth(), t->GetLayerHeight(), m);
	x = new float[GetInputsCount()];
	y = new float[GetOutputsCount()];
	
	FetchNativeParameters();
}

array<Single>^ KohonenManaged::Solve(array<Single>^ x)
{
	__int64 inputs = GetInputsCount();
	__int64 outputs = GetOutputsCount();

	if (x->Length != inputs)
		throw gcnew System::ArgumentException();

	for (int i = 0; i < inputs; i++)
		this->x[i] = x[i];

	if (outputs != solver->solve(this->x, this->y))
		throw gcnew System::IndexOutOfRangeException();

	array<Single>^ y = gcnew array<Single>(outputs);
	for (int i = 0; i < outputs; i++)
	{
		y[i] = this->y[i];
	}
	return y;
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

void * KohonenManaged::getNativeSolver()
{
	return solver;
}

void KohonenManaged::FetchNativeParameters()
{
	int y_size = GetOutputsCount();
	int w_size = solver->getWeightsMatrixSize();
	float* w = new float[w_size];
	solver->getWeights(w);
	weights = gcnew array<float>(w_size);
	for (int i = 0; i < w_size; i++)
		weights[i] = w[i];
	delete[] w;

	std::vector<nnets_kohonen::NeuronIndex> ns = solver->getNeurons();
	neurons = gcnew List<Tuple<int, int>^>();

	float** cls = new float*[ns.size()];
	for (int i = 0; i < ns.size(); i++)
	{
		neurons->Add(gcnew Tuple<int, int>(ns[i].even_r.x, ns[i].even_r.y));
		cls[i] = new float[y_size];
	}

	solver->getClasses(cls);
	classes = gcnew array<array<float>^>(ns.size());
	for (int i = 0; i < ns.size(); i++)
	{
		classes[i] = gcnew array<float>(y_size);
		for (int j = 0; j < y_size; j++)
			classes[i][j] = cls[i][j];
		delete[] cls[i];
	}
	delete[] cls;

	use_normalization = solver->getUseNormalization();
}

void KohonenManaged::PushNativeParameters()
{
	int y_size = GetOutputsCount();
	int w_size = solver->getWeightsMatrixSize();
	float* w = new float[w_size];
	for (int i = 0; i < w_size; i++)
		w[i] = weights[i];
	solver->setWeights(w);
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
	solver->setNeurons(ns);
	solver->setClasses(cls);
	solver->setUseNormalization(use_normalization);

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

KohonenManaged::~KohonenManaged()
{
	delete[] x;
	delete[] y;
	delete solver;
}
