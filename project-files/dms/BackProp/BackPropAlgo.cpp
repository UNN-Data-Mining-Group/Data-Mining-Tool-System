#include "BackPropAlgo.h"
#include "mkl_cblas.h"
#include "mkl_vml.h"
#define PRINT_DEBUG_BACK 1

#ifdef PRINT_DEBUG_BACK
#include"stdio.h"
#include <fstream>
#include <iostream>
using namespace std;
#endif // PRINT_DEBUG_BACK


namespace backPropAlgo
{

	float minus(float* a, float* b, int size)
	{
		float res = 0;
		for (int i = 0; i < size; i++)
		{						//#################################
			res = a[i] - b[i];	//########_FIX_ME!!!_##############
		}						//#################################
		return res;
	}

	float backPropAlgo::startBackPropAlgo(
		void* solver, float** inputs, float** outputs, int count_row, int count_col,
		size_t(*get_res)(float* in, float* out, void* solver),
		size_t(*set_next_weights)(float* weights, int i, void* solver),
		size_t(*get_next_grads)(float* grads, int i, void* solver),
		size_t(*get_next_activate)(float* activate, int i, void* solver),
		int count_layers, size_t* count_neuron_per_layer, int count_steps, float** res_weights,
		int count_lauer_to_layer, int* count_weights_per_lauer, float start_lr, int out_size
	)
	{
		float* result = new float[out_size];
		float tmp;
		float error = 0;
		float** grads = (float**)malloc(count_layers * sizeof(float*));
		float** activate_numbers = (float**)malloc(count_layers * sizeof(float*));
		float** delts = (float**)malloc(count_lauer_to_layer * sizeof(float*));
		float** sigma = (float**)malloc(count_layers * sizeof(float*));

		for (int i = 0; i < count_lauer_to_layer; i++)
		{
			for (int j = 0; j < count_weights_per_lauer[i]; j++)
			{
				res_weights[i][j]= ((float)rand()) / RAND_MAX;
			}
			set_next_weights(res_weights[i],i, solver);
		}

#ifdef PRINT_DEBUG_BACK
		ofstream fout;
		fout.open("BackProp_debug.txt", ios_base::trunc);
		fout << "Count step = " << count_steps << endl;
		fout << "count_layers = " << count_layers << endl;
		fout << "count_lauer_to_layer = " << count_lauer_to_layer << endl;
		fout.close();
#endif // PRINT_DEBUG_BACK
		for (int num_layer_to_layer = 0; num_layer_to_layer < count_lauer_to_layer; num_layer_to_layer++)
		{
			delts[num_layer_to_layer] = (float*)malloc(count_weights_per_lauer[num_layer_to_layer] * sizeof(float));
		}

		for (int num_layer = 0; num_layer < count_layers; num_layer++)
		{
			grads[num_layer] = (float*)malloc(count_neuron_per_layer[num_layer] * sizeof(float));
			activate_numbers[num_layer] = (float*)malloc(count_neuron_per_layer[num_layer] * sizeof(float));			
			sigma[num_layer] = (float*)malloc(count_neuron_per_layer[num_layer] * sizeof(float));
		}



		for (int num_step = 0; num_step < count_steps; num_step++)
		{
			for (int num_row = 0; num_row < count_row; num_row++)
			{
				get_res(inputs[num_row], result, solver);
				tmp = minus(result, outputs[num_row], out_size);
				error = tmp*tmp;
				sigma[count_layers - 1][0] = tmp;
#ifdef PRINT_DEBUG_BACK
				fout.open("BackProp_debug.txt", ios::app);
				fout << "tmp = " << tmp << endl;
				fout << "err, " << error << endl;
				fout << "out_gold, " << outputs[num_row][0] << endl;
				fout << "res, " << result[0] << endl;
				fout.close();

#endif // PRINT_DEBUG_BACK
				for (int k = count_layers - 1; k > 1; k--)
				{
					get_next_grads(grads[k], k, solver);
					get_next_activate(activate_numbers[k - 1], k-1, solver);
					//Считаем сигму для нейронов к-го слоя
#ifdef PRINT_DEBUG_BACK
					fout.open("BackProp_debug.txt", ios::app);
					fout << "Start sigma*grads, "<< k<< endl;
					for (int l = 0; l < count_neuron_per_layer[k]; l++)
					{
						fout << "sigma "<< l <<" = "<< sigma[k][l] << endl;
						fout << "grads "<< l<<" = "<< grads[k][l] << endl;
					}
					fout << "Finish sigma*grads, " << k << endl;
					fout.close();

#endif // PRINT_DEBUG_BACK
					vsMul(
						count_neuron_per_layer[k],
						sigma[k],
						grads[k],
						sigma[k]
					);
#ifdef PRINT_DEBUG_BACK
					fout.open("BackProp_debug.txt", ios::app);
					fout << "Result:" << endl;
					for (int l = 0; l < count_neuron_per_layer[k]; l++)
					{
						fout << "sigma "<< l<<" = "<< sigma[k][l] << endl;
					}
					fout.close();
#endif // PRINT_DEBUG_BACK


#ifdef PRINT_DEBUG_BACK
					fout.open("BackProp_debug.txt", ios::app);
					fout << "Start start_lr * sigma x activate_numbers, "<< k << endl;
					fout << "start_lr = "<< start_lr << endl;
					for (int l = 0; l < count_neuron_per_layer[k]; l++)
					{
						fout << "sigma "<< l<<" = "<< sigma[k][l] << endl;
					}
					for (int l = 0; l < count_neuron_per_layer[k-1]; l++)
					{
						fout << "activate_numbers "<< l<<" = "<< activate_numbers[k - 1][l] << endl;
					}
					fout.close();
#endif // PRINT_DEBUG_BACK
					//Считаем смещение для весов
					cblas_sgemm(
						CblasRowMajor, CblasNoTrans, CblasNoTrans, count_neuron_per_layer[k], count_neuron_per_layer[k-1], 1,
						start_lr, sigma[k],
						1, activate_numbers[k - 1],
						count_neuron_per_layer[k-1], 0.0f, delts[k-1], count_neuron_per_layer[k-1]
					);
#ifdef PRINT_DEBUG_BACK
					fout.open("BackProp_debug.txt", ios::app);
					fout << "Result:" << endl;
					for (int l = 0; l < count_weights_per_lauer[k - 1]; l++)
					{
						fout << "delts "<< l<<" = "<< delts[k - 1][l] << endl;
					}
					fout.close();
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
					fout.open("BackProp_debug.txt", ios::app);
					fout << "Start weights =" << k - 1 << endl;
					for (int l = 0; l < count_weights_per_lauer[k-1]; l++)
					{
						fout << "Weight "<< l<<" = "<<res_weights[k - 1][l] << endl;
						fout << "delts "<<l << " = " << delts[k - 1][l] << endl;
					}
					fout.close();
#endif // PRINT_DEBUG_BACK

					cblas_saxpy(
						count_weights_per_lauer[k - 1],
						1.0f, delts[k - 1], 1, res_weights[k - 1], 1
					);
					
#ifdef PRINT_DEBUG_BACK
					fout.open("BackProp_debug.txt", ios::app);
					fout << "Finish weights" << k - 1 << endl;;
					for (int l = 0; l < count_weights_per_lauer[k - 1]; l++)
					{
						fout << "Weight "<<l<<" = "<< res_weights[k - 1][l] << endl;;
					}
					fout.close();
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
	float tmp_(size_t * count, float ** in_1, float ** in_2, float ** out)
	{
		int k = 0;
		vsMul(
			count[0],
			in_1[k],
			in_2[k],
			out[k]
		);
		return 0;
	}
}
