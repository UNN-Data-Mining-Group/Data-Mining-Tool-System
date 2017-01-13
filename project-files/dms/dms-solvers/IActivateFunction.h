#pragma once

//Источник инфы по активационным функциям:
//https://en.wikipedia.org/wiki/Activation_function

namespace dms::solvers::neural_nets
{
	public interface class IActivateFunction
	{
	public:
		virtual float getResult(float x) = 0;
	};
}
