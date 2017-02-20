#include "GeneticAlgo.h"
namespace geneticAlgo
{
	float startGeneticAlgo(void** solvers, float** inputs, float* outputs, int count_row, int count_col, 
		float(*get_res)(void* solver, float* in), void(*set_weights)(void* solver, float* weights),
		int count_weights, int count_person, int count_epochs, int count_bests, float* res_weights)
	{
		float** new_weights = (float**)malloc(count_person * sizeof(float*));
		int person_num, epoch_num = 0, best_num, weight_num;
		float* err = (float*)malloc(count_person * sizeof(float));
		int* arr_best_nums = (int*)malloc(count_bests * sizeof(int));
		int* arr_bad_nums = (int*)malloc(count_bests * sizeof(int));
		float best_err = 0;

		free(res_weights);
		res_weights = (float*)malloc(count_weights * sizeof(float));

		for (person_num = 0; person_num < count_person; person_num++)
		{
			new_weights[person_num] = (float*)malloc(count_weights * sizeof(float));
			for (weight_num = 0; weight_num < count_weights; weight_num++)
			{
				new_weights[person_num][weight_num] = ((float)rand()) / RAND_MAX;
			}
			set_weights(solvers[person_num], new_weights[person_num]);
		}

		while (epoch_num < count_epochs)
		{
			for (person_num = 0; person_num < count_person; person_num++)
			{
				int row_num;
				float tmp_res;
				err[person_num] = 0;
				for (row_num = 0; row_num < count_row; row_num++)
				{
					tmp_res = get_res(solvers[person_num], inputs[row_num]);
					err[person_num] += (outputs[row_num] - tmp_res) * (outputs[row_num] - tmp_res);
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
				} while (err[tmp_bad_person_num] < 0);

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
				int tmp_mask, tmp_l, tmp_r;
				for (weight_num = 0; weight_num < count_weights; weight_num++)
				{
					tmp_mask = rand();
					tmp_l = (int)new_weights[arr_best_nums[best_num]] & tmp_mask;
					tmp_r = (int)new_weights[arr_best_nums[best_num + 1]] & (!tmp_mask);
					new_weights[arr_bad_nums[best_num]][weight_num] = (float)(tmp_l | tmp_r);

					tmp_l = (int)new_weights[arr_best_nums[best_num]] & (!tmp_mask);
					tmp_r = (int)new_weights[arr_best_nums[best_num + 1]] & tmp_mask;
					new_weights[arr_bad_nums[best_num + 1]][weight_num] = (float)(tmp_l | tmp_r);
				}
				set_weights(solvers[arr_bad_nums[best_num]], new_weights[arr_bad_nums[best_num]]);
				set_weights(solvers[arr_bad_nums[best_num + 1]], new_weights[arr_bad_nums[best_num + 1]]);
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

	}
}
