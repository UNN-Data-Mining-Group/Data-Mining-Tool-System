#include"../BackProp/BackPropAlgo.h"
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
//	backPropAlgo::startBackPropAlgo(
//		solver, inputs, outputs, count_row, count_col,
//		get_res,
//		set_next_weights,
//		get_next_grads_,
//		get_next_activate,
//		count_layers, count_neuron_per_layer, count_steps, res_weights,
//		count_lauer_to_layer, count_weights_per_lauer, start_lr
//	);

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
/*
int main()
{
	float weigth;
	first_test();
	scanf("%f", &weigth);
	return 0;
}
*/

int main()
{
	int countLayers = 2;
	int k = countLayers - 1;
	size_t* count_neuron_per_layer = new size_t[countLayers];
	float** sigma = new float*[countLayers];
	float** grads = new float*[countLayers];
	float start_lr = 1;
	float** activate_numbers = new float*[countLayers];
	float** delts = new float*[countLayers];
	int* count_weights_per_lauer = new int[k];
	float** res_weights = new float*[k];
	float* gold_sigma = new float[3];
	count_weights_per_lauer[0] = 6;
	res_weights[0] = new float[6];
	res_weights[0][0] = 0.3f;
	res_weights[0][1] = -0.1f;
	res_weights[0][2] = 0.7f;
	res_weights[0][3] = 0.4f;
	res_weights[0][4] = -0.5f;
	res_weights[0][5] = -0.2f;

	delts[0] = new float[6];

	count_neuron_per_layer[0] = 3;
	count_neuron_per_layer[1] = 2;

	activate_numbers[0] = new float[3];
	activate_numbers[0][0] = 0.3f;
	activate_numbers[0][1] = 0.6f;
	activate_numbers[0][2] = 0.1f;

	grads[1] = new float[2];
	grads[1][0] = 1.0f;
	grads[1][1] = 0.2f;

	sigma[0] = new float[3];
	sigma[1] = new float[2];
	sigma[1][0] = 1.0f;
	sigma[1][1] = -1.0f;

	backPropAlgo::tmp_(k, count_neuron_per_layer,
		sigma, grads, start_lr,
		activate_numbers, delts,
		count_weights_per_lauer, res_weights);

	for (int i = 0; i < 3; i++)
	{
		printf("%f\n", sigma[0][i]);
	}

	gold_sigma[0] = 0.22f;
	gold_sigma[1] = 0.f;
	gold_sigma[2] = 0.74f;

	for (int i = 0; i < 3; i++)
	{
		if (gold_sigma[i] != sigma[0][i])
			printf("ERROR!!!!gold_sigma != sigma: %f != %f\n", gold_sigma[i], sigma[0][i]);
	}

	return 0;
}