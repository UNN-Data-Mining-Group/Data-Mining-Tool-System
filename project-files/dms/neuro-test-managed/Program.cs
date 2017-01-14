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
            Stopwatch sw = new Stopwatch();

            int N = 100000;

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

            sw.Start();
            PerceptronTopology topology = new PerceptronTopology(layers, neurons, delays, afs);
            sw.Stop();
            Console.WriteLine("topology creation = " + sw.ElapsedMilliseconds);

            sw.Start();
            PerceptronManaged perc = new PerceptronManaged(topology);
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
