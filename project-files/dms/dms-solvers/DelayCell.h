#pragma once
#include "ICell.h"
#include "INeuron.h"

using namespace System::Collections::Generic;

namespace dms::solvers::neural_nets
{
	public ref class DelayCell : ICell
	{
	public:
		void addConnection(INeuron^ n);
		array<INeuron^>^ getConnections();
		bool isConnected(INeuron^ n);
		virtual float getOutput();
	private:
		List<INeuron^> delayedNeurons;
	};
}
