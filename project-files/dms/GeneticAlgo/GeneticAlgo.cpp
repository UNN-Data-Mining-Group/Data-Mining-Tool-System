#include "GeneticAlgo.h"

#define GENETIC_DEBUG 0

#ifdef GENETIC_DEBUG
#include <fstream>
#include <iostream>
using namespace std;
#endif // GENETIC_DEBUG
namespace geneticAlgo
{


	float startGeneticAlgo(void** solvers, float** inputs, float* outputs, int count_row, int count_col, 
		float(*get_res)(void* solver, float* in), void(*set_weights)(void* solver, float* weights),
		int count_weights, int count_person, int count_epochs, int count_bests, float* res_weights)
	{

#ifdef GENETIC_DEBUG
		ofstream fout;
		fout.open("Genetic_debug.txt", ios_base::trunc);
		fout << "count_row = " << count_row << endl;
		fout << "count_col = " << count_col << endl;
		fout << "count_weights = " << count_weights << endl;
		fout << "count_person = " << count_person << endl;
		fout << "count_epochs = " << count_epochs << endl;
		fout << "count_bests = " << count_bests << endl;
		fout.close();
#endif // GENETIC_DEBUG


		float** new_weights = (float**)malloc(count_person * sizeof(float*));
#ifdef GENETIC_DEBUG
		fout.open("Genetic_debug.txt", ios::app);
		fout << "Created new_weights" << endl;		
		fout.close();
#endif // GENETIC_DEBUG

		int person_num, epoch_num = 0, best_num, weight_num;
		float* err = (float*)malloc(count_person * sizeof(float));
#ifdef GENETIC_DEBUG
		fout.open("Genetic_debug.txt", ios::app);
		fout << "Created err" << endl;
		fout.close();
#endif // GENETIC_DEBUG

		int* arr_best_nums = (int*)malloc(count_bests * sizeof(int));
#ifdef GENETIC_DEBUG
		fout.open("Genetic_debug.txt", ios::app);
		fout << "Created arr_best_nums" << endl;
		fout.close();
#endif // GENETIC_DEBUG

		int* arr_bad_nums = (int*)malloc(count_bests * sizeof(int));
#ifdef GENETIC_DEBUG
		fout.open("Genetic_debug.txt", ios::app);
		fout << "Created arr_bad_nums" << endl;
		fout.close();
#endif // GENETIC_DEBUG
		float best_err = 0;

		free(res_weights);
#ifdef GENETIC_DEBUG
		fout.open("Genetic_debug.txt", ios::app);
		fout << "Free res_weights" << endl;
		fout.close();
#endif // GENETIC_DEBUG

		res_weights = (float*)malloc(count_weights * sizeof(float));
#ifdef GENETIC_DEBUG
		fout.open("Genetic_debug.txt", ios::app);
		fout << "Realloc res_weights" << endl;
		fout.close();
#endif // GENETIC_DEBUG

		for (person_num = 0; person_num < count_person; person_num++)
		{
			new_weights[person_num] = (float*)malloc(count_weights * sizeof(float));
			for (weight_num = 0; weight_num < count_weights; weight_num++)
			{
				new_weights[person_num][weight_num] = ((float)rand()) / RAND_MAX;
			}
			set_weights(solvers[person_num], new_weights[person_num]);
		}
#ifdef GENETIC_DEBUG
		fout.open("Genetic_debug.txt", ios::app);
		fout << "Created and set new_weights" << endl;
		fout.close();
#endif // GENETIC_DEBUG

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
#ifdef GENETIC_DEBUG
			fout.open("Genetic_debug.txt", ios::app);
			fout << "epoch_num = "<< epoch_num << ", of " << count_epochs << endl;
			for (person_num = 0; person_num < count_person; person_num++)
			{
				fout << "	person_num = " << person_num << ", err = " << err[person_num] << endl;
			}
			fout.close();
#endif // GENETIC_DEBUG
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
#ifdef GENETIC_DEBUG
			fout.open("Genetic_debug.txt", ios::app);
			fout << "epoch_num = " << epoch_num << ", of " << count_epochs << endl;
			for (best_num = 0; best_num < count_bests; best_num++)
			{
				fout << "	best_num = " << arr_best_nums[best_num] << ", err = " << -err[arr_best_nums[best_num]] << endl;
			}
			fout.close();
#endif // GENETIC_DEBUG
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
#ifdef GENETIC_DEBUG
			fout.open("Genetic_debug.txt", ios::app);
			fout << "epoch_num = " << epoch_num << ", of " << count_epochs << endl;
			for (best_num = 0; best_num < count_bests; best_num++)
			{
				fout << "	bad_num = " << arr_bad_nums[best_num] << ", err = " << -err[arr_bad_nums[best_num]] << endl;
			}
			fout.close();
#endif // GENETIC_DEBUG
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
