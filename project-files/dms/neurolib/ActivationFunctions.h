#pragma once

namespace neurolib
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
	#define neurolib_logistic_der(x) (1.0f - neurolib_logistic(x))
	#define neurolib_soft_plus_der(x) (1.0f / (1.0f + std::exp(-x)))
	#define neurolib_tanh_der(x) (1.0f - std::pow(neurolib_tanh(x), 2))

	//Useful constructions
	#define get_activation_function_for_vec(activate_function, vec, size) for(int _ = 0; _ < size; _++) { vec[_] = activate_function(vec[_]); }
	#define get_activation_function_for_layer(af_type, layer, layer_size) switch (af_type) \
			{\
			case ActivationFunctionType::BentIdentity:\
				get_activation_function_for_vec(neurolib_bent_identity, layer, layer_size);\
				break;\
			case ActivationFunctionType::BinaryStep:\
				get_activation_function_for_vec(neurolib_binary_step, layer, layer_size);\
				break;\
			case ActivationFunctionType::Identity:\
				get_activation_function_for_vec(neurolib_identity, layer, layer_size);\
				break;\
			case ActivationFunctionType::Logistic:\
				get_activation_function_for_vec(neurolib_logistic, layer, layer_size);\
				break;\
			case ActivationFunctionType::Softplus:\
				get_activation_function_for_vec(neurolib_soft_plus, layer, layer_size);\
				break;\
			case ActivationFunctionType::Tanh:\
				get_activation_function_for_vec(neurolib_tanh, layer, layer_size);\
				break;\
			}
}