#pragma once
#include<stdlib.h>
#include<malloc.h>
namespace backProp
{
	float startBackProp(void* solver, float** inputs,
		float* outputs, int count_row, int count_col,
		float(*get_res)(void* solver, float* in), 
		void(*set_next_weights)(float* weights, int i, void* solver),
		void(*get_next_grads)(void* solver, int i, float* grads),
		void(*get_next_activate)(void* solver, int i, float* activate),
		int count_layers, int* count_neuron_per_layer, int count_steps,
		float** res_weights,
		int count_lauer_to_layer,int* count_weights_per_lauer,
		float start_lr);
}