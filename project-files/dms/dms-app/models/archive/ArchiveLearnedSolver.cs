using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models.archive
{
    [Serializable]
    public class ArchiveLearnedSolver : ArchiveModel
    {
        private ArchiveTaskSolver taskSolver;
        public ArchiveTaskSolver TaskSolver
        {
            get
            {
                return taskSolver;
            }

            set
            {
                taskSolver = value;
            }
        }

        private ArchiveScenario scenario;
        public ArchiveScenario Scenario
        {
            get
            {
                return scenario;
            }

            set
            {
                scenario = value;
            }
        }

        private ArchiveSelection selection;
        public ArchiveSelection Selection
        {
            get
            {
                return selection;
            }

            set
            {
                selection = value;
            }
        }

        private List<ArchiveLearningQuality> qualities = new List<ArchiveLearningQuality>();
        public List<ArchiveLearningQuality> Qualities
        {
            get
            {
                return qualities;
            }

            set
            {
                qualities = value;
            }
        }

        public ArchiveLearnedSolver(models.LearnedSolver solver)
        {
            this.ID = solver.ID;
        }

        override public bool equalsEntity(models.Entity entity)
        {
            if (entity.GetType() != typeof(models.LearnedSolver))
            {
                return false;
            }
            LearnedSolver solver = (LearnedSolver)entity;
            return this.TaskSolver.ID == solver.TaskSolverID && this.Selection.ID == solver.SelectionID && this.scenario.ID == solver.LearningScenarioID;
        }
    }
}
