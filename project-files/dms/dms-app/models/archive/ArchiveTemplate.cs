using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models.archive
{
    [Serializable]
    public class ArchiveTemplate : ArchiveModel
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        private IPreprocessingParameters preprocessingParameters;
        public IPreprocessingParameters PreprocessingParameters
        {
            get
            {
                return preprocessingParameters;
            }

            set
            {
                preprocessingParameters = value;
            }
        }

        private List<ArchiveSelection> selections = new List<ArchiveSelection>();
        public List<ArchiveSelection> Selections
        {
            get
            {
                return selections;
            }

            set
            {
                selections = value;
            }
        }

        private List<ArchiveParameter> parameters = new List<ArchiveParameter>();
        public List<ArchiveParameter> Parameters
        {
            get
            {
                return parameters;
            }

            set
            {
                parameters = value;
            }
        }

        public ArchiveTemplate(models.TaskTemplate template)
        {
            this.ID = template.ID;
            this.Name = template.Name;
            this.PreprocessingParameters = template.PreprocessingParameters;
        }

        override public bool equalsEntity(models.Entity entity)
        {
            if (entity.GetType() != typeof(models.TaskTemplate))
            {
                return false;
            }
            return this.Name == ((models.TaskTemplate)entity).Name;
        }
    }
}
