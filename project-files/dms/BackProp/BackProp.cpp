#include "BackProp.h"

void vec_minus(float* res, const float* const v_left, const float* const v_right, int size)
{
	for (int i = 0; i < size; i++)
	{
		res[i] = v_left[i] - v_right[i];
	}
}

float backProp::startBackProp(void* solver, float** inputs, float* outputs, int count_row, int count_col,
	float(*get_res)(void* solver, float* in),
	void(*set_next_weights)(void* solver, int i, float* weights),
	void(*get_next_grads)(void* solver, int i, float* grads),
	int count_layers, int* count_neuron_per_layer, int count_steps, float** res_weights,
	float start_lr) 
{
	float error = 0;
	float** grads = (float**)malloc(count_layers*sizeof(float*));

	for (int i = 0; i < count_layers; i++)
	{
		grads[i] = (float*)malloc(count_neuron_per_layer[i] * sizeof(float));
	}
	for (int i = 0; i < count_steps; i++)
	{
		for (int  j = 0; j < count_row; j++)
		{
			float tmp = get_res(solver, inputs[j]);
			float delta;
			error = (tmp - outputs[j])*(tmp - outputs[j]);
			for (int k = count_layers - 1; k >= 0; k--)
			{	
				get_next_grads(solver, k, grads[k]);
				delta = (tmp - outputs[j])*
			}
		}		
	}
	return error;
}
