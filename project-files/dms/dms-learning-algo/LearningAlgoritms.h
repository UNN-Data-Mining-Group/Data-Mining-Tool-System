#pragma once
#include <list>
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
	public:
		
		LearningAlgoritms();
		array<System::String^>^ getTeacherTypesList();
		array<float>^ getParams();
		float startLearn(ISolver^ solver, array<array<float>^>^ train_x, array<float>^ train_y);
	};
}
