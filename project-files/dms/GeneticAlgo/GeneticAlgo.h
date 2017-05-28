#pragma once
#include<stdlib.h>
#include<malloc.h>

namespace geneticAlgo
{
	float startGeneticAlgo(void** solvers, float** inputs,
		float* outputs, int count_row, int count_col,
		size_t(*get_res)( float* in, float* out, void* solver),
		void(*set_weights)(float* weights,void* solver),
		int count_weights, int count_person, int count_epochs,
		int count_bests, float mutation_percent, float* res_weights);
}