#include "BackProp.h"
#include "mkl_cblas.h"

void vec_minus(float* res, const float* const v_left, const float* const v_right, int size)
{
	for (int i = 0; i < size; i++)
	{
		res[i] = v_left[i] - v_right[i];
	}
}
namespace backProp
{
	float backProp::startBackProp(
		void* solver, float** inputs, float* outputs, int count_row, int count_col,
		float(*get_res)(void* solver, float* in),
		void(*set_next_weights)(void* solver, int i, float* weights),
		void(*get_next_grads)(void* solver, int i, float* grads),
		void(*get_next_activate)(void* solver, int i, float* activate),
		int count_layers, int* count_neuron_per_layer, int count_steps, float** res_weights,
		int count_lauer_to_layer, int* count_weights_per_lauer, float start_lr
	)
	{
		float tmp;
		float error = 0;
		float** grads = (float**)malloc(count_layers * sizeof(float*));
		float** activate_numbers = (float**)malloc(count_layers * sizeof(float*));
		float** delts = (float**)malloc(count_layers * sizeof(float*));
		float** sigma = (float**)malloc(count_layers * sizeof(float*));

		for (int i = 0; i < count_layers; i++)
		{
			grads[i] = (float*)malloc(count_neuron_per_layer[i] * sizeof(float));
			activate_numbers[i] = (float*)malloc(count_neuron_per_layer[i] * sizeof(float));
			delts[i] = (float*)malloc(count_neuron_per_layer[i] * sizeof(float));
			sigma[i] = (float*)malloc(count_neuron_per_layer[i] * sizeof(float));
		}


		for (int i = 0; i < count_steps; i++)
		{
			for (int j = 0; j < count_row; j++)
			{
				tmp = get_res(solver, inputs[j]);
				error = (tmp - outputs[j])*(tmp - outputs[j]);
				sigma[count_layers - 1][0] = (tmp - outputs[j]);
				for (int k = count_layers - 1; k > 0; k--)
				{
					get_next_grads(solver, k, grads[k]);
					get_next_activate(solver, k, activate_numbers[k]);
					//Считаем смещение для весов
					cblas_sgemv(
						CblasRowMajor, CblasNoTrans, count_neuron_per_layer[k], 1,
						start_lr, sigma[k],
						count_neuron_per_layer[k], activate_numbers[k],
						1, 0.0f, delts[k], 1
					);
					//Смещаем веса
					cblas_saxpy(
						count_neuron_per_layer[k],
						1.0f, delts[k], 1, res_weights[k], 1
					);
					//Передаём ошибку на след слой
					cblas_sgemv(
						CblasRowMajor, CblasTrans, count_neuron_per_layer[k-1], count_neuron_per_layer[k],
						1.0f, res_weights[k-1],
						count_neuron_per_layer[k-1], sigma[k],
						1, 0.0f, sigma[k-1], 1
					);
					set_next_weights(solver, k, res_weights[k]);
				}
			}
		}
		return error;
	}
}
