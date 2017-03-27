#pragma once
#include "ISolverDescription.h"

namespace dms::solvers::neural_nets::kohonen
{
	[System::SerializableAttribute]
	public ref class KohonenNNTopology : public ISolverDescription
	{
	public:
		KohonenNNTopology(int inputs, int outputs, int width, int height,
			System::String^ metric) : inputs(inputs), outputs(outputs),
			width(width), height(height), metric(metric)
		{}

		int GetInputsCount() { return inputs; }
		int GetOutputsCount() { return outputs; }
		int GetLayerWidth() { return width; }
		int GetLayerHeight() { return height; }
		System::String^ GetMetric() { return metric; }
		static array<System::String^>^ GetAvaliableMetrics() 
		{
			return gcnew array<System::String^>{"Default", "Euclidean"};
		}
	private:
		int inputs, outputs, width, height;
		System::String^ metric;
	};
}
