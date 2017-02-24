#include "ActivationFunctions.h"
#include <cmath>

#define get_activation_function_for_vec(src, activate_function, dest, size) for(size_t _ = 0; _ < size; _++) { dest[_] = activate_function(src[_]); }

void nnets::calc_activation_function(const float* src, size_t size, ActivationFunctionType af, float* dest)
{
	switch (af) 
	{
	case ActivationFunctionType::BentIdentity:
		get_activation_function_for_vec(src, neurolib_bent_identity, dest, size); 
		break; 
	case ActivationFunctionType::BinaryStep:
		get_activation_function_for_vec(src, neurolib_binary_step, dest, size); 
		break; 
	case ActivationFunctionType::Identity:
		get_activation_function_for_vec(src, neurolib_identity, dest, size);
		break; 
	case ActivationFunctionType::Logistic:
		get_activation_function_for_vec(src, neurolib_logistic, dest, size);
		break; 
	case ActivationFunctionType::Softplus:
		get_activation_function_for_vec(src, neurolib_soft_plus, dest, size);
		break; 
	case ActivationFunctionType::Tanh:
		get_activation_function_for_vec(src, neurolib_tanh, dest, size);
		break; 
	}
}