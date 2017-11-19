#include "KohonenLearning.h"
#include <iostream>

using namespace nnets_kohonen_learning;

#define DEBUG_OUTPUT

bool ClassExtracter::is_equal(float* y, int size, std::vector<float> _class)
{
	for (int i = 0; i < size; i++)
		if (std::abs(y[i] - _class[i]) > eps)
			return false;
	return true;
}

void ClassExtracter::fit(Selection s)
{
	yToClass.clear();
	classes.clear();
	distrib.clear();
	xByClass.clear();

	yToClass.insert({ 0, 0 });
	classes.push_back(std::vector<float>());
	distrib.push_back(1);

	xByClass.push_back(std::vector<const float*>());
	xByClass[0].push_back(s.x[0]);

	for (int j = 0; j < s.ySize; j++)
		classes[0].push_back(s.y[0][j]);

	for (int i = 1; i < s.rowsCount; i++)
	{
		int class_number = -1;
		for (int k = 0; k < classes.size(); k++)
		{
			if (is_equal(s.y[i], s.ySize, classes[k]) == true)
			{
				class_number = k;
				break;
			}
		}
		if (class_number == -1)
		{
			classes.push_back(std::vector<float>());
			distrib.push_back(1);
			for (int j = 0; j < s.ySize; j++)
				classes[classes.size() - 1].push_back(s.y[i][j]);
			yToClass.insert({ i, classes.size() - 1 });

			xByClass.push_back(std::vector<const float*>());
			xByClass[classes.size() - 1].push_back(s.x[i]);
		}
		else
		{
			yToClass.insert({ i, class_number });
			xByClass[class_number].push_back(s.x[i]);
			distrib[class_number]++;
		}
	}
}

vector2d<float> ClassExtracter::getClasses() { return classes; }
std::map<int, int> ClassExtracter::getYClassMapping() { return yToClass; }
std::vector<int> ClassExtracter::getClassesDistributions() { return distrib; }
vector2d<const float*> ClassExtracter::getXByClass() { return xByClass; }

void KohonenSelfOrganizer::initRandomWeights(void* trainedKn)
{
	float* w = new float[opers.getWeightsMatrixSize(trainedKn)];
	std::srand(randomSeed);
	for (int i = 0; i < opers.getWeightsMatrixSize(trainedKn); i++)
		w[i] = (float)std::rand() / RAND_MAX - 0.5f;
	opers.setWeights(w, trainedKn);
	delete[] w;
}

vector2d<float> KohonenSelfOrganizer::clasterize(void* trainedKn, Selection s, ClassExtracter& c_ext)
{
	std::map<int, int> yToClass = c_ext.getYClassMapping();
	const int maxNeuronIndex = opers.getMaxNeuronIndex(trainedKn);
	//Set up class distribution for kohonen layers
	vector2d<float> classDistribution;
	for (int i = 0; i < maxNeuronIndex; i++)
		classDistribution.push_back(std::vector<float>(c_ext.getClasses().size(), 0.0f));
	
	float lambda = static_cast<float>(2.0f * maxIterations) / maxNeuronIndex;
	float* yp = new float[s.ySize];
	//Clasterization
	for (int iteration = 0; iteration < maxIterations; iteration++)
	{
		float et = std::expf(static_cast<float>(-iteration) / lambda);
		for (int row = 0; row < s.rowsCount; row++)
		{
			opers.solve(s.x[row], yp, trainedKn);
			int winner = opers.getWinner(trainedKn);
			for (int i = 0; i < maxNeuronIndex; i++)
			{
				int curNeuron = i;
				float distBMU = opers.getDistance(winner, curNeuron, trainedKn);
				float distBMU2 = distBMU * distBMU;
					
				float theta = std::expf(-distBMU2 / (2.0f * sigma0 * sigma0 * et * et));
				float l = l0 * et;
				float learning_rate = l * theta;

				if (learning_rate >= minLearningRate)
				{
					opers.addmultWeights(curNeuron, 1.0f - l*theta, l*theta, s.x[row], trainedKn);
					int classIndex = yToClass[row];
					classDistribution[curNeuron][classIndex] += std::expf(-2.0f * distBMU / maxNeuronIndex);
				}
			}
		}

		err = 0.0f;
		float avg2 = 0.0f;
		for (int row = 0; row < s.rowsCount; row++)
		{
			opers.solve(s.x[row], yp, trainedKn);
			int winner = opers.getWinner(trainedKn);
			const float* w = opers.getWeights(winner, trainedKn);
			float norm = 0.0f;
			for (int i = 0; i < s.xSize; i++)
			{
				float temp = s.x[row][i] - w[i];
				norm += temp * temp;
			}
			norm = sqrt(norm);
			err += norm;

			float miss = 0.0f;
			for (int i = 0; i < s.ySize; i++)
			{
				float temp = s.y[row][i] - yp[i];
				miss += temp * temp;
			}
			avg2 += miss;
		}
		err /= s.rowsCount;
		avg2 /= s.rowsCount;

#ifdef DEBUG_OUTPUT
		std::cout << "Iteration " << iteration + 1 
			<< " err:" << err 
			<< " avg2:" << avg2 
			<< std::endl;
#endif
	}
	delete[] yp;
	return classDistribution;
}

float KohonenSelfOrganizer::selfOrganize(Selection trainSel, void* trainedKn)
{
	opers.setUseNormalization(false, trainedKn);
	if (has_norm == true)
	{
		for (int i = 0; i < trainSel.rowsCount; i++)
		{
			float norm = 0.0f;
			for (int j = 0; j < trainSel.xSize; j++)
				norm += trainSel.x[i][j] * trainSel.x[i][j];
			norm = std::sqrt(norm);
			for (int j = 0; j < trainSel.xSize; j++)
				trainSel.x[i][j] = trainSel.x[i][j] / norm;
		}
	}

	initRandomWeights(trainedKn);
	
	ClassExtracter c_extr(opers, eps);
	c_extr.fit(trainSel);
	auto distr = clasterize(trainedKn, trainSel, c_extr);
	normalize(trainSel.ySize, trainedKn, c_extr, distr);
	opers.setUseNormalization(has_norm, trainedKn);

	return err;
}

void KohonenSelfOrganizer::normalize(int ySize, void* trainedKn, ClassExtracter& c_extr, vector2d<float>& clasters)
{
	int maxNeuron = opers.getMaxNeuronIndex(trainedKn);
	vector2d<float> classes = c_extr.getClasses();
	std::vector<int> unused_neurons;

	float *yp = new float[ySize];
	for (int x = 0; x < maxNeuron; x++)
	{
		auto& distrCurNeuron = clasters[x];
		float sum = 0.0f;
		for(int i = 0; i < distrCurNeuron.size(); i++)
			sum += distrCurNeuron[i];

		if (abs(sum) > eps)
		{
			int maxIndex = 0;
			float max = distrCurNeuron[0];
			for (int i = 0; i < distrCurNeuron.size(); i++)
			{
				if (max < distrCurNeuron[i])
				{
					max = distrCurNeuron[i];
					maxIndex = i;
				}
			}
			auto yClass = classes[maxIndex];
			for (int j = 0; j < ySize; j++)
				yp[j] = yClass[j];
			opers.setY(x, yp, trainedKn);
		}
		else
			unused_neurons.push_back(x);
	}
	opers.disableNeurons(unused_neurons, trainedKn);
	delete[] yp;
}

void StatisticalPretrainer::pretrain(Selection s, void* trainedKn, int seed)
{
	std::srand(seed);

	ClassExtracter c_ext(opers, eps);
	c_ext.fit(s);
	auto classes = c_ext.getClasses();
	auto distr = c_ext.getClassesDistributions();
	auto xByClass = c_ext.getXByClass();

	int all_sizes = opers.getMaxNeuronIndex(trainedKn);

	for (int i = 0; i < distr.size(); i++)
	{
		int temp = distr[i] * all_sizes;
		distr[i] = temp / s.rowsCount;
		if ((temp % s.rowsCount) != 0)
			distr[i]++;
	}

	int currentClassIndex = 0;
	float* yp = new float[s.ySize];
	for (int j = 0; j < all_sizes; j++)
	{
		if (distr[currentClassIndex] <= 0)
			currentClassIndex++;
		for (int i = 0; i < s.ySize; i++)
		{
			yp[i] = classes[currentClassIndex][i];
		}
		int xIndex = (float)rand() / RAND_MAX * xByClass[currentClassIndex].size();
		opers.setY(j, yp, trainedKn);
		opers.addmultWeights(j, 0.0f, 1.0f,
			xByClass[currentClassIndex][xIndex], trainedKn);
		distr[currentClassIndex]--;
	}
}

bool KohonenClassifier::is_equal(float* v1, float* v2, int size)
{
	for (int i = 0; i < size; i++)
		if (std::fabs(v1[i] - v2[i]) > eps)
			return false;
	return true;
}

float KohonenClassifier::train(Selection trainSel, void* trainedKn)
{
	opers.setUseNormalization(false, trainedKn);
	if (normalize == true)
	{
		for (int i = 0; i < trainSel.rowsCount; i++)
		{
			float norm = 0.0f;
			for (int j = 0; j < trainSel.xSize; j++)
				norm += trainSel.x[i][j] * trainSel.x[i][j];
			norm = std::sqrt(norm);
			for (int j = 0; j < trainSel.xSize; j++)
				trainSel.x[i][j] = trainSel.x[i][j] / norm;
		}
	}

	if (hasPretrainer == true)
	{
		StatisticalPretrainer trainer(opers, eps);
		trainer.pretrain(trainSel, trainedKn, seed);
	}

	float *yp = new float[trainSel.ySize];
	float err;

	for(int iter = 0; iter < maxIterations; iter++)
	{
		float stepLearn = learnA / (iter + learnB);
		for(int row = 0; row < trainSel.rowsCount; row++)
		{
			opers.solve(trainSel.x[row], yp, trainedKn);
			int winner = opers.getWinner(trainedKn);
			float l = is_equal(yp, trainSel.y[row], trainSel.ySize) == true ? stepLearn : -stepLearn;
			opers.addmultWeights(winner, 1.0f - l, l, trainSel.x[row], trainedKn);
		}

		err = 0.0f;
		for (int row = 0; row < trainSel.rowsCount; row++)
		{
			opers.solve(trainSel.x[row], yp, trainedKn);
			float miss = 0.0f;
			for (int i = 0; i < trainSel.ySize; i++)
			{
				float temp = trainSel.y[row][i] - yp[i];
				miss += temp * temp;
			}
			err += miss;
		}
		err /= trainSel.rowsCount;
		err = std::sqrt(err);
#ifdef DEBUG_OUTPUT
		std::cout << "Iteration " << iter + 1 << " err:" << err << std::endl;
		//std::cout << " percent:" << static_cast<float>(count_mistakes) / trainSel.rowsCount * 100 << std::endl;
#endif 
	}
	opers.setUseNormalization(normalize, trainedKn);

	delete[] yp;
	return err;
}