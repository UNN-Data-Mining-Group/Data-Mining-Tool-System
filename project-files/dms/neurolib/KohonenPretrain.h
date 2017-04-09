#pragma once
#include <vector>
#include <map>
#include "KohonenNet.h"

namespace nnets_kohonen
{
	class ClassExtracter
	{
	public:
		ClassExtracter(float eps) : eps(eps) {}

		//first - index of element in y, second - count of elements in selection of this class
		std::vector<std::pair<int, int>> getClassesDistributions();

		void fit(float** y, int rowsCount);
	private:
		float eps;
		std::vector<std::pair<int, int>> distrib;

		bool is_equal(float* y1, float* y2, int size);
	};
}
