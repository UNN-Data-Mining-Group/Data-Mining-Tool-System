#pragma once
#include <vector>
#include <map>
#include "KohonenNet.h"

namespace nnets_kohonen
{
	template <typename Type>
	using vector2d = std::vector<std::vector<Type>>;

	struct Selection
	{
		float **x, **y;
		int rowsCount;
		int xSize, ySize;

		Selection(float **_x, float **_y,
			int _rowsCount, int _xSize, int _ySize) :
			x(_x), y(_y), rowsCount(_rowsCount),
			xSize(_xSize), ySize(_ySize) {}

		std::vector<int> getMixIndexes(float seed);
	};

	class ClassExtracter
	{
	public:
		ClassExtracter(float eps) : eps(eps) {}
		vector2d<float> getClasses();
		std::vector<int> getClassesDistributions(); //index - class number, value - count of elements in selection of this class
		std::map<int, int> getYClassMapping();
		vector2d<const float*> getXByClass();
		void fit(Selection s);
	private:
		float eps;
		vector2d<float> classes;
		std::vector<int> distrib;
		std::map<int, int> yToClass; //key - number of y in selection, 
									 //value - number of class
		vector2d<const float*> xByClass;

		bool is_equal(float* y, int size, std::vector<float> _class);
	};

	class IPretrainer
	{
	public:
		virtual void pretrain(Selection s, KohonenNet* trainedKn, int seed) = 0;
		virtual ~IPretrainer() {}
	};

	class KohonenSelfOrganizer : public IPretrainer
	{
	public:
		KohonenSelfOrganizer(int maxIter, int seed, float sigma0,
			float l0, float minLearningRate, float eps) :randomSeed(seed),
			maxIterations(maxIter), sigma0(sigma0), l0(l0),
			minLearningRate(minLearningRate), eps(eps)
		{}
		~KohonenSelfOrganizer() {}

		void selfOrganize(Selection trainSel, KohonenNet* trainedKn);
		virtual void pretrain(Selection s, KohonenNet* trainedKn, int seed) override;
	private:
		int randomSeed;
		int maxIterations;
		float sigma0, l0, minLearningRate, eps;

		void initRandomWeights(KohonenNet* trainedKn);
		vector2d<float> clasterize(KohonenNet* trainedKn, Selection s, ClassExtracter& c_ext);
		void normalize(int ySize, KohonenNet* trainedKn, ClassExtracter& c_extr, vector2d<float>& clasters);
	};

	class StatisticalPretrainer : public IPretrainer
	{
	public:
		StatisticalPretrainer(float eps) : eps(eps) {}
		~StatisticalPretrainer() {}

		virtual void pretrain(Selection s, KohonenNet* trainedKn, int seed) override;
	private:
		float eps;
	};
}
