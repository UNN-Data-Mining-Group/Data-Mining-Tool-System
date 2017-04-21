using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.solvers;

namespace dms.models.archive
{
    [Serializable]
    public class ArchiveTaskSolver : ArchiveModel
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

        private ISolverDescription description;
        public ISolverDescription Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        private string typeName;
        public string TypeName
        {
            get
            {
                return typeName;
            }

            set
            {
                typeName = value;
            }
        }

        public ArchiveTaskSolver(models.TaskSolver solv)
        {
            this.ID = solv.ID;
            this.Name = solv.Name;
            this.TypeName = solv.TypeName;
            this.Description = solv.Description;
        }

        override public bool equalsEntity(models.Entity entity)
        {
            if (entity.GetType() != typeof(models.TaskSolver))
            {
                return false;
            }
            return this.Name == ((models.TaskSolver)entity).Name;
        }
    }
}
