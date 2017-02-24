#pragma once
#include "ISolverDescription.h"
#include "ActivationFunctionTypes.h"

using System::String;
using System::Collections::Generic::List;

namespace dms::solvers::neural_nets::conv_net
{
	[System::SerializableAttribute]
	public ref struct ILayer abstract {};

	[System::SerializableAttribute]
	public ref struct FullyConnectedLayer : public ILayer
	{
		property int NeuronsCount;
		property String^ ActivationFunction;
	};

	[System::SerializableAttribute]
	public ref struct PoolingLayer : public ILayer
	{
		property int FilterWidth;
		property int FilterHeight;
		property int StrideWidth; 
		property int StrideHeight;
	};

	[System::SerializableAttribute]
	public ref struct ConvolutionLayer : public PoolingLayer
	{
		property int Padding;
		property int CountFilters;
		property String^ ActivationFunction;
	};

	[System::SerializableAttribute]
	public ref class ConvNNTopology : public ISolverDescription
	{
	public:
		[System::SerializableAttribute]
		ref struct VolumeDimention
		{
			int Width, Heigth, Depth;
		};

		ConvNNTopology(int input_weight, int input_heigth, int input_depth, List<ILayer^>^ layers);
		
		int GetInputWidth();
		int GetInputHeigth();
		int GetInputDepth();
		int GetInputsCount();
		int GetOutputsCount();
		List<ILayer^>^ GetLayers();
		List<VolumeDimention^>^ GetVolumeDimentions();
	private:
		int calcOutputVolume(int input, int filter, int padding, int stride);

		int w, h, d;
		List<ILayer^>^ layers;
		List<VolumeDimention^>^ layerDimentions;
	};
}
