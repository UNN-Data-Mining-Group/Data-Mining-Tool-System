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
		KohonenManaged(KohonenManaged^ k);

		virtual void* getAttributes() override;
		virtual void* getOperations() override;

		virtual void FetchNativeParameters() override;
		virtual void PushNativeParameters() override;

		virtual array<Single>^ Solve(array<Single>^ x) override;

		virtual ISolver^ Copy() override;

		void setClasses(array<array<float>^>^ outputs);
		array<List<Tuple<int2d^, double>^>^>^ GetVisualData();
		void declareWinnerOutput(bool is_positive);
		void startLogWinners();
		void stopLogWinners();
		void declareWinnersOutput(List<bool>^ positives);
	private:
		array<float>^ weights;
		array<array<float>^>^ classes;
		array<int>^ false_sols;
		array<int>^ true_sols;
		List<Tuple<int, int>^>^ neurons;
		bool use_normalization;

		[NonSerializedAttribute]
		List<int>^ winners_log;
		[NonSerializedAttribute]
		bool is_logging;
	};
}
