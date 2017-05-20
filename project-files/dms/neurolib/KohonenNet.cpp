#include "KohonenNet.h"
#include "KohonenPretrain.h"
#include <algorithm>
#include "mkl_cblas.h"

using namespace nnets_kohonen;

int nnets_kohonen::getDistance(int neuron1, int neuron2, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);

	NeuronIndex n1 = kn->neuron_index_map[neuron1];
	NeuronIndex n2 = kn->neuron_index_map[neuron2];

	return n1.distanceTo(n2);
}

size_t nnets_kohonen::getWeightsMatrixSize(void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return kn->getWeightsMatrixSize();
}

void nnets_kohonen::setWeights(const float* w, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	kn->setWeights(w);
}

void nnets_kohonen::setUseNormalization(bool norm, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	kn->setUseNormalization(norm);
}

size_t nnets_kohonen::getAllWeights(float * w, void * obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return kn->getWeights(w);
}

void nnets_kohonen::disableNeurons(std::vector<int> neurons, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	std::vector<NeuronIndex> new_neuron_map;
	std::vector<int> old_new_map;

	for (int j = 0; j < neurons.size(); j++)
		if ((neurons[j] < 0) || (neurons[j] > kn->neuron_index_map.size()))
			throw "Neuron index out of range";

	for (int i = 0; i < kn->neuron_index_map.size(); i++)
	{
		bool is_disable = false;
		for (int j = 0; j < neurons.size(); j++)
		{
			if (i == neurons[j])
			{
				is_disable = true;
				break;
			}
		}

		if (is_disable == false)
		{
			new_neuron_map.push_back(kn->neuron_index_map[i]);
			old_new_map.push_back(i);
		}
	}

	delete[] kn->kohonen_layer;
	kn->kohonen_layer = new float[new_neuron_map.size()];

	float* new_weights = new float[new_neuron_map.size() * kn->x_size];
	float** new_classes = new float*[new_neuron_map.size()];

	for (int i = 0; i < new_neuron_map.size(); i++)
	{
		int old_index = old_new_map[i];
		for (int j = 0; j < kn->x_size; j++)
			new_weights[i * kn->x_size + j] = kn->weights[old_index * kn->x_size + j];
		
		new_classes[i] = new float[kn->y_size];
		for (int j = 0; j < kn->y_size; j++)
			new_classes[i][j] = kn->classes[old_index][j];
		delete[] kn->classes[old_index];
	}
	delete[] kn->classes;
	delete[] kn->weights;

	kn->classes = new_classes;
	kn->weights = new_weights;
	kn->neuron_index_map = new_neuron_map;
}

void * nnets_kohonen::copyKohonen(void * obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return new KohonenNet(*kn);
}

void nnets_kohonen::freeKohonen(void *& obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	delete kn;
	obj = nullptr;
}

const float* nnets_kohonen::getWeights(int neuron, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);

	if (neuron == -1) throw "Invalid neuron index";

	return kn->weights + neuron * kn->x_size;
}

int nnets_kohonen::getWinner(void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return kn->getInternalIndex(kn->winner);
}

size_t nnets_kohonen::solve(const float* x, float* y, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return kn->solve(x, y);
}

int nnets_kohonen::getMaxNeuronIndex(void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);
	return kn->neuron_index_map.size();
}

void nnets_kohonen::addmultWeights(int neuron, float alpha, float beta, const float* x, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);

	if (neuron != -1)
	{
		float* w = kn->weights + neuron * kn->x_size;
		for (int i = 0; i < kn->x_size; i++)
			w[i] = alpha * w[i] + beta * x[i];
	}
	else throw "Invalid neuron index";
}

void nnets_kohonen::setY(int neuron, const float* y, void* obj)
{
	KohonenNet* kn = static_cast<KohonenNet*>(obj);

	if (neuron != -1)
		kn->setClass(kn->neuron_index_map[neuron], y);
	else throw "Invalid neuron index";
}

void KohonenNet::initByNeuronMap(std::vector<NeuronIndex> map)
{
	kohonen_layer = new float[map.size()];

	weights = new float[map.size() * x_size];
	classes = new float*[map.size()];

	for (int i = 0; i < map.size(); i++)
	{
		for (int j = 0; j < x_size; j++)
			weights[i * x_size + j] = 0.0f;

		classes[i] = new float[y_size];
		for (int j = 0; j < y_size; j++)
			classes[i][j] = 0.0f;
	}
	neuron_index_map = map;
}

KohonenNet::KohonenNet(int inputs_count, int outputs_count,
	int koh_width, int koh_height, float classEps,
	ClassInitializer initializer, Metric metric) :
	winner({0,0}), 
	neurons_width(koh_width), neurons_height(koh_height),
	x_size(inputs_count), y_size(outputs_count), 
	initializer(initializer), metric(metric), class_eps(classEps)
{
	use_norm_x = false;
	x_internal = new float[x_size];

	std::vector<NeuronIndex> map;
	for (int y = 0; y < neurons_height; y++)
		for (int x = 0; x < neurons_width; x++)
			map.push_back(int2d{ x, y });
	initByNeuronMap(map);
}

KohonenNet::KohonenNet(KohonenNet& kn) : winner({0,0})
{
	use_norm_x = kn.use_norm_x;
	neurons_width = kn.neurons_width; neurons_height = kn.neurons_height;
	x_size = kn.x_size; y_size = kn.y_size;
	metric = kn.metric;	initializer = kn.initializer; 
	class_eps = kn.class_eps;

	x_internal = new float[x_size];

	initByNeuronMap(kn.neuron_index_map);
	for (int i = 0; i < neuron_index_map.size() * x_size; i++)
		weights[i] = kn.weights[i];

	for (int i = 0; i < neuron_index_map.size(); i++)
		for (int j = 0; j < y_size; j++)
			classes[i][j] = kn.classes[i][j];
}

size_t KohonenNet::getInputsCount() { return x_size; }
size_t KohonenNet::getOutputsCount() { return y_size; }
void KohonenNet::setWeights(const float* weights)
{
	for (int i = 0; i < neuron_index_map.size() * x_size; i++)
		this->weights[i] = weights[i];
}

void KohonenNet::setClasses(float ** classes)
{
	for (int i = 0; i < neuron_index_map.size(); i++)
		for (int j = 0; j < y_size; j++)
			this->classes[i][j] = classes[i][j];
}

void nnets_kohonen::KohonenNet::setClasses(float ** y, int rowsCount)
{
	ClassExtracter extr { class_eps };
	extr.fit(y, rowsCount, getOutputsCount());
	auto distr = extr.getClassesDistributions();
	int all_sizes = getMaxNeuronIndex(this);

	if ((initializer == Statistical) || (initializer == Revert))
	{
		if (initializer == Revert)
		{
			std::sort(distr.begin(), distr.end(),
				[](std::pair<int, int> p1, std::pair<int, int> p2) { return p1.second < p2.second; });
			auto first = distr.begin();
			auto last = distr.end() - 1;
			while (first < last)
			{
				int temp = first->first;
				first->first = last->first;
				last->first = temp;

				first++;
				last--;
			}
		}

		int sum = 0;
		for (int i = 0; i < distr.size(); i++)
		{
			int temp = distr[i].second * all_sizes;
			distr[i].second = temp / rowsCount;
			sum += distr[i].second;
		}
		distr[distr.size() - 1].second += (all_sizes - sum);
	}
	else if (initializer == Evenly)
	{
		int sum = 0;
		for (int i = 0; i < distr.size(); i++)
		{
			distr[i].second = all_sizes / distr.size();
			sum += distr[i].second;
		}
		distr[distr.size() - 1].second += (all_sizes - sum);
	}
	else throw "Unsupported class initializer";

	int currentClassIndex = 0;
	for (int j = 0; j < all_sizes; j++)
	{
		if (distr[currentClassIndex].second <= 0)
			currentClassIndex++;
		setY(j, y[distr[currentClassIndex].first], this);
		distr[currentClassIndex].second--;
	}
}

void KohonenNet::setNeurons(std::vector<NeuronIndex>& neurons)
{
	delete[] kohonen_layer;
	for (int i = 0; i < neurons_width * neurons_height; i++)
		delete[] classes[i];
	delete[] classes;
	delete[] weights;

	initByNeuronMap(neurons);
}

int KohonenNet::getInternalIndex(NeuronIndex n)
{
	int found_index = -1;
	for (int i = 0; i < neuron_index_map.size(); i++)
		if (neuron_index_map[i] == n)
			return i;
	return -1;
}

void KohonenNet::setClass(NeuronIndex n, const float* y)
{
	int found_index = getInternalIndex(n);
	if (found_index != -1)
	{
		float* cur_class = classes[found_index];
		for (int i = 0; i < y_size; i++)
			cur_class[i] = y[i];
	}
	else
		throw "Invalid neuron index";
}

void nnets_kohonen::KohonenNet::setClassEps(float eps)
{
	class_eps = eps;
}

void KohonenNet::setUseNormalization(bool norm)
{
	use_norm_x = norm;
}

size_t nnets_kohonen::KohonenNet::getClasses(float ** classes)
{
	for (int i = 0; i < neuron_index_map.size(); i++)
		for (int j = 0; j < y_size; j++)
			classes[i][j] = this->classes[i][j];
	return neuron_index_map.size() * y_size;
}

float nnets_kohonen::KohonenNet::getClassEps()
{
	return class_eps;
}

size_t KohonenNet::getWeights(float* weights)
{
	for (int i = 0; i < neuron_index_map.size() * x_size; i++)
		weights[i] = this->weights[i];
	return neuron_index_map.size() * x_size;
}

std::vector<NeuronIndex> nnets_kohonen::KohonenNet::getNeurons()
{
	return neuron_index_map;
}

size_t KohonenNet::getClass(NeuronIndex n, float* y)
{
	int found_index = getInternalIndex(n);
	if (found_index != -1)
	{
		float* cur_class = classes[found_index];
		for (int i = 0; i < y_size; i++)
			y[i] = cur_class[i];
		return y_size;
	}

	return 0;
}

size_t KohonenNet::getWeightsMatrixSize()
{
	return neuron_index_map.size() * x_size;
}

bool nnets_kohonen::KohonenNet::getUseNormalization()
{
	return use_norm_x;
}

int nnets_kohonen::KohonenNet::getWinnerIndex()
{
	return getInternalIndex(winner);
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

NeuronIndex KohonenNet::calcWinner(const float* x)
{
	const float* input = x;
	if (use_norm_x)
	{
		float norm = 0.0f;
		for (int i = 0; i < x_size; i++)
			norm += x[i] * x[i];
		norm = std::sqrt(norm);
		for (int i = 0; i < x_size; i++)
			x_internal[i] = x[i] / norm;

		input = x_internal;
	}

	int winner_index = 0;

	if (metric == Metric::Default)
	{
		cblas_sgemv(CblasRowMajor, CblasNoTrans,
			neuron_index_map.size(), x_size,
			1.0f, weights, x_size,
			input, 1, 0.0f, kohonen_layer, 1);
		float max_value = kohonen_layer[0];
		for (int i = 1; i < neuron_index_map.size(); i++)
		{
			if (max_value < kohonen_layer[i])
			{
				max_value = kohonen_layer[i];
				winner_index = i;
			}
		}
	}
	else if (metric == Metric::Euclidean)
	{
		for (int i = 0; i < neuron_index_map.size(); i++)
		{
			kohonen_layer[i] = 0.0f;
			for (int j = 0; j < x_size; j++)
			{
				float temp = input[i] - weights[i * x_size + j];
				kohonen_layer[i] += temp * temp;
			}
		}

		float min_value = kohonen_layer[0];
		for (int i = 1; i < neuron_index_map.size(); i++)
		{
			if (min_value > kohonen_layer[i])
			{
				min_value = kohonen_layer[i];
				winner_index = i;
			}
		}
	}
	else throw "Undefined metric";

	return neuron_index_map[winner_index];
}

size_t KohonenNet::solve(const float* x, float* y)
{
	winner = calcWinner(x);
	int found_index = getInternalIndex(winner);
	float* cur_class = classes[found_index];
	for (int i = 0; i < y_size; i++)
		y[i] = cur_class[i];
	return y_size;
}