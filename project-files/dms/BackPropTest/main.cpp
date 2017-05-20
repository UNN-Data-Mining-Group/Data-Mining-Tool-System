#include"../BackProp/BackProp.h"
#include "main.h"
#include"stdio.h"

//void* solver, float** inputs, float* outputs, int count_row, int count_col,
//float(*get_res)(void* solver, float* in),
//void(*set_next_weights)(void* solver, int i, float* weights),
//void(*get_next_grads)(void* solver, int i, float* grads),
//void(*get_next_activate)(void* solver, int i, float* activate),
//int count_layers, int* count_neuron_per_layer, int count_steps, float** res_weights,
//int count_lauer_to_layer, int* count_weights_per_lauer, float start_lr;

float get_res_(void* solver, float* in)
{
	return 3;
}

void set_next_weights_(void* solver, int i, float* weights)
{

}

void get_next_grads_(void* solver, int i, float* grads)
{
	grads[0] = 2;
}

void get_next_activate_(void* solver, int i, float* activate)
{
	activate[0] = 1;
}

void first_test()
{

	void(*get_next_activate)(void* solver, int i, float* activate) = get_next_activate_;

	void(*get_next_grads)(void* solver, int i, float* grads) = get_next_grads_;
	float(*get_res)(void* solver, float* in) = get_res_;
	void(*set_next_weights)(void* solver, int i, float* weights) = set_next_weights_;
	void* solver = malloc(sizeof(double));
	int count_row = 1;
	int count_col = 2;
	float** inputs = (float**)malloc(count_row * sizeof(float*));
	float* outputs = (float*)malloc(count_row * sizeof(float));
	int count_layers = 4;
	int* count_neuron_per_layer = (int*)malloc(count_layers * sizeof(int));
	int count_steps = 1;
	int count_lauer_to_layer = count_layers-1;
	int* count_weights_per_lauer = (int*)malloc(count_lauer_to_layer * sizeof(int));
	float** res_weights = (float**)malloc(count_lauer_to_layer * sizeof(float*));
	float start_lr = 0.1f;



	count_neuron_per_layer[0] = count_col;
	count_neuron_per_layer[1] = count_col*2;
	count_neuron_per_layer[2] = count_col;
	count_neuron_per_layer[3] = 1;

	for (int i = 0; i < count_lauer_to_layer; i++)
	{
		count_weights_per_lauer[i] = count_neuron_per_layer[i] * count_neuron_per_layer[i + 1];
		res_weights[i] = (float*)malloc(count_weights_per_lauer[i] * sizeof(float));
		for (int j = 0; j < count_weights_per_lauer[i]; j++)
		{
			res_weights[i][j] = (i + 1)*(j + 1);
		}
	}





	for (int i = 0; i < count_row; i++)
	{
		inputs[i] = (float*)malloc(count_col * sizeof(float));
		for (int j = 0; j < count_col; j++)
		{
			inputs[i][j] = (i + 1)*(j + 1);
		}
		outputs[i] = i;
	}
	backProp::startBackProp(
		solver, inputs, outputs, count_row, count_col,
		get_res,
		set_next_weights,
		get_next_grads_,
		get_next_activate,
		count_layers, count_neuron_per_layer, count_steps, res_weights,
		count_lauer_to_layer, count_weights_per_lauer, start_lr
	);

	for (int i = 0; i < count_lauer_to_layer; i++)
	{
		free(res_weights[i]);
	}
	free(res_weights);

	free(count_weights_per_lauer);
	free(count_neuron_per_layer);
	free(solver);
	for (int i = 0; i < count_row; i++)
	{
		free(inputs[i]);
	}
	free(inputs);
	free(outputs);
}

int main()
{
	float weigth;
	first_test();
	scanf("%f", &weigth);
	return 0;
}