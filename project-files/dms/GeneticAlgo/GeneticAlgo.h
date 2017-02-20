#pragma once
#include<stdlib.h>
#include<malloc.h>

namespace geneticAlgo
{
	float startGeneticAlgo(void** solvers, float** inputs, float* outputs, int count_row, int count_col,
		float(*get_res)(void* solver, float* in), void(*set_weights)(void* solver, float* weights),
		int count_weights, int count_person, int count_epochs, int count_bests, float* res_weights);
}