#pragma once
#include "NeuralNetwork.h"
#include <vector>

namespace nnets_ward
{
	size_t getAllWeightsWard(float* &dest, void* obj);
	void setAllWeightsWard(const float* src, void* obj);
	size_t solveWard(const float* x, float* y, void* obj);
	size_t getWeightsCountWard(void* obj);
	void* copyWard(void* obj);
	void freeWard(void* &obj);

	struct InputLayer
	{
		size_t NeuronsCount;
		size_t ForwardConnection;

		InputLayer(size_t neuronsCount, size_t forwardConnection) :
			NeuronsCount(neuronsCount), ForwardConnection(forwardConnection) {}
	};

	struct NeuronsGroup
	{
		size_t NeuronsCount;
		bool HasDelay;
		nnets::ActivationFunctionType ActivationFunction;
		NeuronsGroup(size_t neurons, bool hasDelay, nnets::ActivationFunctionType af) :
			NeuronsCount(neurons), HasDelay(hasDelay), ActivationFunction(af) {}
	};

	struct Layer
	{
		std::vector<NeuronsGroup> Groups;
		size_t ForwardConnection;	//indicates, that this layer has additional connection 
									//to layer stayed forward on (1+ForwardConnection) regarding this.
									//If ForwardConnection = 0, there is only connection to next layer.
		Layer(size_t forwardConnection, const std::vector<NeuronsGroup> &groups) :
			ForwardConnection(forwardConnection), Groups(groups) {}
	};

	class WardNN : public nnets::NeuralNetwork
	{
	public:
		WardNN(const WardNN& wnn);
		WardNN(InputLayer input, const std::vector<Layer> layers, float** weights);
		
		size_t solve(const float* x, float* y) override;
		size_t getInputsCount() override;
		size_t getOutputsCount() override;

		~WardNN();
	private:
		struct Layer;	//internal representation of layers

		std::vector<Layer> layers;
		size_t* w_sizes;
		float** w;
		float* buf_x;

		void alloc_data(float** weights);	//allocation w and buf_x

		friend size_t getAllWeightsWard(float* &dest, void* obj);
		friend void setAllWeightsWard(const float* src, void* obj);
		friend size_t getWeightsCountWard(void* obj);
	};
}
