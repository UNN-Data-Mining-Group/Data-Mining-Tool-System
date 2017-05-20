#include "BackProp.h"
#include "mkl_cblas.h"
#include "mkl_vml.h"
#define PRINT_DEBUG_BACK 1

#ifdef PRINT_DEBUG_BACK
#include"stdio.h"
#endif // PRINT_DEBUG_BACK


namespace backProp
{
	float backProp::startBackProp(
		void* solver, float** inputs, float* outputs, int count_row, int count_col,
		float(*get_res)(void* solver, float* in),
		void(*set_next_weights)(float* weights, int i, void* solver),
		void(*get_next_grads)(float* grads, int i, void* solver),
		void(*get_next_activate)(void* solver, int i, float* activate),
		int count_layers, int* count_neuron_per_layer, int count_steps, float** res_weights,
		int count_lauer_to_layer, int* count_weights_per_lauer, float start_lr
	)
	{
		float tmp;
		float error = 0;
		float** grads = (float**)malloc(count_layers * sizeof(float*));
		float** activate_numbers = (float**)malloc(count_layers * sizeof(float*));
		float** delts = (float**)malloc(count_lauer_to_layer * sizeof(float*));
		float** sigma = (float**)malloc(count_layers * sizeof(float*));

		for (int i = 0; i < count_lauer_to_layer; i++)
		{
			delts[i] = (float*)malloc(count_weights_per_lauer[i] * sizeof(float));
		}

		for (int i = 0; i < count_layers; i++)
		{
			grads[i] = (float*)malloc(count_neuron_per_layer[i] * sizeof(float));
			activate_numbers[i] = (float*)malloc(count_neuron_per_layer[i] * sizeof(float));			
			sigma[i] = (float*)malloc(count_neuron_per_layer[i] * sizeof(float));
		}


		for (int i = 0; i < count_steps; i++)
		{
			for (int j = 0; j < count_row; j++)
			{
				tmp = get_res(solver, inputs[j]);
				error = (tmp - outputs[j])*(tmp - outputs[j]);
				sigma[count_layers - 1][0] = (tmp - outputs[j]);
				for (int k = count_layers - 1; k > 1; k--)
				{
					get_next_grads(grads[k], k, solver);
					get_next_activate(solver, k-1, activate_numbers[k-1]);
					//Считаем сигму для нейронов к-го слоя
#ifdef PRINT_DEBUG_BACK
					printf("Start sigma*grads, %d\n", k);
					for (int l = 0; l < count_neuron_per_layer[k]; l++)
					{
						printf("sigma %d = %f\n", l, sigma[k][l]);
						printf("grads %d = %f\n", l, grads[k][l]);
					}
#endif // PRINT_DEBUG_BACK
					vsMul(
						count_neuron_per_layer[k],
						sigma[k],
						grads[k],
						sigma[k]
					);
#ifdef PRINT_DEBUG_BACK
					printf("Result:\n");
					for (int l = 0; l < count_neuron_per_layer[k]; l++)
					{
						printf("sigma %d = %f\n", l, sigma[k][l]);
					}
#endif // PRINT_DEBUG_BACK


#ifdef PRINT_DEBUG_BACK
					printf("Start start_lr * sigma x activate_numbers, %d\n", k);
					printf("start_lr = %f\n", start_lr);
					for (int l = 0; l < count_neuron_per_layer[k]; l++)
					{
						printf("sigma %d = %f\n", l, sigma[k][l]);
					}
					for (int l = 0; l < count_neuron_per_layer[k-1]; l++)
					{
						printf("activate_numbers %d = %f\n", l, activate_numbers[k - 1][l]);
					}
#endif // PRINT_DEBUG_BACK
					//Считаем смещение для весов
					cblas_sgemm(
						CblasRowMajor, CblasNoTrans, CblasNoTrans, count_neuron_per_layer[k], count_neuron_per_layer[k-1], 1,
						start_lr, sigma[k],
						1, activate_numbers[k - 1],
						count_neuron_per_layer[k-1], 0.0f, delts[k-1], count_neuron_per_layer[k-1]
					);
#ifdef PRINT_DEBUG_BACK
					printf("Result:\n");
					for (int l = 0; l < count_weights_per_lauer[k - 1]; l++)
					{
						printf("delts %d = %f\n", l, delts[k - 1][l]);
					}
#endif // PRINT_DEBUG_BACK
										
					//Передаём ошибку на след слой
					cblas_sgemv(
						CblasRowMajor, CblasTrans, count_neuron_per_layer[k - 1], count_neuron_per_layer[k],
						1.0f, res_weights[k - 1],
						count_neuron_per_layer[k - 1], sigma[k],
						1, 0.0f, sigma[k - 1], 1
					);

					//Смещаем веса

#ifdef PRINT_DEBUG_BACK
					printf("Start weights, %d\n",k-1);
					for (int l = 0; l < count_weights_per_lauer[k-1]; l++)
					{
						printf("Weight %d = %f\n", l, res_weights[k - 1][l]);
						printf("delts %d = %f\n", l, delts[k - 1][l]);
					}
#endif // PRINT_DEBUG_BACK

					cblas_saxpy(
						count_weights_per_lauer[k - 1],
						1.0f, delts[k - 1], 1, res_weights[k - 1], 1
					);
					
#ifdef PRINT_DEBUG_BACK
					printf("Finish weights, %d\n", k - 1);
					for (int l = 0; l < count_weights_per_lauer[k - 1]; l++)
					{
						printf("Weight %d = %f\n", l, res_weights[k - 1][l]);
					}
#endif // PRINT_DEBUG_BACK

					set_next_weights(res_weights[k - 1], k - 1, solver);
				}

				get_next_grads(grads[1], 1, solver);
#ifdef PRINT_DEBUG_BACK
				printf("Start sigma*grads, %d\n", 1);
				for (int l = 0; l < count_neuron_per_layer[1]; l++)
				{
					printf("sigma %d = %f\n", l, sigma[1][l]);
					printf("grads %d = %f\n", l, grads[1][l]);
				}
#endif // PRINT_DEBUG_BACK
				vsMul(
					count_neuron_per_layer[1],
					sigma[1],
					grads[1],
					sigma[1]
				);
#ifdef PRINT_DEBUG_BACK
				printf("Result:\n");
				for (int l = 0; l < count_neuron_per_layer[1]; l++)
				{
					printf("sigma %d = %f\n", l, sigma[1][l]);
				}
#endif // PRINT_DEBUG_BACK

#ifdef PRINT_DEBUG_BACK
				printf("Start start_lr * sigma x inputs, %d\n", 1);
				printf("start_lr = %f\n", start_lr);
				for (int l = 0; l < count_neuron_per_layer[1]; l++)
				{
					printf("sigma %d = %f\n", l, sigma[1][l]);
				}
				for (int l = 0; l < count_neuron_per_layer[0]; l++)
				{
					printf("inputs %d = %f\n", l, inputs[0][l]);
				}
#endif // PRINT_DEBUG_BACK
				//Считаем смещение для весов
				cblas_sgemm(
					CblasRowMajor, CblasNoTrans, CblasNoTrans, count_neuron_per_layer[1], count_neuron_per_layer[0], 1,
					start_lr, sigma[1],
					1, inputs[0],
					count_neuron_per_layer[0], 0.0f, delts[0], count_neuron_per_layer[0]
				);
#ifdef PRINT_DEBUG_BACK
				printf("Result:\n");
				for (int l = 0; l < count_weights_per_lauer[0]; l++)
				{
					printf("delts %d = %f\n", l, delts[0][l]);
				}
#endif // PRINT_DEBUG_BACK
				//Передаём ошибку на след слой
				cblas_sgemv(
					CblasRowMajor, CblasTrans, count_neuron_per_layer[0], count_neuron_per_layer[1],
					1.0f, res_weights[0],
					count_neuron_per_layer[0], sigma[1],
					1, 0.0f, sigma[0], 1
				);

				//Смещаем веса
#ifdef PRINT_DEBUG_BACK
				printf("Start weights, %d\n", 0);
				for (int l = 0; l < count_weights_per_lauer[0]; l++)
				{
					printf("Weight %d = %f\n", l, res_weights[0][l]);
					printf("delts %d = %f\n", l, delts[0][l]);
				}
#endif // PRINT_DEBUG_BACK

				cblas_saxpy(
					count_weights_per_lauer[0],
					1.0f, delts[0], 1, res_weights[0], 1
				);

#ifdef PRINT_DEBUG_BACK
				printf("Finish weights, %d\n", 0);
				for (int l = 0; l < count_weights_per_lauer[0]; l++)
				{
					printf("Weight %d = %f\n", l, res_weights[0][l]);
				}
#endif // PRINT_DEBUG_BACK

				set_next_weights(res_weights[0], 0, solver);
			}
		}

		for (int i = 0; i < count_lauer_to_layer; i++)
		{
			free(delts[i]);
		}

		for (int i = 0; i < count_layers; i++)
		{
			free(grads[i]);
			free(activate_numbers[i]);
			free(sigma[i]);
		}

		free(grads);
		free(activate_numbers);
		free(delts);
		free(sigma);




		return error;
	}
}
