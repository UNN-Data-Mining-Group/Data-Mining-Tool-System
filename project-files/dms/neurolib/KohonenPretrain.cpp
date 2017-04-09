#include "KohonenPretrain.h"

using namespace nnets_kohonen;

//#define DEBUG_OUTPUT
#ifdef DEBUG_OUTPUT
#include <iostream>
#endif

using pair = std::pair<int, int>;

bool ClassExtracter::is_equal(float* y1, float* y2, int size)
{
	for (int i = 0; i < size; i++)
		if (std::abs(y1[i] - y2[i]) > eps)
			return false;
	return true;
}

void ClassExtracter::fit(float** y, int rowsCount)
{
	distrib.clear();

	distrib.push_back(pair(0, 1));
	for (int i = 1; i < rowsCount; i++)
	{
		int class_number = -1;
		for (int k = 0; k < distrib.size(); k++)
		{
			if (is_equal(y[i], y[distrib[k].first], rowsCount) == true)
			{
				class_number = k;
				break;
			}
		}
		if (class_number == -1)
			distrib.push_back(pair(i, 1));
		else
			distrib[class_number].second++;
	}
}

std::vector<std::pair<int, int>> ClassExtracter::getClassesDistributions() { return distrib; }