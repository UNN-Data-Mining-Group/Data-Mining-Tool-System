#pragma once

#include "ICell.h"

namespace dms::solvers::neural_nets
{
	public ref class INeuron
	{
	public:
		INeuron()
		{
			setInitialState();
		}

		~INeuron()
		{
			setInitialState();
		}

		//Установления указателя на массив, где лежат веса нейрона
		virtual void setWeigthsSource(float* src, int wcount);

		//Установка весов нейрона. Если перед вызовом данного метода
		//источник весов не был установлен, веса не будут выставлены
	    virtual void setWeights(array<float>^ w);

		virtual array<float>^ getWeights();
		virtual float getResult(array<float>^ x) = 0;
		virtual float getWeightedSum() = 0;
	private:
		float* weights_src;
		int weights_src_size;

		void setInitialState();
	};
}
