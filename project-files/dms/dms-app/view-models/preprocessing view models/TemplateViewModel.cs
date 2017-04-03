using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;

namespace dms.view_models
{
    public class TemplateViewModel
    {
        public TemplateViewModel(int templateId, int var = 0)
        {
            TemplateName = ((dms.models.TaskTemplate)dms.services.DatabaseManager.SharedManager.entityById(templateId, typeof(dms.models.TaskTemplate))).Name; ;

            List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", templateId.ToString()), typeof(dms.models.Parameter));

            List<Parameter> input = new List<Parameter>();
            List<Parameter> output = new List<Parameter>();
            
            foreach (Entity param in parameters)
            {
                dms.models.Parameter p = (dms.models.Parameter)param;
                if (p.IsOutput == 0)
                {
                    input.Add(new Parameter(p.Name, p.Type.ToString(), p.Comment));
                } else
                {
                    output.Add(new Parameter(p.Name, p.Type.ToString(), p.Comment));
                }
            }
            InputParameters = input.ToArray();
            OutputParameters = output.ToArray();
        }
        public string TemplateName { get; }
        public Parameter[] InputParameters { get; }
        public Parameter[] OutputParameters { get; }
    }
}
