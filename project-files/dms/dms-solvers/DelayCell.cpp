#include "Stdafx.h"
#include "DelayCell.h"

namespace dms::solvers::neural_nets
{
	void DelayCell::addConnection(INeuron^ n)
	{
		delayedNeurons.Add(n);
	}

	array<INeuron^>^ DelayCell::getConnections()
	{
		return delayedNeurons.ToArray();
	}

	bool DelayCell::isConnected(INeuron^ n)
	{
		return delayedNeurons.Contains(n);
	}
}