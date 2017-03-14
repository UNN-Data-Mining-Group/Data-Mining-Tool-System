#include "KohonenNet.h"
#include "mkl_cblas.h"

using namespace nnets_kohonen;

NeuronIndex nnets_kohonen::getWinner(const float* x, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return kn->getWinner(x);
}

size_t nnets_kohonen::solve(const float* x, float* y, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return kn->solve(x, y);
}

int2d nnets_kohonen::getNetDimention(void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return int2d(kn->neurons_width, kn->neurons_height);
}

std::vector<NeuronIndex> nnets_kohonen::getNeighbours(const NeuronIndex n,
	int radius, void* obj)
{
	int2d dim = getNetDimention(obj);
	int3d directions[] = 
	{
		int3d(1, -1, 0), int3d(1, 0, -1), int3d(0, 1, -1),
		int3d(-1, 1, 0), int3d(-1, 0, 1), int3d(0, -1, 1)
	};

	std::vector<NeuronIndex> res;

	if (radius <= 0)
		res.push_back(n);
	else
	{
		int3d cur_coord(n.cube.x - radius, n.cube.y, n.cube.z + radius);
		for (int side = 0; side < 6; side++)
		{
			for (int elem = 0; elem < radius; elem++)
			{
				NeuronIndex cur_n(cur_coord);

				if ((cur_n.even_r.x >= 0) && (cur_n.even_r.x < dim.x) && 
					(cur_n.even_r.y >= 0) && (cur_n.even_r.y < dim.y))
					res.push_back(cur_n);

				cur_coord.x += directions[side].x;
				cur_coord.y += directions[side].y;
				cur_coord.z += directions[side].z;
			}
		}
	}

	return res;
}

void nnets_kohonen::addmultWeights(const NeuronIndex n, float alpha, float beta, float* x, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	int neuron_index = n.even_r.y * kn->neurons_width + n.even_r.x;

	float* w = kn->weights + neuron_index * kn->x_size;
	for (int i = 0; i < kn->x_size; i++)
		w[i] = alpha * w[i] + beta * x[i];
}

void nnets_kohonen::setY(NeuronIndex n, const float* y, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	kn->setClass(n, y);
}

KohonenNet::KohonenNet(int inputs_count, int outputs_count,
	int koh_width, int koh_height, bool use_normalization)
{
	use_norm_x = use_normalization;

	neurons_width = koh_width;	neurons_height = koh_height;
	x_size = inputs_count;		y_size = outputs_count;

	int layer_size = neurons_width * neurons_height;

	x_internal = new float[x_size];

	weights = new float[layer_size * x_size];
	for (int i = 0; i < layer_size * x_size; i++)
		weights[i] = 0.0f;

	classes = new float*[layer_size];
	for (int i = 0; i < layer_size; i++)
	{
		classes[i] = new float[y_size];
		for (int j = 0; j < y_size; j++)
			classes[i][j] = 0.0f;
	}

	kohonen_layer = new float[layer_size];
}

KohonenNet::KohonenNet(KohonenNet& kn) : 
	KohonenNet(kn.x_size, kn.y_size, kn.neurons_width, kn.neurons_height, kn.use_norm_x)
{
	int layer_size = neurons_width * neurons_height;
	for (int i = 0; i < layer_size * x_size; i++)
		weights[i] = kn.weights[i];

	for (int i = 0; i < layer_size; i++)
		for (int j = 0; j < y_size; j++)
			classes[i][j] = kn.classes[i][j];
}

size_t KohonenNet::getInputsCount() { return x_size; }
size_t KohonenNet::getOutputsCount() { return y_size; }
void KohonenNet::setWeights(float* weights)
{
	for (int i = 0; i < neurons_width * neurons_height * x_size; i++)
		this->weights[i] = weights[i];
}

void KohonenNet::setClass(NeuronIndex n, const float* y)
{
	int neuron_index = n.even_r.y * neurons_width + n.even_r.x;
	float* cur_class = classes[neuron_index];
	for (int i = 0; i < y_size; i++)
		cur_class[i] = y[i];
}

void KohonenNet::setUseNormalization(bool norm)
{
	use_norm_x = norm;
}

size_t KohonenNet::getWeights(float* weights)
{
	for (int i = 0; i < neurons_width * neurons_height * x_size; i++)
		weights[i] = this->weights[i];
	return neurons_width * neurons_height * x_size;
}

size_t KohonenNet::getClass(NeuronIndex n, float* y)
{
	int neuron_index = n.even_r.y * neurons_width + n.even_r.x;
	float* cur_class = classes[neuron_index];
	for (int i = 0; i < y_size; i++)
		y[i] = cur_class[i];
	return y_size;
}

size_t KohonenNet::getWeightsMatrixSize()
{
	return neurons_width * neurons_height * x_size;
}

KohonenNet::~KohonenNet()
{
	delete[] kohonen_layer;
	for (int i = 0; i < neurons_width * neurons_height; i++)
		delete[] classes[i];
	delete[] classes;
	delete[] weights;
	delete[] x_internal;
}

NeuronIndex KohonenNet::getWinner(const float* x)
{
	const float* input = x;
	if (use_norm_x)
	{
		float norm = 0.0f;
		for (int i = 0; i < x_size; i++)
			norm += x[i] * x[i];
		norm = std::sqrtf(norm);
		for (int i = 0; i < x_size; i++)
			x_internal[i] = x[i] / norm;

		input = x_internal;
	}

	cblas_sgemv(CblasRowMajor, CblasNoTrans,
		neurons_width * neurons_height, x_size,
		1.0f, weights, x_size,
		input, 1, 0.0f, kohonen_layer, 1);
	int winner_index = cblas_isamax(neurons_width * neurons_height, kohonen_layer, 1);
	int row = winner_index / neurons_width;
	int col = winner_index - row * neurons_width;
	
	return NeuronIndex(int2d{ col, row });
}

size_t KohonenNet::solve(const float* x, float* y)
{
	NeuronIndex n = getWinner(x);
	float* cur_class = classes[n.even_r.y * neurons_width + n.even_r.x];
	for (int i = 0; i < y_size; i++)
		y[i] = cur_class[i];
	return y_size;
}