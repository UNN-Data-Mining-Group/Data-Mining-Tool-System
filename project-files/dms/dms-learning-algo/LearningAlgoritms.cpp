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
		std::map<std::string, void*>* operations = (std::map<std::string, void*>*)solver->getOperations();
		solvers[0] = solver->getNativeSolver();

		typedef size_t(*GetWeightsCount)(void*);
		GetWeightsCount getWeightsCount = (GetWeightsCount)(*operations)["getWeightsCount"];
		size_t count_weights = getWeightsCount(solvers[0]);
		float* res_weights = new float[count_weights];

		typedef size_t(*GetAllWeights)(float*, void*);
		((GetAllWeights)((*operations)["getAllWeights"]))(res_weights, solvers[0]);

//		(*_opers)["getAllWeights"] = nnets_perceptron::getAllWeightsPerc;

		typedef void(*SetAllWeights)(float*, void*);
		SetAllWeights set_weights = (SetAllWeights)((*operations)["setAllWeights"]);
//		(*_opers)["setAllWeights"] = nnets_perceptron::setAllWeightsPerc;

		typedef size_t(*Solve)(float*, float*, void*);
		Solve get_res = (Solve)((*operations)["solve"]);
//		(*_opers)["solve"] = nnets_perceptron::solvePerc;
//		(*_opers)["getWeightsCount"] = nnets_perceptron::getWeightsCountPerc;
		typedef void*(*CopySolver)(void*);
		CopySolver copySolver = (CopySolver)((*operations)["copySolver"]);
//		(*_opers)["copySolver"] = nnets_perceptron::copyPerc;
//		(*_opers)["freeSolver"] = nnets_perceptron::freePerc;
		for (int  i = 1; i < count_person; i++)
		{
			solvers[i] = copySolver(solvers[0]);
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

		int count_epochs = params[1];

		int count_bests = params[2];

		return startGeneticAlgo(solvers, inputs, outputs, train_y->Length, train_x[0]->Length,
			get_res, set_weights, count_weights, count_person, count_epochs,
			 count_bests, res_weights);
	}
	

}