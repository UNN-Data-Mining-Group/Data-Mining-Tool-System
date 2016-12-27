using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class TemplateViewModel
    {
        public TemplateViewModel(string templateName, int var = 0)
        {
            TemplateName = templateName;

            if (var == 0)
            {
                var p1 = new Parameter("Параметр 1", "float", "");
                var p2 = new Parameter("Параметр 2", "int", "");
                var p3 = new Parameter("Параметр 3", "enum", "выходной параметр");

                InputParameters = new Parameter[] { p1, p2 };
                OutputParameters = new Parameter[] { p3 };
            }
            else
            {
                var p1 = new Parameter("Параметр 1", "float", "");
                var p2 = new Parameter("Параметр 2", "enum", "выходной параметр");

                InputParameters = new Parameter[] { p1 };
                OutputParameters = new Parameter[] { p2 };
            }
        }
        public string TemplateName { get; }
        public Parameter[] InputParameters { get; }
        public Parameter[] OutputParameters { get; }
    }
}
