#pragma once

#include "ISolverDescription.h"
#include "ICell.h"

namespace dms::solvers::neural_nets
{
	public interface class ITopology : public ISolverDescription
	{
	public:
		virtual bool isConnectionExists(int index_cell1, int index_cell2);
		virtual ICell^ getCell(int index);
		virtual int getIndexOf(ICell^ cell);
		virtual int getCellsCount();
		virtual int getCellConnectionsCount(int index);
		virtual array<int>^ getCellConnections(int index);
	};
}
