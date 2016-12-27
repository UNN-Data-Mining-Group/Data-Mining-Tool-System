using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.view_models
{
    public class CreateSolutionViewModel : ViewmodelBase
    {
        public string TaskName { get; }
        public string Name { get; set; }
        public string TemplateName { get; set; }
        public string[] Templates { get; }
        public CreateSolutionViewModel(string taskName)
        {
            TaskName = taskName;
            Templates = new string[] { "Шаблон 1", "Шаблон 2" };
            TemplateName = Templates[0];
        }
    }
}
