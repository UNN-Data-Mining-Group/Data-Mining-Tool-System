using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models.archive
{
    [Serializable]
    public class Archive
    {
        private List<ArchiveTask> tasks = new List<ArchiveTask>();
        public List<ArchiveTask> Tasks
        {
            get
            {
                return tasks;
            }

            set
            {
                tasks = value;
            }
        }

        private List<ArchiveScenario> scenarios = new List<ArchiveScenario>();
        public List<ArchiveScenario> Scenarios
        {
            get
            {
                return scenarios;
            }

            set
            {
                scenarios = value;
            }
        }

        private List<ArchiveLearnedSolver> learnedSolvers = new List<ArchiveLearnedSolver>();
        public List<ArchiveLearnedSolver> LearnedSolvers
        {
            get
            {
                return learnedSolvers;
            }

            set
            {
                learnedSolvers = value;
            }
        }
    }
}
