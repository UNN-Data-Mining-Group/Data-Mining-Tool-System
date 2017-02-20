#include "ConvNN.h"
#include <mkl_cblas.h>

using namespace neurolib;

#define is_a_positive_and_lower_b(a,b) (static_cast<unsigned int>(a) < static_cast<unsigned int>(b))

int ConvNN::solve(float* x, float* y)
{
	size_t inputs = getInputsCount();
	for (size_t i = 0; i < inputs; i++)
		volumes[0]->Values[i] = x[i];

	int weightsIndex = 0;
	for (int volumeIndex = 1; volumeIndex < volumesCount; volumeIndex++)
	{
		Volume* curVolume = volumes[volumeIndex];
		Volume* prevVolume = volumes[volumeIndex - 1];

		switch (curVolume->Type)
		{
			case VolumeType::Convolutional:
			{
				auto cv = static_cast<VolumeConvolutional*>(curVolume);
				im2col(prevVolume->Values, prevVolume->Depth, prevVolume->Height, prevVolume->Width, 
					cv->FilterHeight, cv->FilterWidth, cv->StrideWidth, cv->StrideHeight, cv->Padding, deconvMatrix);

				int m = cv->Depth;
				int n = cv->Width * cv->Height;
				int k = cv->FilterWidth * cv->FilterHeight * prevVolume->Depth;

				cblas_sgemm(CblasRowMajor, CblasNoTrans, CblasNoTrans, m, n, k,
					1.0f, weights[weightsIndex++], k, deconvMatrix, n, 0.0f, cv->Values, n);
				break;
			}
			case VolumeType::FullyConnected:
			{
				im2col(prevVolume->Values, prevVolume->Depth, prevVolume->Height, prevVolume->Width, 
					prevVolume->Height, prevVolume->Width, 1, 1, 0, deconvMatrix);

				int m = curVolume->Depth;
				int n = 1;
				int k = prevVolume->Width * prevVolume->Height * prevVolume->Depth;

				cblas_sgemm(CblasRowMajor, CblasNoTrans, CblasNoTrans, m, n, k,
					1.0f, weights[weightsIndex++], k, deconvMatrix, n, 0.0f, curVolume->Values, n);
				break;
			}
			case VolumeType::Pooling:
			{
				auto pv = static_cast<VolumePooling*>(curVolume);
				pool_max(prevVolume->Values, prevVolume->Depth, prevVolume->Height, prevVolume->Width, 
					pv->FilterHeight, pv->FilterWidth, pv->StrideHeight, pv->StrideWidth, pv->Values);
				break;
			}
			case VolumeType::Activation:
			{
				auto av = static_cast<VolumeActivation*>(curVolume);
				size_t size = static_cast<size_t>(prevVolume->Width) * prevVolume->Height * prevVolume->Depth;
				calc_activation_function(prevVolume->Values, size, av->ActivationFunction, av->Values);
				break;
			}
		}
	}

	size_t outputs = getOutputsCount();
	float* res = volumes[volumesCount - 1]->Values;
	for (size_t i = 0; i < outputs; i++)
	{
		y[i] = res[i];
	}

	return outputs;
}

void ConvNN::pool_max(const float* source, const int channels,
	const int height, const int width, const int kernel_h, const int kernel_w,
	const int stride_h, const int stride_w, float* dest)
{
	const int output_h = (height - kernel_h) / stride_h + 1;
	const int output_w = (width - kernel_w) / stride_w + 1;

	const int channel_size = height * width;

	for (int channel = 0; channel < channels; channel++)
	{
		for (int output_row = 0; output_row < output_h; output_row++)
		{
			int input_row = output_row * stride_h;

			for (int output_col = 0; output_col < output_w; output_col++)
			{
				int input_col = output_col * stride_w;
				const float * kernel = source + input_row * width + input_col;

				float max_src = *kernel;
				for (int kernel_col = 0; kernel_col < kernel_w; kernel_col++)
				{
					float cur_src = kernel[kernel_col];
					if (max_src < cur_src)
						max_src = cur_src;
				}
				dest[output_row * output_w + output_col] = max_src;
			}

			for (int kernel_row = 1; kernel_row < kernel_h; kernel_row++)
			{
				for (int output_col = 0; output_col < output_w; output_col++)
				{
					int input_col = output_col * stride_w;
					const float * kernel = source + input_row * width + input_col;

					float max_src = *kernel;
					for (int kernel_col = 0; kernel_col < kernel_w; kernel_col++)
					{
						float cur_src = kernel[kernel_row * width + kernel_col];
						if (max_src < cur_src)
							max_src = cur_src;
					}

					if (dest[output_row * output_w + output_col] < max_src)
						dest[output_row * output_w + output_col] = max_src;
				}
			}
		}
		source += channel_size;
		dest += output_w * output_h;
	}
}

void ConvNN::im2col(const float* source, const int channels,
	const int height, const int width, const int kernel_h, const int kernel_w,
	const int stride_h, const int stride_w,
	const int padding, float* dest)
{

	const int output_h = (height + 2 * padding - kernel_h) / stride_h + 1;
	const int output_w = (width + 2 * padding - kernel_w) / stride_w + 1;

	const int channel_size = height * width;

	for (int channel = channels; channel--; source += channel_size)
	{
		for (int kernel_row = 0; kernel_row < kernel_h; kernel_row++)
		{
			for (int kernel_col = 0; kernel_col < kernel_w; kernel_col++)
			{
				int input_row = -padding + kernel_row;
				for (int output_rows = output_h; output_rows; output_rows--)
				{
					if (!is_a_positive_and_lower_b(input_row, height)) {
						for (int output_cols = output_w; output_cols; output_cols--)
						{
							*(dest++) = 0;
						}
					}
					else
					{
						int input_col = -padding + kernel_col;
						for (int output_col = output_w; output_col; output_col--)
						{
							if (is_a_positive_and_lower_b(input_col, width))
							{
								*(dest++) = source[input_row * width + input_col];
							}
							else
							{
								*(dest++) = 0;
							}
							input_col += stride_w;
						}
					}
					input_row += stride_h;
				}
			}
		}
	}
}

int ConvNN::getInputsCount() 
{
	Volume* v = volumes[0];
	return v->Width * v->Height * v->Depth;
}

int ConvNN::getOutputsCount() 
{
	Volume* v = volumes[volumesCount - 1];
	return v->Width * v->Height * v->Depth;
}

inline int ConvNN::calcOutputVolume(int input, int filter, int padding, int stride)
{
	int temp = input - filter + 2 * padding;
	if ((temp < 0) || ((temp % stride) != 0))
	{
		freeMemory();
		throw "Invalid filter";
	}
	return temp / stride + 1;
}

inline void ConvNN::clearPtrs()
{
	volumes = nullptr;
	deconvMatrix = nullptr;
	weights = nullptr;
}

inline void ConvNN::freeMemory()
{
	if (volumes != nullptr)
	{
		for (int i = 0; i < volumesCount; i++)
		{
			if (volumes[i] != nullptr)
				delete volumes[i];
		}
		delete[] volumes;
	}
	if (weights != nullptr)
	{
		for (int i = 0; i < weightsCount; i++)
			delete[] weights[i];
		delete[] weights;
	}
	if (deconvMatrix != nullptr) delete[] deconvMatrix;

	clearPtrs();
}

ConvNN::ConvNN(int w, int h, int d, const std::vector<ConvNNLayer*>& layers, float** weights)
{
	clearPtrs();

	if ((w < 1) || (h < 1) || (d < 1) || (layers.size() < 1))
	{
		freeMemory();
		throw "Invalid input parameters size";
	}

	volumesCount = 1;
	for (auto layer = layers.begin(); layer != layers.end(); layer++)
	{
		if ((*layer)->Type == ConvNNLayerType::Pooling)
			volumesCount++;
		else
			volumesCount += 2;
	}

	deconvSize = 0;
	weightsCount = 0;

	volumes = new Volume*[volumesCount];
	for (int i = 0; i < volumesCount; i++)
		volumes[i] = nullptr;

	int volumeIndex = 0;
	volumes[volumeIndex++] = new Volume(w, h, d, VolumeType::Simple);
	for (auto layer = layers.begin(); layer != layers.end(); layer++)
	{
		switch ((*layer)->Type)
		{
			case ConvNNLayerType::Convolution:
			{
				auto conv = static_cast<const ConvNNConvolutionLayer*>(*layer);

				int w = calcOutputVolume(volumes[volumeIndex - 1]->Width, conv->FilterWidth, conv->Padding, conv->StrideWidth);
				int h = calcOutputVolume(volumes[volumeIndex - 1]->Height, conv->FilterHeight, conv->Padding, conv->StrideHeight);
				int d = conv->CountFilters;

				if (d < 1)
				{
					freeMemory();
					throw "Invalid filter";
				}

				size_t temp = w * h * volumes[volumeIndex - 1]->Depth * conv->FilterWidth * conv->FilterHeight;
				if (deconvSize < temp)
					deconvSize = temp;

				volumes[volumeIndex++] = new VolumeConvolutional(w, h, d, 
					conv->FilterWidth, conv->FilterHeight, conv->StrideWidth, conv->StrideHeight, 
					conv->Padding, conv->CountFilters);
				volumes[volumeIndex++] = new VolumeActivation(volumes[volumeIndex - 1], conv->ActivationFunction);

				weightsCount++;
				break;
			}
			case ConvNNLayerType::FullyConnected:
			{
				auto fc = static_cast<const ConvNNFullyConnectedLayer*>(*layer);

				int d = fc->NeuronsCount;

				if (d < 1)
				{
					freeMemory();
					throw "Invalid filter";
				}

				size_t temp = volumes[volumeIndex - 1]->Depth * volumes[volumeIndex - 1]->Width * volumes[volumeIndex - 1]->Height;
				if (deconvSize < temp)
					deconvSize = temp;

				volumes[volumeIndex++] = new Volume(1, 1, d, VolumeType::FullyConnected);
				volumes[volumeIndex++] = new VolumeActivation(volumes[volumeIndex - 1], fc->ActivationFunction);
				
				weightsCount++;
				break;
			}
			case ConvNNLayerType::Pooling:
			{
				auto pool = static_cast<const ConvNNPoolingLayer*>(*layer);
				int w = calcOutputVolume(volumes[volumeIndex - 1]->Width, pool->FilterWidth, 0, pool->StrideWidth);
				int h = calcOutputVolume(volumes[volumeIndex - 1]->Height, pool->FilterHeight, 0, pool->StrideHeight);
				int d = volumes[volumeIndex - 1]->Depth;

				volumes[volumeIndex++] = new VolumePooling(w, h, d, pool->FilterWidth, pool->FilterHeight, pool->StrideWidth, pool->StrideHeight);
				break;
			}
		}
	}

	deconvMatrix = new float[deconvSize];

	this->weights = new float*[weightsCount];
	int weightsIndex = 0;
	for (int i = 0; i < volumesCount; i++)
	{
		int size = 0;
		if (volumes[i]->Type == VolumeType::Convolutional)
		{
			auto v = static_cast<VolumeConvolutional*>(volumes[i]);
			size = v->FilterWidth * v->FilterHeight * volumes[i - 1]->Depth * v->CountFilters;
		}
		else if (volumes[i]->Type == VolumeType::FullyConnected)
		{
			size = volumes[i - 1]->Width * 
				volumes[i - 1]->Height * 
				volumes[i - 1]->Depth * 
				volumes[i]->Depth;
		}

		if (size > 0)
		{
			this->weights[weightsIndex] = new float[size];
			for (int i = 0; i < size; i++)
			{
				this->weights[weightsIndex][i] = weights[weightsIndex][i];
			}
			weightsIndex++;
		}
	}
}