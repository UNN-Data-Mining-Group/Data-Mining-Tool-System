#include "ActivationFunctions.h"
#include <cmath>
#include <memory>

#define get_activation_function_for_vec(src, activate_function, dest, size) for(int _ = 0; _ < size; _++) { dest[_] = activate_function(src[_]); }

inline void calc_af_par(const float* src, size_t size, nnets::ActivationFunctionType af, float* dest)
{
	switch (af)
	{
	case nnets::ActivationFunctionType::BentIdentity:
#pragma omp parallel for
		get_activation_function_for_vec(src, neurolib_bent_identity, dest, size);
		break;
	case nnets::ActivationFunctionType::BinaryStep:
#pragma omp parallel for
		get_activation_function_for_vec(src, neurolib_binary_step, dest, size);
		break;
	case nnets::ActivationFunctionType::Identity:
#pragma omp parallel for
		get_activation_function_for_vec(src, neurolib_identity, dest, size);
		break;
	case nnets::ActivationFunctionType::Logistic:
#pragma omp parallel for
		get_activation_function_for_vec(src, neurolib_logistic, dest, size);
		break;
	case nnets::ActivationFunctionType::Softplus:
#pragma omp parallel for
		get_activation_function_for_vec(src, neurolib_soft_plus, dest, size);
		break;
	case nnets::ActivationFunctionType::Tanh:
#pragma omp parallel for
		get_activation_function_for_vec(src, neurolib_tanh, dest, size);
		break;
	default:
		throw "Unexpected activation function type";
	}
}

inline void calc_af_seq(const float* src, size_t size, nnets::ActivationFunctionType af, float* dest)
{
	switch (af)
	{
	case nnets::ActivationFunctionType::BentIdentity:
		get_activation_function_for_vec(src, neurolib_bent_identity, dest, size);
		break;
	case nnets::ActivationFunctionType::BinaryStep:
		get_activation_function_for_vec(src, neurolib_binary_step, dest, size);
		break;
	case nnets::ActivationFunctionType::Identity:
		get_activation_function_for_vec(src, neurolib_identity, dest, size);
		//memcpy(dest, src, sizeof(float) * size);
		break;
	case nnets::ActivationFunctionType::Logistic:
		get_activation_function_for_vec(src, neurolib_logistic, dest, size);
		break;
	case nnets::ActivationFunctionType::Softplus:
		get_activation_function_for_vec(src, neurolib_soft_plus, dest, size);
		break;
	case nnets::ActivationFunctionType::Tanh:
		get_activation_function_for_vec(src, neurolib_tanh, dest, size);
		break;
	default:
		throw "Unexpected activation function type";
	}
}

void nnets::calc_activation_function(const float* src, size_t size, ActivationFunctionType af, float* dest)
{
	if (size < 25000)
		calc_af_seq(src, size, af, dest);
	else
		calc_af_par(src, size, af, dest);
}

void nnets::calc_activation_derivatives(const float* src, size_t size, ActivationFunctionType af, float* dest)
{
	switch (af)
	{
	case ActivationFunctionType::BentIdentity:
		get_activation_function_for_vec(src, neurolib_bent_identity_der, dest, size);
		break;
	case ActivationFunctionType::Identity:
		get_activation_function_for_vec(src, neurolib_identity_der, dest, size);
		break;
	case ActivationFunctionType::Logistic:
		get_activation_function_for_vec(src, neurolib_logistic_der, dest, size);
		break;
	case ActivationFunctionType::Softplus:
		get_activation_function_for_vec(src, neurolib_soft_plus_der, dest, size);
		break;
	case ActivationFunctionType::Tanh:
		get_activation_function_for_vec(src, neurolib_tanh_der, dest, size);
		break;
	default:
		throw "Unexpected activation function type";
	}
}

bool nnets::has_derivative(nnets::ActivationFunctionType af)
{
	if (af == ActivationFunctionType::BinaryStep)
		return false;
	return true;
}