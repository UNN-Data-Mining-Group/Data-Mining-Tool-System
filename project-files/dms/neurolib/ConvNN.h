#pragma once
#include "ActivationFunctions.h"
#include <vector>

namespace neurolib
{
	enum class ConvNNLayerType
	{
		Convolution,
		Pooling,
		FullyConnected
	};

	struct ConvNNLayer
	{
		ConvNNLayerType Type;
		ConvNNLayer(ConvNNLayerType type) : Type(type) {}
	};

	struct ConvNNFullyConnectedLayer : public ConvNNLayer
	{
		int NeuronsCount;
		ActivationFunctionType ActivationFunction;

		ConvNNFullyConnectedLayer(int neurons, ActivationFunctionType af) :
			ConvNNLayer(ConvNNLayerType::FullyConnected), NeuronsCount(neurons), ActivationFunction(af) {}
	};

	struct ConvNNPoolingLayer : public ConvNNLayer
	{
		int FilterWidth, FilterHeight;
		int StrideWidth, StrideHeight;

		ConvNNPoolingLayer(int fw, int fh, int sw, int sh) :
			ConvNNLayer(ConvNNLayerType::Pooling), FilterWidth(fw), FilterHeight(fh), StrideWidth(sw), StrideHeight(sh) {}
	};

	struct ConvNNConvolutionLayer : public ConvNNPoolingLayer
	{
		int Padding;
		int CountFilters;
		ActivationFunctionType ActivationFunction;

		ConvNNConvolutionLayer(int fw, int fh, int sw, int sh,
			int p, int count_f, ActivationFunctionType af) :
			ConvNNPoolingLayer(fw, fh, sw, sh), 
			Padding(p), CountFilters(count_f), ActivationFunction(af) 
		{
			Type = ConvNNLayerType::Convolution;
		}
	};

	class ConvNN
	{
	public: 
		//w - number of neurons in output volume by width
		//h - by height
		//d - by depth
		ConvNN(int w, int h, int d, const std::vector<ConvNNLayer*>& layers, float** weights);

		int solve(float* x, float* y);
		int getInputsCount();
		int getOutputsCount();

		~ConvNN() { freeMemory(); }
	private:

		enum class VolumeType {Convolutional, Activation, Pooling, FullyConnected, Simple};

		struct Volume
		{
			int Width, Height, Depth;
			float* Values;
			VolumeType Type;

			Volume(int w, int h, int d, VolumeType type)
			{
				Width = w;	Height = h;	Depth = d;
				Values = new float[Width * Height * Depth];
				Type = type;
			}
			~Volume() 
			{ 
				delete[] Values; 
			}
		};

		struct VolumeActivation : Volume
		{
			ActivationFunctionType ActivationFunction;

			VolumeActivation(Volume* prev, ActivationFunctionType af) : 
				Volume(prev->Width, prev->Height, prev->Depth, VolumeType::Activation)
			{
				ActivationFunction = af;
			}
		};

		struct VolumePooling : Volume
		{
			int FilterWidth, FilterHeight;
			int StrideWidth, StrideHeight;

			VolumePooling(int w, int h, int d, int fw, int fh, int sw, int sh) : Volume(w, h, d, VolumeType::Pooling)
			{
				FilterWidth = fw;	FilterHeight = fh;	StrideWidth = sw; StrideHeight = sh;
				Type = VolumeType::Pooling;
			}
		};

		struct VolumeConvolutional : VolumePooling
		{
			int Padding;
			int CountFilters;

			VolumeConvolutional(int w, int h, int d, 
				int fw, int fh, 
				int sw, int sh, int p, int cf) : VolumePooling(w, h, d, fw, fh, sw, sh)
			{
				Padding = p;	CountFilters = cf;
				Type = VolumeType::Convolutional;
			}
		};

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

