#include "ActivateFunctionsFactory.h"
#include "BentIdentityAF.h"
#include "BinaryStepAF.h"
#include "IdentityAF.h"
#include "LogisticAF.h"
#include "SoftplusAF.h"
#include "TanhAF.h"

using namespace dms::solvers::neural_nets;

array<System::String^>^ ActivateFunctionsFactory::GetAllTypeNames()
{
	return gcnew array<System::String^>
	{
		"BentIdentity",
		"BinaryStep",
		"Identity",
		"Logistic",
		"Softplus",
		"Tanh"
	};
}

IActivateFunction^ ActivateFunctionsFactory::CreateActivateFunction(System::String^ name)
{
	if (name->Equals("BentIdentity"))
		return gcnew BentIdentityAF();
	if (name->Equals("BinaryStep"))
		return gcnew BinaryStepAF();
	if (name->Equals("Identity"))
		return gcnew IdentityAF();
	if (name->Equals("Logistic"))
		return gcnew LogisticAF();
	if (name->Equals("Softplus"))
		return gcnew SoftplusAF();
	if (name->Equals("Tanh"))
		return gcnew TanhAF();
}