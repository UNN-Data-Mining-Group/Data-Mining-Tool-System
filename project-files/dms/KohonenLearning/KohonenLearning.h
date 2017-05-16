#pragma once
#include <vector>
#include <map>

namespace nnets_kohonen_learning
{
	template <typename Type>
	using vector2d = std::vector<std::vector<Type>>;

	using getWinnerFunc = int(*)(void*);
	using addMultWeightsFunc = void(*)(int, float, float, const float*, void*);
	using getMaxNeuronIndexFunc = int(*)(void*);
	using setYFunc = void(*)(int, const float*, void*);
	using solveFunc = size_t(*)(const float*, float*, void*);
	using getWeightsFunc = const float* (*)(int, void*);
	using disableNeuronsFunc = void(*)(std::vector<int>, void*);
	using getDistanceFunc = int(*)(int, int, void*);
	using getWeightsMatrixSizeFunc = size_t(*)(void*);
	using setWeightsFunc = void(*)(const float*, void*);
	using setUseNormalizationFunc = void(*)(bool, void*);

	struct OperatorList
	{
		int(*getWinner)(void*);
		void(*addmultWeights)(int neuron, float alpha, float beta, const float* x, void* obj);
		int(*getMaxNeuronIndex)(void* obj);
		void(*setY)(int neuron, const float* y, void* obj);
		size_t(*solve)(const float* x, float* y, void* obj);
		const float* (*getWeights)(int neuron, void* obj);
		void(*disableNeurons)(std::vector<int> neurons, void* obj);
		int(*getDistance)(int neuron1, int neuron2, void* obj);
		size_t(*getWeightsMatrixSize)(void* obj);
		void(*setWeights)(const float* w, void* obj);
		void(*setUseNormalization)(bool norm, void* obj);
	};

	struct Selection
	{
		float **x, **y;
		int rowsCount;
		int xSize, ySize;

		Selection(float **_x, float **_y,
			int _rowsCount, int _xSize, int _ySize) :
			x(_x), y(_y), rowsCount(_rowsCount),
			xSize(_xSize), ySize(_ySize) {}
	};

	class ClassExtracter
	{
	public:
		ClassExtracter(OperatorList opers, float eps) : eps(eps), opers(opers) {}
		vector2d<float> getClasses();
		std::vector<int> getClassesDistributions(); //index - class number, value - count of elements in selection of this class
		std::map<int, int> getYClassMapping();
		vector2d<const float*> getXByClass();
		void fit(Selection s);
	private:
		OperatorList opers;
		float eps;
		vector2d<float> classes;
		std::vector<int> distrib;
		std::map<int, int> yToClass; //key - number of y in selection, 
									 //value - number of class
		vector2d<const float*> xByClass;

		bool is_equal(float* y, int size, std::vector<float> _class);
	};

	class KohonenSelfOrganizer
	{
	public:
		KohonenSelfOrganizer(OperatorList opers, int maxIter, int seed, float sigma0,
			float l0, float minLearningRate, float eps, bool normalize) :randomSeed(seed),
			maxIterations(maxIter), sigma0(sigma0), l0(l0),
			minLearningRate(minLearningRate), eps(eps),
			opers(opers), has_norm(normalize) {}

		float selfOrganize(Selection trainSel, void* trainedKn);
	private:
		OperatorList opers;
		int randomSeed;
		int maxIterations;
		float sigma0, l0, minLearningRate, eps;
		bool has_norm;

		float err;

		void initRandomWeights(void* trainedKn);
		vector2d<float> clasterize(void* trainedKn, Selection s, ClassExtracter& c_ext);
		void normalize(int ySize, void* trainedKn, ClassExtracter& c_extr, vector2d<float>& clasters);
	};

	class StatisticalPretrainer
	{
	public:
		StatisticalPretrainer(OperatorList opers, float eps) : eps(eps), opers(opers) {}
		virtual void pretrain(Selection s, void* trainedKn, int seed);
	private:
		OperatorList opers;
		float eps;
	};

	class KohonenClassifier
	{
	public:
		KohonenClassifier(OperatorList opers, bool hasPretrainer, float eps,
			int maxIterations, int seed, float learnA, float learnB,
			bool normalize) :hasPretrainer(hasPretrainer), eps(eps),
			maxIterations(maxIterations), seed(seed),
			learnA(learnA), learnB(learnB), normalize(normalize),
			opers(opers)
		{}

		float train(Selection trainSel, void* trainedKn);
	private:
		bool normalize;
		float eps;
		int maxIterations, seed;
		float learnA, learnB;
		bool hasPretrainer;
		OperatorList opers;

		bool is_equal(float* v1, float* v2, int size);
	};
}