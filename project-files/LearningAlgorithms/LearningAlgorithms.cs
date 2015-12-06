using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LearningAlgorithms
{
    public interface ILearning
    { }

    public interface INeuroNetLearning : ILearning
    {
        bool[,] get_bool_links();
        double[,] get_links();
        void set_links(double[,] links);
        double get_res(double[] X);
        INeuroNetLearning copy();
        void write_result(string algorithm);
    }

    public abstract class LearningAlgorithm
    {
        public LearningAlgorithm()
        {
        }

        public abstract string Name { get; }
    }

    public static class LearningAlgorithmsLibrary
    {
        private static List<Type> typesOfLA;

        public enum GetterParameter { AlgorithmName, TypeOfAlgorithmName };

        static LearningAlgorithmsLibrary()
        {
            Type ourtype = typeof(LearningAlgorithm);
            IEnumerable<Type> en = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype));
            if (en == null)
            {
                throw new Exception("Empty list of topologies");
            }
            typesOfLA = new List<Type>(en);
        }

        public static int CountAlgorithms { get { return  typesOfLA.Count; } }
        public static string GetNameOfTypeOfAlgoritm(string nameLA)
        {
            foreach (Type item in typesOfLA)
            {
                LearningAlgorithm la = (LearningAlgorithm)Activator.CreateInstance(item);
                if (String.Compare(la.Name, nameLA) == 0)
                {
                    return item.Name;
                }
            }
            throw new Exception("Invalid topology name");
        }
        public static string GetNameOfAlgorithm(string nameType)
        {
            foreach (Type item in typesOfLA)
            {
                if (String.Compare(item.Name, nameType) == 0)
                {
                    return ((LearningAlgorithm)Activator.CreateInstance(item)).Name;
                }
            }
            throw new Exception("Invalid topology type name");
        }
        public static string[] GetAllNamesOfAlgorithms()
        {
            string[] res = new string[typesOfLA.Count];
            int i = 0;
            foreach (Type item in typesOfLA)
            {
                res[i] = ((LearningAlgorithm)Activator.CreateInstance(item)).Name;
                i++;
            }
            return res;
        }
        public static string[] GetAllNamesOfTypesOfAlgorithms()
        {
            string[] res = new string[typesOfLA.Count];
            int i = 0;
            foreach (Type item in typesOfLA)
            {
                res[i] = item.Name;
                i++;
            }
            return res;
        }     
        public static LearningAlgorithm GetAlgorithm(string name, GetterParameter par)
        {
            switch (par)
            {
                case GetterParameter.AlgorithmName:
                    foreach (Type item in typesOfLA)
                    {
                        LearningAlgorithm la = (LearningAlgorithm)Activator.CreateInstance(item);
                        if (String.Compare(la.Name, name) == 0)
                        {
                            return la;
                        }
                    }
                    throw new Exception("Invalid topology name");

                case GetterParameter.TypeOfAlgorithmName:
                    foreach (Type item in typesOfLA)
                    {
                        if (String.Compare(item.Name, name) == 0)
                        {
                            return (LearningAlgorithm)Activator.CreateInstance(item);
                        }
                    }
                    throw new Exception("Invalid topology type name");

                default:
                    throw new Exception("Invalid mode");
            }
        }
    }
}
