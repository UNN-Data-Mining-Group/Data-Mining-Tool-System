#pragma once
#include "KohonenLearning.h"

namespace dms::kohonen_learning_algorithms
{
	public ref class KohonenTrainer
	{
	public:
		virtual array<System::String^>^ getParamsNames() = 0;
		virtual array<float>^ getParams() = 0;
		virtual bool canTrain(dms::solvers::ISolver^ solver) = 0;
		virtual System::String^ getType() = 0;
		virtual float learn(dms::solvers::ISolver^ solver,
			array<array<float>^>^ train_x, array<float>^ train_y) = 0;
	protected:
		nnets_kohonen_learning::OperatorList initFuncList(std::map<std::string, void*>* map);
		nnets_kohonen_learning::Selection initSelection(
			array<array<float>^>^ train_x, array<float>^ train_y);
		void freeSelection(nnets_kohonen_learning::Selection& sel);
	};

	public ref class KohonenClassifier : public KohonenTrainer
	{
	public:
		KohonenClassifier();
		array<System::String^>^ getParamsNames() override;
		array<float>^ getParams() override;
		bool canTrain(dms::solvers::ISolver^ solver) override;
		System::String^ getType() override;
		float learn(dms::solvers::ISolver^ solver,
			array<array<float>^>^ train_x, array<float>^ train_y) override;
	private:
		array<System::String^>^ paramNames;
		array<float>^ paramValues;
	};

	public ref class KohonenSelfOrganizer : public KohonenTrainer
	{
	public:
		KohonenSelfOrganizer();
		array<System::String^>^ getParamsNames() override { return paramNames; }
		array<float>^ getParams() override { return paramValues; }
		bool canTrain(dms::solvers::ISolver^ solver) override;
		System::String^ getType() override { return "Самоорганизация Кохонена"; }
		float learn(dms::solvers::ISolver^ solver,
			array<array<float>^>^ train_x, array<float>^ train_y) override;
	private:
		array<System::String^>^ paramNames;
		array<float>^ paramValues;
	};
}
