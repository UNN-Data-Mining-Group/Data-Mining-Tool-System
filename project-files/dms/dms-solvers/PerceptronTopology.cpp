#include "stdafx.h"
#include "PerceptronTopology.h"

namespace dms::solvers::neural_nets
{
	PerceptronTopology::PerceptronTopology(array<ICell^, 2>^ cells) : PerceptronTopology(cells, nullptr)
	{
	}

	PerceptronTopology::PerceptronTopology(array<ICell^, 2>^ cells, DelayCell^ delayCell)
	{
	}

	bool PerceptronTopology::isConnectionExists(int index_cell1, int index_cell2)
	{
		return false;
	}

	ICell^ PerceptronTopology::getCell(int index)
	{
		throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
	}

	int PerceptronTopology::getIndexOf(ICell ^ cell)
	{
		return 0;
	}

	int PerceptronTopology::getCellsCount()
	{
		return 0;
	}

	int PerceptronTopology::getCellConnectionsCount(int index)
	{
		return 0;
	}

	array<int>^ PerceptronTopology::getCellConnections(int index)
	{
		throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
	}
}