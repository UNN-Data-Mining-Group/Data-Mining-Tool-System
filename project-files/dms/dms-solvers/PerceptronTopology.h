#pragma once
#include "ITopology.h"
#include "DelayCell.h"

namespace dms::solvers::neural_nets
{
	public ref struct LayeredIndex
	{
		int Layer;
		int Index;
		LayeredIndex(int layer, int index) : Layer(layer), Index(index) {}
	};

	public ref class PerceptronTopology : public ITopology
	{
	public:
		PerceptronTopology(array<ICell^, 2>^ cells);
		PerceptronTopology(array<ICell^, 2>^, DelayCell^ delayCell);
		
		virtual bool isConnectionExists(int index_cell1, int index_cell2);
		virtual ICell ^ getCell(int index);
		virtual int getIndexOf(ICell ^ cell);
		virtual int getCellsCount();
		virtual int getCellConnectionsCount(int index);
		virtual array<int>^ getCellConnections(int index);

		bool isConnectionExists(LayeredIndex index_cell1, LayeredIndex index_cell2);
		ICell^ getCell(LayeredIndex index);
		LayeredIndex getLayeredIndex(ICell^ cell);
		LayeredIndex getLayeredIndex(int index);
		int getCellConnectionsCount(LayeredIndex index);
		array<LayeredIndex^>^ getCellConnections(LayeredIndex index);
		int getCellIndex(LayeredIndex index);
		array<ICell^>^ getLayer(int index);
	private:
		array<ICell^>^ cells;
		int cellsCount;
		int layersCount;
		int* cellsInLayers;
		int* delayedCellIndexes;
		int delayedCellCount;
	};
}
