using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NeuroWnd.Activate_functions
{
    public class ActivateFunctionParameter
    {
        private string name;
        private double defaultValue;

        public double Value;

        public ActivateFunctionParameter(string _name, double _defVal)
        {
            name = _name;
            defaultValue = _defVal;
            Value = defaultValue;
        }
        public ActivateFunctionParameter(ActivateFunctionParameter par)
        {
            name = par.name;
            defaultValue = par.defaultValue;
            Value = par.Value;
        }
        public string Name { get { return name; } }
        public double DefaultValue { get { return defaultValue; } }
    }

    public abstract class ActivateFunction
    {
        public abstract bool HasContinuousDerivative { get; }
        public abstract string Name { get; }

        protected List<ActivateFunctionParameter> parameters;

        public int CountParameters { get { return parameters.Count; } }

        public string GetNameOfParameter(int index)
        {
            return parameters[index].Name;
        }
        public double GetDefaultValueOfParameter(string name)
        {
            foreach (ActivateFunctionParameter item in parameters)
            {
                if (String.Compare(item.Name, name) == 0)
                    return item.DefaultValue;
            }
            throw new Exception("Invalid name of parameter");
        }
        public double GetDefaultValueOfParameter(int index)
        {
            return parameters[index].DefaultValue;
        }
        public double GetValueOfParameter(string name)
        {
            int i = 0;
            foreach (ActivateFunctionParameter item in parameters)
            {
                if (String.Compare(item.Name, name) == 0)
                    return parameters[i].Value;
                i++;
            }
            throw new Exception("Invalid name of parameter");
        }
        public double GetValueOfParameter(int index)
        {
            return parameters[index].Value;
        }
        public void SetValueOfParameter(string name, double value)
        {
            int i = 0;
            foreach (ActivateFunctionParameter item in parameters)
            {
                if (String.Compare(item.Name, name) == 0)
                {
                    parameters[i].Value = value;
                    return;
                }
                i++;
            }
            throw new Exception("Invalid name of parameter");
        }
        public void SetValueOfParameter(int index, double value)
        {
            parameters[index].Value = value;
        }

        public ActivateFunction()
        {
            parameters = new List<ActivateFunctionParameter>();
        }
        public ActivateFunction(ActivateFunction af)
        {
            parameters = new List<ActivateFunctionParameter>();
            for (int i = 0; i < af.parameters.Count; i++)
            {
                parameters[i] = new ActivateFunctionParameter(af.parameters[i]);
            }
        }

        public abstract double Function(double x);
        public abstract double Derivative(double x);
    }

    public static class LibraryOfActivateFunctions
    {
        static private List<Type> activateFunctionsTypes;
        public enum GetterParameter { ActivateFunctionName, TypeOfActivateFunctionName };

        static LibraryOfActivateFunctions()
        {
            Type ourtype = typeof(ActivateFunction);
            IEnumerable<Type> en = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype));
            if (en == null)
            {
                throw new Exception("Empty list of activate functions");
            }
            activateFunctionsTypes = new List<Type>(en);
        }

        static public int GetCountActivateFunctions()
        {
            return activateFunctionsTypes.Count;
        }
        static public string[] GetAllActivateFunctionNames()
        {
            string[] res = new string[activateFunctionsTypes.Count];
            int i = 0;
            foreach (Type item in activateFunctionsTypes)
            {
                res[i] = ((ActivateFunction)Activator.CreateInstance(item)).Name;
                i++;
            }
            return res;
        }
        static public string GetActivateFunctionName(string typeName)
        {
            foreach (Type item in activateFunctionsTypes)
            {
                if (String.Compare(item.Name, typeName) == 0)
                {
                    return ((ActivateFunction)Activator.CreateInstance(item)).Name;
                }
            }
            throw new Exception("Invalid type name");
        }
        static public string GetActivateFunctionTypeName(string activateFunctionName)
        {
            foreach (Type item in activateFunctionsTypes)
            {
                if (String.Compare(GetActivateFunctionName(item.Name), activateFunctionName) == 0)
                    return item.Name;
            }
            throw new Exception("Wrong activate function name");
        }
        static public int GetCountParametersOfAF(string name, GetterParameter par)
        {
            switch (par)
            {
                case GetterParameter.ActivateFunctionName:
                    for (int i = 0; i < activateFunctionsTypes.Count; i++)
                    {
                        string nameBf = GetActivateFunctionName(activateFunctionsTypes[i].Name);
                        if (String.Compare(nameBf, name) == 0)
                            return ((ActivateFunction)Activator.CreateInstance(activateFunctionsTypes[i])).CountParameters;
                    }
                    throw new Exception("Incorrect name of activate function");
                case GetterParameter.TypeOfActivateFunctionName:
                    for (int i = 0; i < activateFunctionsTypes.Count; i++)
                    {
                        if (String.Compare(activateFunctionsTypes[i].Name, name) == 0)
                            return ((ActivateFunction)Activator.CreateInstance(activateFunctionsTypes[i])).CountParameters;
                    }
                    throw new Exception("Incorrect name of type of activate function");
                default:
                    throw new Exception("Invalid mode");
            }
        }
        static public double GetDefaultValueOfParameterAF(string name, int indexPar, GetterParameter par)
        {
            switch (par)
            {
                case GetterParameter.ActivateFunctionName:
                    for (int i = 0; i < activateFunctionsTypes.Count; i++)
                    {
                        string nameBf = GetActivateFunctionName(activateFunctionsTypes[i].Name);
                        if (String.Compare(nameBf, name) == 0)
                            return ((ActivateFunction)Activator.
                                CreateInstance(activateFunctionsTypes[i])).
                                GetDefaultValueOfParameter(indexPar);
                    }
                    throw new Exception("Incorrect name of activate function");
                case GetterParameter.TypeOfActivateFunctionName:
                    for (int i = 0; i < activateFunctionsTypes.Count; i++)
                    {
                        if (String.Compare(activateFunctionsTypes[i].Name, name) == 0)
                            return ((ActivateFunction)Activator.
                                CreateInstance(activateFunctionsTypes[i])).
                                GetDefaultValueOfParameter(indexPar);
                    }
                    throw new Exception("Incorrect name of type of activate function");
                default:
                    throw new Exception("Invalid mode");
            }
        }
        static public double GetDefaultValueOfParameterAF(string name, string namePar, GetterParameter par)
        {
            switch (par)
            {
                case GetterParameter.ActivateFunctionName:
                    for (int i = 0; i < activateFunctionsTypes.Count; i++)
                    {
                        string nameBf = GetActivateFunctionName(activateFunctionsTypes[i].Name);
                        if (String.Compare(nameBf, name) == 0)
                            return ((ActivateFunction)Activator.
                                CreateInstance(activateFunctionsTypes[i])).
                                GetDefaultValueOfParameter(namePar);
                    }
                    throw new Exception("Incorrect name of activate function");
                case GetterParameter.TypeOfActivateFunctionName:
                    for (int i = 0; i < activateFunctionsTypes.Count; i++)
                    {
                        if (String.Compare(activateFunctionsTypes[i].Name, name) == 0)
                            return ((ActivateFunction)Activator.
                                CreateInstance(activateFunctionsTypes[i])).
                                GetDefaultValueOfParameter(namePar);
                    }
                    throw new Exception("Incorrect name of type of activate function");
                default:
                    throw new Exception("Invalid mode");
            }
        }
        static public ActivateFunction GetActivateFunction(string name, GetterParameter par)
        {
            switch (par)
            {
                case GetterParameter.ActivateFunctionName:
                    for (int i = 0; i < activateFunctionsTypes.Count; i++)
                    {
                        string nameBf = GetActivateFunctionName(activateFunctionsTypes[i].Name);
                        if (String.Compare(nameBf, name) == 0)
                            return ((ActivateFunction)Activator.
                                CreateInstance(activateFunctionsTypes[i]));
                    }
                    throw new Exception("Incorrect name of activate function");
                case GetterParameter.TypeOfActivateFunctionName:
                    for (int i = 0; i < activateFunctionsTypes.Count; i++)
                    {
                        if (String.Compare(activateFunctionsTypes[i].Name, name) == 0)
                            return ((ActivateFunction)Activator.
                                CreateInstance(activateFunctionsTypes[i]));
                    }
                    throw new Exception("Incorrect name of type of activate function");
                default:
                    throw new Exception("Invalid mode");
            }
        }
    }
}
