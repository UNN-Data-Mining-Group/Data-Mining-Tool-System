#pragma once
#include "ITopology.h"
#include "KohonenNet.h"

namespace dms::solvers::neural_nets::kohonen
{
	[System::SerializableAttribute]
	public ref class KohonenNNTopology : public ITopology
	{
	public:
		KohonenNNTopology(int inputs, int outputs, int width, int height,
			System::String^ metric) : inputs(inputs), outputs(outputs),
			width(width), height(height), metric(metric)
		{}

		int GetLayerWidth() { return width; }
		int GetLayerHeight() { return height; }
		System::String^ GetMetric() { return metric; }
		static array<System::String^>^ GetAvaliableMetrics() 
		{
			return gcnew array<System::String^>{"Default", "Euclidean"};
		}

		virtual System::Int64 GetInputsCount() { return inputs; }
		virtual System::Int64 GetOutputsCount() { return outputs; }
		virtual nnets::NeuralNetwork* createNativeSolver()
		{
			nnets_kohonen::KohonenNet::Metric m;
			if (metric->Equals("Default"))
				m = nnets_kohonen::KohonenNet::Default;
			else if (metric->Equals("Euclidean"))
				m = nnets_kohonen::KohonenNet::Euclidean;
			else throw gcnew System::ArgumentException();

			return new nnets_kohonen::KohonenNet(inputs, outputs, width, height, 
				nnets_kohonen::KohonenNet::ClassInitializer::Random, m);
		}
	private:
		int inputs, outputs, width, height;
		System::String^ metric;
	};
}
