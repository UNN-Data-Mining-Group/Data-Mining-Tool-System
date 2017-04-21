using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models.archive
{
    [Serializable]
    public class ArchiveTask : ArchiveModel
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

        private List<ArchiveTemplate> templates = new List<ArchiveTemplate>();
        public List<ArchiveTemplate> Templates
        {
            get
            {
                return templates;
            }

            set
            {
                templates = value;
            }
        }

        private List<ArchiveTaskSolver> solvers = new List<ArchiveTaskSolver>();
        public List<ArchiveTaskSolver> Solvers
        {
            get
            {
                return solvers;
            }

            set
            {
                solvers = value;
            }
        }

        public ArchiveTask(models.Task task)
        {
            this.ID = task.ID;
            this.Name = task.Name;
        }

        override public bool equalsEntity(models.Entity entity)
        {
            if (entity.GetType() != typeof(models.Task))
            {
                return false;
            }
            return this.Name == ((models.Task)entity).Name;
        }
    }
}
