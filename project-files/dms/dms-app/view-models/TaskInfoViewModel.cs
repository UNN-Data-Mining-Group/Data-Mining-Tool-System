using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class Parameter
    {
        public Parameter(string name, string type, string comment)
        {
            Name = name;
            Type = type;
            Comment = comment;
        }
        public string Name { get; }
        public string Type { get; }
        public string Comment { get; }
    }

    public class Preprocessing
    {
        public Preprocessing(string name, Tuple<Parameter, string>[] preproc)
        {
            Name = name;
            ParameterProcessing = preproc;
        }
        public string Name { get; }
        public Tuple<Parameter, string>[] ParameterProcessing { get; }
    }

    public class TaskInfoViewModel
    {
        public TaskInfoViewModel(string taskName)
        {
            var p1 = new Parameter("Параметр 1", "float", "");
            var p2 = new Parameter("Параметр 2", "int", "");
            var p3 = new Parameter("Параметр 3", "enum", "выходной параметр");

            TaskName = taskName;
            InputParameters = new Parameter[] { p1, p2 };
            OutputParameters = new Parameter[] { p3 };
            PreprocessingList = new Preprocessing[]
            {
                new Preprocessing("Преобразование 1", new Tuple<Parameter, string>[] 
                {
                    new Tuple<Parameter, string>(p1, "normalize"),
                    new Tuple<Parameter, string>(p2, "none"),
                    new Tuple<Parameter, string>(p3, "binarise")
                })
            };
        }

        public string TaskName { get; }
        public Parameter[] InputParameters { get; }
        public Parameter[] OutputParameters { get; }
        public Preprocessing[] PreprocessingList { get; }
    }
}
