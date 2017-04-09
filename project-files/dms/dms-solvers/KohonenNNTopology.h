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
			float classEps, System::String^ classInitializer, System::String^ metric) 
			: inputs(inputs), outputs(outputs),
			width(width), height(height), 
			metric(metric), classInitializer(classInitializer), classEps(classEps)
		{}

		int GetLayerWidth() { return width; }
		int GetLayerHeight() { return height; }
		float GetClassEps() { return classEps; }
		System::String^ GetMetric() { return metric; }
		static array<System::String^>^ GetAvaliableMetrics() 
		{
			return gcnew array<System::String^>{"Default", "Euclidean"};
		}

		System::String^ GetClassInitializer() { return classInitializer; }
		static array<System::String^>^ GetClassInitializerList()
		{
			return gcnew array<System::String^> {"Evenly", "Statistical", "Revert"};
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

			nnets_kohonen::KohonenNet::ClassInitializer cl;
			if (classInitializer->Equals("Evenly"))
				cl = nnets_kohonen::KohonenNet::ClassInitializer::Evenly;
			else if (classInitializer->Equals("Statistical"))
				cl = nnets_kohonen::KohonenNet::ClassInitializer::Statistical;
			else if (classInitializer->Equals("Revert"))
				cl = nnets_kohonen::KohonenNet::ClassInitializer::Revert;
			else throw gcnew System::ArgumentException();

			return new nnets_kohonen::KohonenNet(inputs, outputs, width, height, classEps, cl, m);
		}
	private:
		int inputs, outputs, width, height;
		System::String^ metric;
		System::String^ classInitializer;
		float classEps;
	};
}
