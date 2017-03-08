#include "LearningAlgoritms.h"

float get_res_(void* solver, float* in)
{
	return 0;
}

int get_count_weights(void* solver)
{
	return 5;
}

float* get_weights_(void* solver)
{
	float* tmp = (float*)malloc(sizeof(float)*5);
	return tmp;
}

void set_weights_(void* solver, float* weights)
{

}

namespace dms::learningAlgoritms
{
	LearningAlgoritms::LearningAlgoritms()
	{		
		TeacherTypesList = gcnew array<String^>(2);
		TeacherTypesList[0] = "Генетический алгоритм";
		TeacherTypesList[1] = "Обратное распространение ошибки";
		params = gcnew array< float >(4);
		params[0] = 1.0f;
		params[1] = 0.4f;
		params[2] = 0.5f;
		params[3] = 5.0f;		
	}

	array<System::String^>^ LearningAlgoritms::getTeacherTypesList()
	{
		return TeacherTypesList;
	}

	array<float>^ LearningAlgoritms::getParams()
	{
		return params;
	}

	float LearningAlgoritms::startLearn(ISolver^ solver, array<array<float>^>^ train_x, array<float>^ train_y)
	{
		int count_person = params[0];
		void** solvers = new void*[count_person];
		int a = 0;
		for (int  i = 0; i < count_person; i++)
		{
			solvers[i] =(void*) a;
		}
		float** inputs = new float*[train_x->GetLength(0)];
		float* outputs = new float[train_y->Length];
		for (int i = 0; i < train_x->GetLength(0); i++)
		{
			inputs[i] = new float[train_x[i]->Length];
			for (int j = 0; j < train_x[i]->Length; j++)
			{
				inputs[i][j] = train_x[i][j];
			}			
		}
		for (int i = 0; i < train_y->Length; i++)
		{
			outputs[i] = train_y[i];
		}
		float* params_ = new float[params->Length];
		for (int i = 0; i < params->Length; i++)
		{
			params_[i] = params[i];
		}
		float(*get_res)(void* solver, float* in) = get_res_;

		void(*set_weights)(void* solver, float* weights) = set_weights_;

		float* res_weights = get_weights_(&solver);
		
		int count_weights = get_count_weights(&solver);

		int count_epochs = params[1];

		int count_bests = params[2];

		return startGeneticAlgo(solvers, inputs, outputs, train_y->Length, train_x[0]->Length,
			get_res, set_weights, count_weights, count_person, count_epochs,
			 count_bests, res_weights);
	}
	

}