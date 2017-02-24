#pragma once
#include "NeuralNetwork.h"
#include <vector>

namespace nnets_conv
{
	enum class LayerType
	{
		Convolution,
		Pooling,
		FullyConnected
	};

	struct Layer
	{
		LayerType Type;
		Layer(LayerType type) : Type(type) {}
	};

	struct FullyConnectedLayer : public Layer
	{
		int NeuronsCount;
		nnets::ActivationFunctionType ActivationFunction;

		FullyConnectedLayer(int neurons, nnets::ActivationFunctionType af) :
			Layer(LayerType::FullyConnected), 
			NeuronsCount(neurons), 
			ActivationFunction(af) {}
	};

	struct PoolingLayer : public Layer
	{
		int FilterWidth, FilterHeight;
		int StrideWidth, StrideHeight;

		PoolingLayer(int fw, int fh, int sw, int sh) :
			Layer(LayerType::Pooling), 
			FilterWidth(fw), FilterHeight(fh), 
			StrideWidth(sw), StrideHeight(sh) {}
	};

	struct ConvolutionLayer : public PoolingLayer
	{
		int Padding;
		int CountFilters;
		nnets::ActivationFunctionType ActivationFunction;

		ConvolutionLayer(int fw, int fh, int sw, int sh,
			int p, int count_f, nnets::ActivationFunctionType af) :
			PoolingLayer(fw, fh, sw, sh), 
			Padding(p), CountFilters(count_f), ActivationFunction(af) 
		{
			Type = LayerType::Convolution;
		}
	};

	class ConvNN : public nnets::NeuralNetwork
	{
	public: 
		//w - number of neurons in output volume by width
		//h - by height
		//d - by depth
		ConvNN(int w, int h, int d, const std::vector<Layer*>& layers, float** weights);

		size_t solve(const float* x, float* y) override;
		size_t getInputsCount() override;
		size_t getOutputsCount() override;

		~ConvNN() { freeMemory(); }
	private:

		enum class VolumeType {Convolutional, Activation, Pooling, FullyConnected, Simple};

		struct Volume;
		struct VolumeActivation;
		struct VolumePooling;
		struct VolumeConvolutional;

		void clearPtrs();
		void freeMemory();
		int calcOutputVolume(int input, int filter, int padding, int stride);	//returns dimention of volume based on its parameters and dimention of previous volume
		
		void im2col(const float* source, const int channels,
			const int height, const int width, const int kernel_h, const int kernel_w,
			const int stride_h, const int stride_w,
			const int padding, float* dest);
		void pool_max(const float* source, const int channels,
			const int height, const int width, const int kernel_h, const int kernel_w,
			const int stride_h, const int stride_w, float* dest);

		int volumesCount;
		int weightsCount;
		size_t deconvSize;

		Volume** volumes;
		float* deconvMatrix;				//deconvolved volume for fast conv-operation
		float** weights;					//weights of convolutional and fully connected volumes
	};
}

