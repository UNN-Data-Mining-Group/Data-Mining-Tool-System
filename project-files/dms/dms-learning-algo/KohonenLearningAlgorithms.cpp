#include "KohonenLearningAlgorithms.h"
#include <map>
#include <string>
using namespace dms::kohonen_learning_algorithms;
using namespace dms::solvers::neural_nets::kohonen;

KohonenClassifier::KohonenClassifier()
{
	paramNames = gcnew array<System::String^>(7);
	paramNames[0] = "Предобучение";
	paramNames[1] = "Точность сравнений";
	paramNames[2] = "Макс. количество итераций";
	paramNames[3] = "Сид";
	paramNames[4] = "Скорость обучения (числитель)";
	paramNames[5] = "Скорость обучения (знаменатель)";
	paramNames[6] = "Нормализация выборки";

	paramValues = gcnew array<float>(7);
	paramValues[0] = 1.0f;
	paramValues[1] = 1e-5f;
	paramValues[2] = 1000.0f;
	paramValues[3] = 27.0f;
	paramValues[4] = 1.0f;
	paramValues[5] = 3.0f;
	paramValues[6] = 1.0f;
}

array<System::String^>^ KohonenClassifier::getParamsNames()
{
	return paramNames;
}

array<float>^ KohonenClassifier::getParams()
{
	return paramValues;
}

bool KohonenClassifier::canTrain(dms::solvers::ISolver ^ solver)
{
	auto map = static_cast<std::map<std::string, void*>*>(solver->getOperations());
	constexpr int count_funcs = 6;
	std::string required_funcs[] =
	{
		"getWinner", "addmultWeights", "getMaxNeuronIndex",
		"setY", "solve", "setUseNormalization"
	};
	int exist_funcs = 0;
	for(int i = 0; i < count_funcs; i++)
		if(map->count(required_funcs[i]) == 0)
			return false;
	return true;
}

System::String ^ KohonenClassifier::getType()
{
	return "Векторное квантование";
}

float KohonenClassifier::learn(dms::solvers::ISolver ^ solver, 
	array<array<float>^>^ train_x, array<float>^ train_y)
{
	if (canTrain(solver) == false)
		throw gcnew System::ArgumentException("Invalid solver");

	auto map = static_cast<std::map<std::string, void*>*>(solver->getOperations());
	auto funcs = initFuncList(map);
	auto sel = initSelection(train_x, train_y);

	bool hasPretrainer = paramValues[0] == 1.0f;
	float eps = paramValues[1];
	int maxIter = System::Convert::ToInt32(paramValues[2]);
	int seed = System::Convert::ToInt32(paramValues[3]);
	float A = paramValues[4];
	float B = paramValues[5];
	bool normalize = paramValues[6] == 1.0f;

	auto native_classifier = 
		new nnets_kohonen_learning::KohonenClassifier(funcs, hasPretrainer,
			eps, maxIter, seed, A, B, normalize);

	using dms::solvers::neural_nets::INeuralNetwork;
	INeuralNetwork^ nn = static_cast<INeuralNetwork^>(solver);
	KohonenManaged^ kn = dynamic_cast<KohonenManaged^>(nn);
	if (kn != nullptr)
	{
		array<array<float>^>^ out = gcnew array<array<float>^>(train_y->Length);
		for (int i = 0; i < train_y->Length; i++)
		{
			out[i] = gcnew array<float>(1);
			out[i][0] = train_y[i];
		}
		kn->setClasses(out);
	}

	float err_train = native_classifier->train(sel, nn->getNativeSolver());
	nn->FetchNativeParameters();

	freeSelection(sel);
	delete native_classifier;
	return err_train;
}

nnets_kohonen_learning::OperatorList KohonenTrainer::initFuncList(
	std::map<std::string, void*>* map)
{
	using namespace nnets_kohonen_learning;

	nnets_kohonen_learning::OperatorList funcs;

	funcs.addmultWeights = static_cast<addMultWeightsFunc>(map->at("addmultWeights"));
	funcs.disableNeurons = static_cast<disableNeuronsFunc>(map->at("disableNeurons"));
	funcs.getDistance = static_cast<getDistanceFunc>(map->at("getDistance"));
	funcs.getMaxNeuronIndex = static_cast<getMaxNeuronIndexFunc>(map->at("getMaxNeuronIndex"));
	funcs.getWeights = static_cast<getWeightsFunc>(map->at("getNeuronWeighs"));
	funcs.getWeightsMatrixSize = static_cast<getWeightsMatrixSizeFunc>(map->at("getWeightsCount"));
	funcs.getWinner = static_cast<getWinnerFunc>(map->at("getWinner"));
	funcs.setUseNormalization = static_cast<setUseNormalizationFunc>(map->at("setUseNormalization"));
	funcs.setWeights = static_cast<setWeightsFunc>(map->at("setAllWeights"));
	funcs.setY = static_cast<setYFunc>(map->at("setY"));
	funcs.solve = static_cast<solveFunc>(map->at("solve"));

	return funcs;
}

nnets_kohonen_learning::Selection KohonenTrainer::initSelection(
	array<array<float>^>^ train_x, array<float>^ train_y)
{
	int rows = train_x->Length;
	int xSize = train_x[0]->Length;
	int ySize = 1;

	float** x = new float*[rows];
	float** y = new float*[rows];

	for (int i = 0; i < rows; i++)
	{
		x[i] = new float[xSize];
		y[i] = new float[ySize];

		for (int j = 0; j < xSize; j++)
			x[i][j] = train_x[i][j];
		for (int j = 0; j < ySize; j++)
			y[i][j] = train_y[i];
	}

	return nnets_kohonen_learning::Selection(x, y, rows, xSize, ySize);
}

void KohonenTrainer::freeSelection(nnets_kohonen_learning::Selection & sel)
{
	for (int i = 0; i < sel.rowsCount; i++)
	{
		delete[] sel.x[i];
		delete[] sel.y[i];
	}
	delete[] sel.x; sel.x = nullptr;
	delete[] sel.y; sel.y = nullptr;
	sel.rowsCount = 0;
	sel.xSize = 0;
	sel.ySize = 0;
}

KohonenSelfOrganizer::KohonenSelfOrganizer()
{
	paramNames = gcnew array<System::String^>(7);
	paramNames[0] = "Макс. количество итераций";
	paramNames[1] = "Сид";
	paramNames[2] = "Дисперсия соседей (sigma0)";
	paramNames[3] = "Скорость обучения (начальная)";
	paramNames[4] = "Скорость обучения (минимальная)";
	paramNames[5] = "Точность сравнений";
	paramNames[6] = "Нормализация выборки";

	paramValues = gcnew array<float>(7);
	paramValues[0] = 100.0f;
	paramValues[1] = 27.0f;
	paramValues[2] = 1.0f;
	paramValues[3] = 0.1f;
	paramValues[4] = 0.0001f;
	paramValues[5] = 1e-5f;
	paramValues[6] = 1.0f;
}

bool KohonenSelfOrganizer::canTrain(dms::solvers::ISolver ^ solver)
{
	auto map = static_cast<std::map<std::string, void*>*>(solver->getOperations());
	constexpr int count_funcs = 11;
	std::string required_funcs[count_funcs] =
	{
		"getWinner", "addmultWeights", "getMaxNeuronIndex",
		"setY", "solve", "getNeuronWeighs", "disableNeurons",
		"getDistance", "getWeightsCount", "setAllWeights",
		"setUseNormalization"
	};
	int exist_funcs = 0;
	for (int i = 0; i < count_funcs; i++)
		if (map->count(required_funcs[i]) == 0)
			return false;
	return true;
}

float KohonenSelfOrganizer::learn(dms::solvers::ISolver ^ solver, 
	array<array<float>^>^ train_x, array<float>^ train_y)
{
	if (canTrain(solver) == false)
		throw gcnew System::ArgumentException("Invalid solver");

	auto map = static_cast<std::map<std::string, void*>*>(solver->getOperations());
	auto funcs = initFuncList(map);
	auto sel = initSelection(train_x, train_y);

	int maxIter = System::Convert::ToInt32(paramValues[0]);
	int seed = System::Convert::ToInt32(paramValues[1]);
	float sigma0 = paramValues[2];
	float l0 = paramValues[3];
	float minLR = paramValues[4];
	float eps = paramValues[5];
	bool norm = paramValues[6] == 1.0f;

	auto native_selforg =
		new nnets_kohonen_learning::KohonenSelfOrganizer(funcs,
			maxIter, seed, sigma0, l0, minLR, eps, norm);

	using dms::solvers::neural_nets::INeuralNetwork;
	INeuralNetwork^ nn = static_cast<INeuralNetwork^>(solver);
	KohonenManaged^ kn = dynamic_cast<KohonenManaged^>(nn);
	if (kn != nullptr)
	{
		array<array<float>^>^ out = gcnew array<array<float>^>(train_y->Length);
		for (int i = 0; i < train_y->Length; i++)
		{
			out[i] = gcnew array<float>(1);
			out[i][0] = train_y[i];
		}
		kn->setClasses(out);
	}

	float err_train = native_selforg->selfOrganize(sel, nn->getNativeSolver());
	nn->FetchNativeParameters();

	freeSelection(sel);
	delete native_selforg;
	return err_train;
}
