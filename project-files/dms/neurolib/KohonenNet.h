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
	};

	NeuronIndex getWinner(const float* x, void* obj);
	std::vector<NeuronIndex> getNeighbours(const NeuronIndex n, int radius, void* obj);
	void addmultWeights(const NeuronIndex n, float alpha, float beta, float* x, void* obj);
	int2d getNetDimention(void* obj);
	void setY(NeuronIndex n, const float* y, void* obj);
	size_t solve(const float* x, float* y, void* obj);

	class KohonenNet : public nnets::NeuralNetwork
	{
	public:
		KohonenNet(KohonenNet& kn);
		KohonenNet(int inputs_count, int outputs_count, 
			int koh_width, int koh_height, bool use_normalization = false);

		size_t solve(const float* x, float* y) override;
		size_t getInputsCount() override;
		size_t getOutputsCount() override;
		void setWeights(float* weights);
		void setClass(NeuronIndex n, const float* y);
		void setUseNormalization(bool norm);
		size_t getWeights(float* weights);
		size_t getClass(NeuronIndex n, float* y);
		size_t getWeightsMatrixSize();

		~KohonenNet();
	private:
		bool use_norm_x;
		float* x_internal;

		float* weights;
		float** classes;
		float* kohonen_layer;
		int neurons_width, neurons_height, x_size, y_size;

		NeuronIndex getWinner(const float* x);
		friend NeuronIndex getWinner(const float* x, void* obj);
		friend int2d getNetDimention(void* obj);
		friend void addmultWeights(const NeuronIndex n, float alpha, float beta, float* x, void* obj);
	};
}