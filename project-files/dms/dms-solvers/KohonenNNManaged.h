#pragma once
#include "INeuralNetwork.h"
#include "KohonenNNTopology.h"
#include "KohonenNet.h"

using System::Collections::Generic::List;

namespace dms::solvers::neural_nets::kohonen
{
	public ref struct int2d
	{
	public:
		int x, y;
		int2d(int _x, int _y)
		{
			x = _x; y = _y;
		}
	};

	[SerializableAttribute]
	public ref class KohonenManaged : public INeuralNetwork 
	{
	public:
		KohonenManaged(KohonenNNTopology^ t);

		virtual array<Single>^ Solve(array<Single>^ x) override;
		virtual void* getAttributes() override;
		virtual void* getOperations() override;
		virtual void* getNativeSolver() override;

		virtual void FetchNativeParameters() override;
		virtual void PushNativeParameters() override;
		
		array<List<Tuple<int2d^, double>^>^>^ GetVisualData();

		virtual ~KohonenManaged();
	private:
		array<float>^ weights;
		array<array<float>^>^ classes;
		List<Tuple<int, int>^>^ neurons;
		bool use_normalization;
		KohonenNNTopology^ t;

		[NonSerializedAttribute]
		float* x;
		[NonSerializedAttribute]
		float* y;
		[NonSerializedAttribute]
		nnets_kohonen::KohonenNet* solver;
	};
}
