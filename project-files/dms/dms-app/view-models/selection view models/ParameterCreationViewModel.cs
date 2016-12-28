using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class ParameterCreationViewModel : ViewmodelBase
    {
        private int index;
        private string name;
        private string type;
        private string comment;
        private bool isOutput;

        public int Index { get { return index; } set { index = value; NotifyPropertyChanged(); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged(); } }
        public string Type { get { return type; } set { type = value; NotifyPropertyChanged(); } }
        public string Comment { get { return comment; } set { comment = value; NotifyPropertyChanged(); } }
        public string KindOfParameter { get { return isOutput ? "Выходной" : "Входной"; } set { isOutput = value.Equals("Выходной"); NotifyPropertyChanged(); } }

        public List<string> AvaliableTypes { get { return new List<string> { "int(5)", "float(5)", "enum(5)" }; } }
        public List<string> ParameterKinds { get { return new List<string> { "Входной", "Выходной" }; } }

        public ParameterCreationViewModel(int index = -1,
            string name = "Parameter 1", string type = "float(5)", bool output = false)
        {
            Index = index;
            Name = name;
            Type = type;
            isOutput = output;
        }
    }
}
