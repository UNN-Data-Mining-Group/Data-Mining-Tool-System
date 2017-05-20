#pragma once
#include<stdlib.h>
#include<malloc.h>
namespace backPropAlgo
{
	float startBackPropAlgo(void* solver, float** inputs,
		float** outputs, int count_row, int count_col,
		size_t(*get_res)(float* in, float* out,void* solver),
		size_t(*set_next_weights)(float* weights, int i, void* solver),
		size_t(*get_next_grads)(float* grads, int i, void* solver),
		size_t(*get_next_activate)(float* activate, int i, void* solver),
		int count_layers, int* count_neuron_per_layer, int count_steps,
		float** res_weights,
		int count_lauer_to_layer,int* count_weights_per_lauer,
		float start_lr, int out_size);
}