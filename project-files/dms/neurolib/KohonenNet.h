#pragma once
#include "NeuralNetwork.h"
#include <vector>

namespace nnets_kohonen
{
	struct int2d 
	{ 
		int x, y;
		int2d(int _x, int _y) : x(_x), y(_y) {}
	};

	struct int3d 
	{ 
		int x, y, z; 
		int3d(int _x, int _y, int _z) : x(_x), y(_y), z(_z) {}
	};

	struct NeuronIndex
	{
		int2d even_r;
		int3d cube;
		NeuronIndex(const int2d even_r) : even_r(even_r), cube(0, 0, 0)
		{
			int row = even_r.y;
			int col = even_r.x;
			cube.x = col - (row + (row & 1)) / 2;
			cube.z = row;
			cube.y = -cube.x - cube.z;
		}
		NeuronIndex(const int3d cube) : cube(cube), even_r(0, 0)
		{
			even_r.x = cube.x + (cube.z - (cube.z & 1)) / 2;
			even_r.y = cube.z;
		}
		int distanceTo(const NeuronIndex& n)
		{
			return (std::abs(cube.x - n.cube.x) +
				std::abs(cube.y - n.cube.y) + std::abs(cube.z - n.cube.z)) / 2;
		}
		bool operator==(const NeuronIndex& n)
		{
			return (even_r.x == n.even_r.x) && (even_r.y == n.even_r.y);
		}
	};

	size_t solve(const float* x, float* y, void* obj);
	size_t getWeightsMatrixSize(void* obj);
	void setWeights(const float* w, void* obj);
	void setUseNormalization(bool norm, void* obj);
	size_t getAllWeights(float* w, void* obj);

	int getMaxNeuronIndex(void* obj);
	int getWinner(void* obj);
	int getDistance(int neuron1, int neuron2, void* obj);
	void addmultWeights(int neuron, float alpha, float beta, const float* x, void* obj);
	const float* getWeights(int neuron, void* obj);
	void setY(int neuron, const float* y, void* obj);
	void disableNeurons(std::vector<int> neurons, void* obj);
	void* copyKohonen(void* obj);
	void freeKohonen(void*& obj);

	class KohonenNet : public nnets::NeuralNetwork
	{
	public:
		enum Metric {Default, Euclidean};
		enum ClassInitializer {Statistical, Evenly, Revert};

		KohonenNet(KohonenNet& kn);
		KohonenNet(int inputs_count, int outputs_count, 
			int koh_width, int koh_height, 
			float classEps,
			ClassInitializer initializer, 
			Metric metric = Default);

		size_t solve(const float* x, float* y) override;
		size_t getInputsCount() override;
		size_t getOutputsCount() override;

		void setWeights(const float* weights);
		void setClasses(float** classes);
		void setClasses(float** y, int rowsCount);	//init classes by train output vectors
		void setNeurons(std::vector<NeuronIndex> &neurons);
		void setClass(NeuronIndex n, const float* y);
		void setClassEps(float eps);
		void setUseNormalization(bool norm);
		size_t getClasses(float** classes);
		float getClassEps();
		size_t getWeights(float* weights);
		std::vector<NeuronIndex> getNeurons();
		size_t getClass(NeuronIndex n, float* y);
		size_t getWeightsMatrixSize();
		bool getUseNormalization();
		int getWinnerIndex();

		virtual ~KohonenNet();
	private:
		bool use_norm_x;
		float class_eps;
		Metric metric;
		ClassInitializer initializer;
		std::vector<NeuronIndex> neuron_index_map;	//index - number of neuron data 
													//in kohonen_layer and weights
													//value - geometrical position in layer
		float* x_internal;

		float* weights;
		float** classes;
		float* kohonen_layer;
		int neurons_width, neurons_height, x_size, y_size;
		NeuronIndex winner;

		NeuronIndex calcWinner(const float* x);
		int getInternalIndex(NeuronIndex n);
		void initByNeuronMap(std::vector<NeuronIndex> map);

		friend int getWinner(void* obj);
		friend int getMaxNeuronIndex(void* obj);
		friend void addmultWeights(int neuron, float alpha, float beta, const float* x, void* obj);
		friend void setY(int neuron, const float* y, void* obj);
		friend const float* getWeights(int neuron, void* obj);
		friend void disableNeurons(std::vector<int> neurons, void* obj);
		friend int getDistance(int neuron1, int neuron2, void* obj);
		friend size_t getAllWeights(float* w, void* obj);
	};
}