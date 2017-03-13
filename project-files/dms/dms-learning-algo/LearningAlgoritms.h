#pragma once
#include <list>
#include <map>
#include "GeneticAlgo.h"
using namespace System;
using namespace dms::solvers;
using namespace geneticAlgo;


namespace dms::learningAlgoritms
{
	public ref class LearningAlgoritms
	{
	private:
		array<float>^ params;
		array<System::String^>^ TeacherTypesList;
		array<System::String^>^ ParamsNames;
	public:
		
		LearningAlgoritms();
		array<System::String^>^ getTeacherTypesList();
		array<float>^ getParams();
		array<System::String^>^ getParamsNames();
		float startLearn(ISolver^ solver, array<array<float>^>^ train_x, array<float>^ train_y);
	};
}
