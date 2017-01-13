#pragma once

namespace neurolib
{
	using oper_af = float(*)(float);

	class Perceptron
	{
	public:
		Perceptron(Perceptron& p);
		Perceptron(int* neurons, oper_af* afs, int layers, float** weights);
		Perceptron(int* neurons, bool* has_delay, oper_af* afs, int layers, float** weights);

		int solve(float* x, float* y);
		int getInputsCount();
		int getOutputsCount();

		~Perceptron();
	private:
		bool can_init(int* neurons, bool* has_delay, oper_af* afs, int layers, float** weights);
		void init(int* neurons, bool* has_delay, oper_af* afs, int layers, float** weights);
		int mult(float* w, float* v, float* dest, int cols, int rows);

		int layers;
		int* neurons;
		bool* has_delay;
		oper_af* afs;
		float** w;
		float** temp_res;
		bool is_initialised;
	};
}