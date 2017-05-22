

#include "NeroNetLearningAlgoritms.h"
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

namespace dms::neroNetLearningAlgoritms
{
	void NeroNetLearningAlgoritms::setGeneticParams()
	{
		ParamsNames = gcnew array<String^>(4);
		ParamsNames[0] = "Количество особей";
		ParamsNames[1] = "Количество эпох";
		ParamsNames[2] = "Процент особей для скрещивания";
		ParamsNames[3] = "Коэффициент мутации";
		params = gcnew array< float >(4);
		params[0] = 100.0f;
		params[1] = 100.0f;
		params[2] = 50.0f;
		params[3] = 0.2f;
	}
	void NeroNetLearningAlgoritms::setBackPropParams()
	{
		ParamsNames = gcnew array<String^>(2);
		ParamsNames[0] = "Число итераций";
		ParamsNames[1] = "Скорость обучения";
		params = gcnew array< float >(2);
		params[0] = 100;
		params[1] = 0.01f;
	}
	float NeroNetLearningAlgoritms::startGenetic(INeuralNetwork ^ solver, array<array<float>^>^ train_x, array<float>^ train_y)
	{
		int count_person = params[0];
		void* parent_Solver;
		void** solvers = new void*[count_person];
		int a = 0;
		std::map<std::string, void*>* operations = (std::map<std::string, void*>*)solver->getOperations();
		parent_Solver = solver->getNativeSolver();

		typedef size_t(*GetWeightsCount)(void*);
		GetWeightsCount getWeightsCount = (GetWeightsCount)(*operations)["getWeightsCount"];
		size_t count_weights = getWeightsCount(parent_Solver);
		float* res_weights = new float[count_weights];

		typedef size_t(*GetAllWeights)(float*, void*);
		((GetAllWeights)((*operations)["getAllWeights"]))(res_weights, parent_Solver);

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

		typedef void(*FreeSolver)(void*&);
		FreeSolver freeSolver = (FreeSolver)((*operations)["freeSolver"]);
		//		(*_opers)["freeSolver"] = nnets_perceptron::freePerc;
		for (int i = 0; i < count_person; i++)
		{
			solvers[i] = copySolver(parent_Solver);
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

		int percent_bests = params[2];

		int count_bests = (float)count_person / 100.0 * percent_bests;

		if (count_bests == 0)
		{
			count_bests = 2;
		}

		if (count_bests % 2 != 0)
		{
			if (count_person - count_bests == 0)
			{
				count_bests--;

			}
			else
			{
				count_bests++;
			}
		}

		float mutation_percent = params[3] / 100.0f;

		float res = startGeneticAlgo(solvers, inputs, outputs, train_y->Length, train_x[0]->Length,
			get_res, set_weights, count_weights, count_person, count_epochs,
			count_bests, mutation_percent, res_weights);

		set_weights(res_weights, parent_Solver);


	

		for (int i = 0; i < train_x->GetLength(0); i++)
		{
			delete[] inputs[i];
		}
		delete[] inputs;
		delete[] outputs;
		delete[] res_weights;

		for (int i = 0; i < count_person; i++)
		{
			freeSolver(solvers[i]);
		}
		delete[] solvers;

		
		solver->FetchNativeParameters();
		return res;
	}

	float NeroNetLearningAlgoritms::startBackProp(INeuralNetwork ^ solver, array<array<float>^>^ train_x, array<float>^ train_y)
	{
#ifdef PRINT_DEBUG_BACK
		std::ofstream fout;
		fout.open("BackProp_debug1.txt", std::ios_base::trunc);
		fout.close();
#endif // PRINT_DEBUG_BACK

		void* result_solver;
		int a = 0;
		std::map<std::string, void*>* operations = (std::map<std::string, void*>*)solver->getOperations();
		result_solver = solver->getNativeSolver();

		typedef size_t(*Solve)(float*, float*, void*);
		Solve get_res = (Solve)((*operations)["solve"]);

		typedef size_t(*SetWeightsVector)(float*, int, void*);
		SetWeightsVector set_next_weights = (SetWeightsVector)((*operations)["setWeightsVector"]);

		typedef size_t(*GetIterationDerivatives)(float*, int, void*);
		GetIterationDerivatives get_next_grads = (GetIterationDerivatives)((*operations)["getIterationDerivatives"]);

		typedef size_t(*GetIterationValues)(float*, int, void*);
		GetIterationValues get_next_activate = (GetIterationValues)((*operations)["getIterationValues"]);


		typedef int(*GetIterationsCount)(void*);
		int count_layers = ((GetIterationsCount)((*operations)["getIterationsCount"]))(result_solver);

		size_t* count_neuron_per_layer = new size_t[count_layers];
		typedef int(*GetIterationSizes)(size_t*, void*);
		((GetIterationSizes)((*operations)["getIterationSizes"]))(count_neuron_per_layer, result_solver);

		float** inputs = new float*[train_x->GetLength(0)];
		float** outputs = new float*[train_y->Length];

		for (int i = 0; i < train_x->GetLength(0); i++)
		{
			inputs[i] = new float[train_x[i]->Length];
			for (int j = 0; j < train_x[i]->Length; j++)
			{
				inputs[i][j] = train_x[i][j];
			}
		}


		int out_size = 1;
		for (int i = 0; i < train_y->Length; i++)
		{
			outputs[i] = new float[out_size];
			for (int j = 0; j < out_size; j++)
			{
				outputs[i][0] = train_y[i];
			}
		}

		typedef int(*GetWeightsVectorsCount)(void*);
		int count_lauer_to_layer = ((GetWeightsVectorsCount)((*operations)["getWeightsVectorsCount"]))(result_solver);
		float** res_weights = new float*[count_lauer_to_layer];
		
		int* count_weights_per_lauer = new int[count_lauer_to_layer];
		typedef size_t(*GetWeightsVectorSize)(int,void*);
		for (int i = 0; i < count_lauer_to_layer; i++)
		{
#ifdef PRINT_DEBUG_BACK
			fout.open("BackProp_debug1.txt", std::ios::app);
			fout << "Start sigma*grads, " << i;			
			fout.close();

#endif // PRINT_DEBUG_BACK
			size_t tm =  ((GetWeightsVectorSize)((*operations)["getWeightsVectorSize"]))(i, result_solver);
			count_weights_per_lauer[i] = tm;
			res_weights[i] = new float[count_weights_per_lauer[i]];
		}
		


		int count_steps = params[0];
		float start_lr = params[1];
		
		float res = startBackPropAlgo(result_solver, inputs, outputs, train_y->Length, train_x[0]->Length,
			get_res, set_next_weights,get_next_grads,get_next_activate,count_layers, count_neuron_per_layer,count_steps,
			res_weights,
			count_lauer_to_layer, count_weights_per_lauer,
			start_lr, out_size);

//		set_weights(res_weights, result_Solver);




		for (int i = 0; i < train_x->GetLength(0); i++)
		{
			delete[] inputs[i];
		}
		for (int i = 0; i < count_lauer_to_layer; i++)
		{
			delete[] res_weights[i];
		}
		for (int i = 0; i < train_y->Length; i++)
		{
			delete[] outputs[i];
		}
		delete[] inputs;
		delete[] outputs;
		delete[] count_neuron_per_layer;
		delete[] res_weights;
		


		solver->FetchNativeParameters();
		return res;
	}

	NeroNetLearningAlgoritms::~NeroNetLearningAlgoritms()
	{
		delete[] TeacherTypesList;
		delete[] ParamsNames;
		delete[] params;
	}



	NeroNetLearningAlgoritms::NeroNetLearningAlgoritms()
	{		
		TeacherTypesList = gcnew array<String^>(2);
		TeacherTypesList[0] = "Генетический алгоритм";
		TeacherTypesList[1] = "Обратное распространение ошибки";
		usedAlgo = TeacherTypesList[0];
		setGeneticParams();
	}

	void NeroNetLearningAlgoritms::setUsedAlgo(System::String ^ usedAlgo_)
	{
		usedAlgo = usedAlgo_;
		if (usedAlgo == TeacherTypesList[0])
		{
			setGeneticParams();
		}
		else
		{
			setBackPropParams();
		}
	}

	array<System::String^>^ NeroNetLearningAlgoritms::getTeacherTypesList()
	{
		return TeacherTypesList;
	}

	array<System::String^>^ NeroNetLearningAlgoritms::getTeacherTypesList(ISolver ^ solver)
	{
		throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
	}

	array<float>^ NeroNetLearningAlgoritms::getParams()
	{
		return params;
	}

	array<System::String^>^ NeroNetLearningAlgoritms::getParamsNames()
	{
		return ParamsNames;
	}



	float NeroNetLearningAlgoritms::startLearn(ISolver^ solver, array<array<float>^>^ train_x, array<float>^ train_y)
	{
		float res = 0;

		using dms::solvers::neural_nets::kohonen::KohonenManaged;
		KohonenManaged^ kn = dynamic_cast<KohonenManaged^>(solver);
		if (kn != nullptr)
		{
			array<array<float>^>^ out = gcnew array<array<float>^>(train_y->Length);
			for (int i = 0; i < train_y->Length; i++)
			{
				out[i] = gcnew array<float>(1);
				out[i][0] = train_y[i];
			}
			kn->setClasses(out);
		}

		if (usedAlgo->Equals(TeacherTypesList[0])) // startGenetic
			res = startGenetic(static_cast<INeuralNetwork^>(solver), train_x, train_y);
		else
		{
			if (usedAlgo->Equals(TeacherTypesList[1]))
				res = startBackProp(static_cast<INeuralNetwork^>(solver), train_x, train_y);
			else
				throw gcnew System::ArgumentException("This algorithm is not supported yet", usedAlgo);
		}
			
			
		return res;
	}
	

}