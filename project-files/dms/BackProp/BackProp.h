#pragma once
#include<stdlib.h>
#include<malloc.h>

namespace backProp
{
	float startBackProp(void* solver, float** inputs, float* outputs, int count_row, int count_col,
		float(*get_res)(void* solver, float* in), 
		void(*set_next_weights)(void* solver, int i, float* weights),
		void(*get_next_grads)(void* solver, int i, float* grads),
		int count_layers, int* count_neuron_per_layer, int count_steps, float** res_weights,
		float start_lr);
}