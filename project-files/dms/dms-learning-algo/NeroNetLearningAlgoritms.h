#pragma once
#include <list>
#include <map>
#include "GeneticAlgo.h"
#include "BackPropAlgo.h"
#define PRINT_DEBUG_BACK 1

#ifdef PRINT_DEBUG_BACK
#include"stdio.h"
#include <fstream>
#include <iostream>
#endif // PRINT_DEBUG_BACK

using namespace System;
using namespace dms::solvers;
using namespace dms::solvers::neural_nets;
using namespace geneticAlgo;
using namespace backPropAlgo;

namespace dms::neroNetLearningAlgoritms
{
	public ref class NeroNetLearningAlgoritms 
	{
	private:
		void setGeneticParams();
		void setBackPropParams();
		array<float>^ params;
		System::String^ usedAlgo;
		array<System::String^>^ TeacherTypesList;
		array<System::String^>^ ParamsNames;
		float startGenetic(INeuralNetwork^ solver, array<array<float>^>^ train_x, array<float>^ train_y);
		float startBackProp(INeuralNetwork ^ solver, array<array<float>^>^ train_x, array<float>^ train_y);
	public:
		~NeroNetLearningAlgoritms();
		NeroNetLearningAlgoritms();
		void setUsedAlgo(System::String^ usedAlgo_);
		array<System::String^>^ getTeacherTypesList();
		array<System::String^>^ getTeacherTypesList(ISolver^ solver);
		array<float>^ getParams();
		array<System::String^>^ getParamsNames();
		float startLearn(ISolver^ solver, array<array<float>^>^ train_x, array<float>^ train_y);
	};
}
