#include "GeneticAlgo.h"

//#define GENETIC_DEBUG 0


#include <fstream>
#include <iostream>
#include <ctime>
using namespace std;

namespace geneticAlgo
{

	void Mut(float* w, int count_weights)
	{
		for (int i = 0; i < count_weights; i++)
		{
			if (rand() % 2 == 1) w[i] = (((float)(rand() % 20001)) / 1000) - 10; //((float)rand()) / RAND_MAX;
		}
	}

	void Cross(float* weights1, float* weights2, float* res, int count_weights)
	{
		for (int i = 0; i < count_weights; i++)
		{
			if (rand() % 2 == 1) res[i] = weights1[i];
			else res[i] = weights2[i];
		}
	}

	// средн€€ ошибка солвера на выборке
	float GetAvgError(float* weights, void* solver, float** inputs, float* outputs, int count_row,
		size_t(*get_res)(float* in, float* out, void* solver), void(*set_weights)(float* weights, void* solver))
	{
		float avgError = 0;
		set_weights(weights, solver);

		for (int i = 0; i < count_row; i++)
		{
			float result = 0;
			get_res(inputs[i], &result, solver);
			avgError += abs(result - outputs[i]);
		}

		avgError /= count_row;
		return avgError;
	}

	float GetMaxError(float* weights, void* solver, float** inputs, float* outputs, int count_row,
		size_t(*get_res)(float* in, float* out, void* solver), void(*set_weights)(float* weights, void* solver))
	{
		float maxError = -1;
		set_weights(weights, solver);

		for (int i = 0; i < count_row; i++)
		{
			float result = 0;
			get_res(inputs[i], &result, solver);
			if (abs(result - outputs[i]) > maxError) maxError = abs(result - outputs[i]);
		}

		return maxError;
	}


	float startGeneticAlgo(void** solvers, float** inputs, float* outputs, int count_row, int count_col,
		size_t(*get_res)(float* in, float* out, void* solver), void(*set_weights)(float* weights, void* solver),
		int count_weights,
		int count_person, int count_epochs, int count_bests, float mutation_percent,
		float* res_weights)
	{
		srand(time(0));

		float** generation = new float*[count_person];
		float** child_generation = new float*[count_person];
		float* errors = new float[count_person];
		int* best_weights = new int[count_bests];
		float* best_errors = new float[count_bests];

		for (int i = 0; i < count_person; i++)
		{
			generation[i] = new float[count_weights];
			for (int j = 0; j < count_weights; j++) generation[i][j] = (((float)(rand() % 20001)) / 1000) - 10; //((float)rand()) / RAND_MAX;

			child_generation[i] = new float[count_weights];
		}

		for (int i = 0; i < count_epochs; i++)
		{
#pragma omp parallel for
			for (int j = 0; j < count_person; j++)
			{
				errors[j] = GetAvgError(generation[j], solvers[j], inputs, outputs, count_row, get_res, set_weights);
			}

			//отобрать лучших
			for (int j = 0; j < count_bests; j++)
			{
				best_weights[j] = j;
				best_errors[j] = errors[j];
			}
			for (int j = count_bests; j < count_person; j++)
			{
				float max = best_errors[0];
				int max_pos = 0;
				for (int k = 1; k < count_bests; k++)
				{
					if (best_errors[k] > max)
					{
						max = best_errors[k];
						max_pos = k;
					}
				}

				if (errors[j] < max)
				{
					best_weights[max_pos] = j;
					best_errors[max_pos] = errors[j];
				}
			}

			//лучших скрестить
			for (int j = 0; j < count_person; j++)
			{
				int parent1 = rand() % count_bests;
				int parent2 = rand() % count_bests;
				Cross(generation[best_weights[parent1]], generation[best_weights[parent2]], child_generation[j], count_weights);
			}

			//часть детей мутировать
			for (int j = 0; j < count_person; j++)
			{
				if (((float)rand()) / RAND_MAX < mutation_percent) Mut(child_generation[j], count_weights);
			}

			//перейти на след итерацию
			float** tmp = generation;
			generation = child_generation;
			child_generation = tmp;
		}

		float min_error = GetAvgError(generation[0], solvers[0], inputs, outputs, count_row, get_res, set_weights);
		int min_error_pos = 0;
		for (int i = 1; i < count_person; i++)
		{
			float cur_err = GetAvgError(generation[i], solvers[i], inputs, outputs, count_row, get_res, set_weights);
			if (cur_err < min_error)
			{
				min_error = cur_err;
				min_error_pos = i;
			}
		}

		for (int i = 0; i < count_weights; i++)
		{
			res_weights[i] = generation[min_error_pos][i];
		}

		for (int i = 0; i < count_person; i++)
		{
			delete[] generation[i];
			delete[] child_generation[i];
		}
		delete[] generation;
		delete[] child_generation;
		delete[] errors;
		delete[] best_weights;
		delete[] best_errors;

		return min_error;
	}


	/*float startGeneticAlgo(void** solvers, float** inputs, float* outputs, int count_row, int count_col,
	size_t(*get_res)(float* in, float* out, void* solver), void(*set_weights)(float* weights, void* solver),
	int count_weights, int count_person, int count_epochs, int count_bests, float mutation_percent, float* res_weights)
	{
	srand(time(0));



	float** new_weights = (float**)malloc(count_person * sizeof(float*));


	int person_num, epoch_num, best_num, weight_num;
	float* err = (float*)malloc(count_person * sizeof(float));

	int* arr_best_nums = (int*)malloc(count_bests * sizeof(int));


	int* arr_bad_nums = (int*)malloc(count_bests * sizeof(int));

	float best_err = 0;
	float** tmp_res = (float**)malloc(count_person*sizeof(float*));


	for (person_num = 0; person_num < count_person; person_num++)
	{
	tmp_res[person_num] = (float*)malloc(sizeof(float));
	new_weights[person_num] = (float*)malloc(count_weights * sizeof(float));
	for (weight_num = 0; weight_num < count_weights; weight_num++)
	{
	new_weights[person_num][weight_num] = ((float)rand()) / RAND_MAX;
	}
	set_weights(new_weights[person_num], solvers[person_num]);
	}
	epoch_num = 0;

	while (epoch_num < count_epochs)
	{
	for (person_num = 0; person_num < count_person; person_num++)
	{
	int row_num;

	err[person_num] = 0;

	for (row_num = 0; row_num < count_row; row_num++)
	{

	get_res( inputs[row_num], tmp_res[person_num],solvers[person_num]);

	err[person_num] += (outputs[row_num] - tmp_res[person_num][0]) * (outputs[row_num] - tmp_res[person_num][0]);
	}

	err[person_num] /= count_row;
	}

	for (best_num = 0; best_num < count_bests; best_num++)
	{
	int tmp_best_person_num;
	float tmp_best_person_err;
	person_num = 0;
	do
	{
	tmp_best_person_num = person_num;
	tmp_best_person_err = err[person_num];
	person_num++;
	} while (err[tmp_best_person_num] < 0);
	for (; person_num < count_person; person_num++)
	{
	if (tmp_best_person_err > err[person_num] && err[person_num] >= 0)
	{
	tmp_best_person_err = err[person_num];
	tmp_best_person_num = person_num;
	}
	}
	arr_best_nums[best_num] = tmp_best_person_num;
	err[tmp_best_person_num] = -err[tmp_best_person_num];
	}

	best_err = -err[0];
	for (best_num = 0; best_num < count_bests; best_num++)
	{
	int tmp_bad_person_num;
	float tmp_bad_person_err;
	person_num = 0;
	do
	{
	tmp_bad_person_num = person_num;
	tmp_bad_person_err = err[person_num];
	person_num++;
	} while (err[tmp_bad_person_num] < 0 && person_num < count_person);

	for (; person_num < count_person; person_num++)
	{
	if (tmp_bad_person_err < err[person_num])
	{
	tmp_bad_person_err = err[person_num];
	tmp_bad_person_num = person_num;
	}
	}
	arr_bad_nums[best_num] = tmp_bad_person_num;
	err[tmp_bad_person_num] = -err[tmp_bad_person_num];
	}

	for (best_num = 0; best_num < count_bests; best_num += 2)
	{
	int tmp_mask, tmp_l, tmp_r, mut_mask, res_tmp;
	float is_mut;
	for (weight_num = 0; weight_num < count_weights; weight_num++)
	{
	tmp_mask = rand();
	tmp_l = (*((int*)(&(new_weights[arr_best_nums[best_num]])))) & tmp_mask;
	tmp_r = (*((int*)(&(new_weights[arr_best_nums[best_num + 1]])))) & (!tmp_mask);
	res_tmp = (tmp_l | tmp_r);
	new_weights[arr_bad_nums[best_num]][weight_num] = (float)(*((float*)(&res_tmp)));
	is_mut = rand() / RAND_MAX;
	if (is_mut < mutation_percent)
	{
	mut_mask = rand();
	new_weights[arr_bad_nums[best_num]][weight_num] = (float)((*((int*)(&(new_weights[arr_bad_nums[best_num]][weight_num])))) ^ mut_mask);
	}


	tmp_l = (*((int*)(&(new_weights[arr_best_nums[best_num]])))) & (!tmp_mask);
	tmp_r = (*((int*)(&(new_weights[arr_best_nums[best_num + 1]])))) & tmp_mask;
	res_tmp = (tmp_l | tmp_r);
	new_weights[arr_bad_nums[best_num + 1]][weight_num] = (float)(*((float*)(&res_tmp)));
	is_mut = rand() / RAND_MAX;
	if (is_mut < mutation_percent)
	{
	mut_mask = rand();
	new_weights[arr_bad_nums[best_num + 1]][weight_num] = (float)((*((int*)(&(new_weights[arr_bad_nums[best_num + 1]][weight_num])))) ^ mut_mask);
	}

	}
	set_weights(new_weights[arr_bad_nums[best_num]], solvers[arr_bad_nums[best_num]]);
	set_weights(new_weights[arr_bad_nums[best_num + 1]], solvers[arr_bad_nums[best_num + 1]]);
	}

	epoch_num++;
	}

	for (weight_num = 0; weight_num < count_weights; weight_num++)
	{
	res_weights[weight_num] = new_weights[arr_best_nums[0]][weight_num];

	}


	for (person_num = 0; person_num < count_person; person_num++)
	{
	free(new_weights[person_num]);
	}

	free(new_weights);

	free(err);

	free(arr_best_nums);

	free(arr_bad_nums);

	return best_err;

	}*/
}
