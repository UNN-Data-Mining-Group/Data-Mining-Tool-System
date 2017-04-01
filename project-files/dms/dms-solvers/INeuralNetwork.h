#pragma once
#include "ISolver.h"
#include "ITopology.h"
#include "NeuralNetwork.h"

namespace dms::solvers::neural_nets
{
	[SerializableAttribute]
	public ref class INeuralNetwork abstract : public ISolver
	{
	public:
		INeuralNetwork(ITopology^ topology);

		//copy native weights to managed for serializing and saving in database
		virtual void FetchNativeParameters() = 0;

		//copy managed weights to native to start working with net
		virtual void PushNativeParameters() = 0;

		virtual void* getNativeSolver();

		virtual array<Single>^ Solve(array<Single>^ x) override;

		virtual ~INeuralNetwork();
	protected:
		ITopology^ topology;
	private:
		[NonSerializedAttribute]
		nnets::NeuralNetwork* solver;
		[NonSerializedAttribute]
		float *x, *y;
		[NonSerializedAttribute]
		bool isInitialized = false;

		void init();
	};
}
