using System;
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
            int N = 20000;

            int layers = 7;
            int[] neurons = new int[] { 5, 55, 70, 100, 60, 15, 2 };
            bool[] delays = new bool[] { true, true, true, true, true, false };
            IActivateFunction[] afs = new IActivateFunction[] 
            {
                new BinaryStepAF(),
                new BinaryStepAF(),
                new BinaryStepAF(),
                new BinaryStepAF(),
                new BinaryStepAF(),
                new BinaryStepAF()
            };
            PerceptronTopology topology = new PerceptronTopology(layers, neurons, delays, afs);

            PerceptronManaged perc = new PerceptronManaged(topology);
            float[] x = { 1.0f, 0.0f, 0.0f, 1.0f, 0.0f };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < N; i++)
            {
                float[] y = perc.Solve(x);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
