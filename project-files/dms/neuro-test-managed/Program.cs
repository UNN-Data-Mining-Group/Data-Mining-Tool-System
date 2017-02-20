﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using dms.solvers.neural_nets;

namespace neuro_test_managed
{
    class Program
    {
        static void Main(string[] args)
        {
            AccuracyTestPerc();
            AccuracyTestWard();
            AccuracyTestConvNN();
            PerformanceTest();
        }

        static float[][] GenerateWeights(int[] neurons, bool[] hasDelay)
        {
            Random r = new Random();

            float[][] res = new float[neurons.Length - 1][];
            for(int i = 0; i < neurons.Length - 1; i++)
            {
                int dim = (neurons[i] + Convert.ToInt32(hasDelay[i])) * neurons[i + 1];

                res[i] = new float[dim];
                for(int j = 0; j < dim; j++)
                {
                    res[i][j] = (float)r.Nextfloat() * 2.0f - 1.0f;
                }
            }

            return res;
        }

        static void AccuracyTestConvNN()
        {
            Console.WriteLine("Accuracy test convNN:");
            var layers = new List<IConvNNLayer>
            {
                new ConvNNConvolutionLayer
                {
                    FilterWidth = 2, FilterHeight = 2,
                    StrideWidth = 1, StrideHeight = 1,
                    Padding = 1, CountFilters = 2,
                    ActivationFunction = "Identity"
                },
                new ConvNNPoolingLayer
                {
                    FilterWidth = 2, FilterHeight = 2,
                    StrideWidth = 1, StrideHeight = 1
                },
                new ConvNNFullyConnectedLayer
                {
                    NeuronsCount = 2,
                    ActivationFunction = "Identity"
                }
            };

            float[][] w = new float[][] 
            {
                new float[] 
                {
                    -1, 0, 0, 1,
                     0, 1, -1, 0
                },
                new float[]
                {
                    1, 0, 0, -1,
                    -0.5f, 0, 1, 0,

                    0, -1, 1, 0,
                    0, 0, -0.5f, 0
                }
            };
            
            var t = new ConvNNTopology(2, 2, 1, layers);
            var net = new ConvNNManaged(t, w);

            float[] y = net.Solve(new float[]{ 0.5f, 0.3f, -0.6f, -0.2f });
            float[] answer = new float[] { 0.35f, -0.15f };

            float EPS = 1e-5f;
            if ((Math.Abs(y[0]-answer[0]) > EPS) || (Math.Abs(y[1] - answer[1]) > EPS))
            {
                Console.WriteLine("FAIL");
            }
            else
            {
                Console.WriteLine("PASS");
            }
        }

        static void AccuracyTestWard()
        {
            Console.WriteLine("Accuracy test ward:");
            int[][] neurons = new int[][]
            {
                new int[] { 2 },
                new int[] { 2, 1 },
                new int[] { 1, 1 },
                new int[] { 1 }
            };
            bool[][] delays = new bool[][]
            {
                new bool[] { false, false },
                new bool[] { false, false },
                new bool[] { false }
            };
            string[][] afs = new string[][]
            {
                new string[] { "Binary Step", "Identity" },
                new string[] { "Identity", "Binary Step" },
                new string[] { "Identity" }
            };
            int[] groups = { 2, 2, 1 };
            int[] conns = { 1, 0 };
            float[][] w =
            {
                new float[]
                {
                    1.0f, 0.5f,
                    2.0f, 0.25f,
                    0.5f, 1.0f
                },
                new float[]
                {
                    1.0f, 2.0f, 2.0f, 1.0f, 2.0f,
                    2.0f, 1.0f, 1.0f, 1.0f, 2.0f
                },
                new float[]
                {
                    1.0f, 1.0f
                }
            };
            WardNNManaged wnn = new WardNNManaged(new WardNNTopology(neurons, delays, afs, groups, conns, 4), w);
            var y = wnn.Solve(new float[] { 1.0f, 0.0f });
            if (Math.Abs(y[0] - 6.0f) < 1e-6)
            {
                Console.WriteLine("PASS");
            }
            else
            {
                Console.WriteLine("FAIL");
            }
        }

        static void AccuracyTestPerc()
        {
            Console.WriteLine("Accuracy test perc:");

            int[] neurons = new int[] { 3, 4, 2 };
            bool[] delays = new bool[] { false, false };
            string[] afs = new string[] { "Binary Step", "Identity" };
            float[][] w = new float[][] 
            { 
                new float[]
                {
                    1.0f/2,     1.0f/3,     -1.0f/5,
                    1,          1.0f/2,     -1.0f/4,
                    1.0f/2,     -1,         1.0f/2,
                    -1,         1.0f/4,     1.0f/3 
                },
                new float[]
                {
                    1.0f/2,     -1.0f/3,    1.0f/4,      -1,
                    1,          -1.0f/2,    -1.0f/3,    1.0f/4
                }
            };
            
            PerceptronManaged p = new PerceptronManaged(new PerceptronTopology(3, neurons, delays, afs), w);
            float[] y = p.Solve(new float[] { 1, 0, 1 });
            float[] answer = { 5.0f / 12, 1.0f / 6 };
            float EPS = 1e-5f;

            if ((Math.Abs(y[0] - answer[0]) > EPS) || (Math.Abs(y[1] - answer[1]) > EPS))
                Console.WriteLine("FAIL");
            else
                Console.WriteLine("PASS");
        }

        static void PerformanceTest()
        {
            Console.WriteLine("Performance test");
            Stopwatch sw = new Stopwatch();

            int N = 1000000;

            int layers = 7;
            int[] neurons = new int[] { 5, 55, 70, 100, 60, 15, 2 };
            bool[] delays = new bool[] { true, true, true, true, true, false };
            string af = "Logistic";
            string[] afs = new string[]
            {
               af,af,af,af,af,af
            };

            sw.Start();
            PerceptronTopology topology = new PerceptronTopology(layers, neurons, delays, afs);
            sw.Stop();
            Console.WriteLine("topology creation = " + sw.ElapsedMilliseconds);

            sw.Start();
            PerceptronManaged perc = new PerceptronManaged(topology, GenerateWeights(neurons, delays));
            sw.Stop();
            Console.WriteLine("perc creation = " + sw.ElapsedMilliseconds);

            float[] x = { 1.0f, 0.0f, 0.0f, 1.0f, 0.0f };

            sw.Start();
            for (int i = 0; i < N; i++)
            {
                float[] y = perc.Solve(x);
            }
            sw.Stop();
            Console.WriteLine("calc = " + sw.ElapsedMilliseconds);
        }
    }
}
