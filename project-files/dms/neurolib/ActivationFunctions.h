#pragma once

namespace nnets
{
	enum class ActivationFunctionType
	{
		BentIdentity,
		BinaryStep,
		Identity,
		Logistic,
		Softplus,
		Tanh
	};

	//Activation functions
#define neurolib_bent_identity(x) ((std::sqrt(x*x + 1.0f) - 1.0f) / 2.0f + x)
#define neurolib_binary_step(x) (x >= 0.0f ? 1.0f : 0.0f)
#define neurolib_identity(x) (x)
#define neurolib_logistic(x) (1.0f / (1.0f + std::exp(-x)))
#define neurolib_soft_plus(x) (std::log(1.0f + std::exp(x)))
#define neurolib_tanh(x) (2.0f / (1.0f + std::exp(-2.0f*x)) - 1.0f)

//Derivatives
#define neurolib_bent_identity_der(x) (x / (2.0f * std::sqrt(x*x + 1.0f)) + 1.0f)
#define neurolib_identity_der(x) (1.0f)
#define neurolib_logistic_der(x) ((1.0f - neurolib_logistic(x))* neurolib_logistic(x))
#define neurolib_soft_plus_der(x) (1.0f / (1.0f + std::exp(-x)))
#define neurolib_tanh_der(x) (1.0f - std::pow(neurolib_tanh(x), 2))

//Useful constructions
void calc_activation_function(const float* src, size_t size, ActivationFunctionType af, float* dest);
void calc_activation_derivatives(const float* src, size_t size, ActivationFunctionType af, float* dest);
bool has_derivative(ActivationFunctionType af);
}