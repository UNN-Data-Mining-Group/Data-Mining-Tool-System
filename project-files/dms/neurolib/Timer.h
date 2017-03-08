#pragma once
#include <ctime>

namespace nnets_debug
{
	class Timer
	{
	public:
		void start() 
		{ 
			cur_time = std::clock(); 
			is_run = true;
		}
		void stop() 
		{ 
			if (is_run == true)
			{
				ticks += std::clock() - cur_time;
				count_iter++;
			}
			is_run = false;
		}
		size_t getTickCount() { return ticks; }
		void reset() 
		{ 
			ticks = 0; 
			is_run = false; 
			cur_time = 0; 
			count_iter = 0; 
		}
		int getCountIterations() { return count_iter; }
	private:
		bool is_run = false;
		size_t cur_time = 0;
		size_t ticks = 0;
		int count_iter = 0;
	};
}