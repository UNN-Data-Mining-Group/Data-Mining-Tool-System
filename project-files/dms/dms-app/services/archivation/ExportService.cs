using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.models.archive;
using dms.models;

namespace dms.services.archivation
{
    class ExportService
    {
        private class RelationExport
        {
            public int ID;
            public ArchiveModel model;

            public RelationExport(int ID, ArchiveModel model)
            {
                this.ID = ID;
                this.model = model;
            }
        }

        private List<RelationExport> selectionRelations = new List<RelationExport>();
        private List<RelationExport> taskSolverRelations = new List<RelationExport>();
        private List<RelationExport> scenarioRelations = new List<RelationExport>();

        private static ExportService sharedManager;

        public static ExportService SharedManager
        {
            get
            {
                if (sharedManager == null)
                {
                    sharedManager = new ExportService();
                }
                return sharedManager;
            }
        }

        public Archive generateArchiveSystem()
        {
            scenarioRelations.Clear();
            selectionRelations.Clear();
            taskSolverRelations.Clear();
            Archive archive = new Archive();
            List<Entity> tasks = models.Task.all(typeof(models.Task));
            foreach (models.Task task in tasks)
            {
                ArchiveTask archTask = new ArchiveTask(task);
                List<TaskTemplate> templates = TaskTemplate.templatesOfTaskId(task.ID);
                foreach (TaskTemplate template in templates)
                {
                    ArchiveTemplate archTemp = new ArchiveTemplate(template);
                    List<Selection> selections = Selection.selectionsOfTaskTemplateId(template.ID);
                    foreach (Selection sel in selections)
                    {
                        ArchiveSelection archSel = new ArchiveSelection(sel);
                        selectionRelations.Add(new RelationExport(sel.ID, archSel));
                        archTemp.Selections.Add(archSel);
                    }
                    List<Parameter> parameters = Parameter.parametersOfTaskTemplateId(template.ID);
                    foreach (Parameter par in parameters)
                    {
                        ArchiveParameter archPar = new ArchiveParameter(par);
                        archTemp.Parameters.Add(archPar);
                    }
                    archTask.Templates.Add(archTemp);
                }
                List<TaskSolver> solvers = TaskSolver.solversOfTaskId(task.ID);
                foreach (TaskSolver solver in solvers)
                {
                    ArchiveTaskSolver archSol = new ArchiveTaskSolver(solver);
                    taskSolverRelations.Add(new RelationExport(solver.ID, archSol));
                    archTask.Solvers.Add(archSol);
                }
                archive.Tasks.Add(archTask);
            }
            List<Entity> scenarios = LearningScenario.all(typeof(LearningScenario));
            foreach (LearningScenario scenario in scenarios)
            {
                ArchiveScenario archScenario = new ArchiveScenario(scenario);
                scenarioRelations.Add(new RelationExport(scenario.ID, archScenario));
                archive.Scenarios.Add(archScenario);
            }
            List<Entity> lSolvers = LearnedSolver.all(typeof(LearnedSolver));
            foreach (LearnedSolver solver in lSolvers)
            {
                ArchiveLearnedSolver archSol = new ArchiveLearnedSolver(solver);
                List<LearningQuality> quailties = LearningQuality.qualitiesOfSolverId(solver.ID);
                foreach (LearningQuality quality in quailties)
                {
                    ArchiveLearningQuality archQ = new ArchiveLearningQuality(quality);
                    archSol.Qualities.Add(archQ);
                }
                var find = taskSolverRelations.Find(x => x.ID == solver.TaskSolverID);
                archSol.TaskSolver = (find != null) ? (ArchiveTaskSolver)find.model : null;
                find = selectionRelations.Find(x => x.ID == solver.SelectionID);
                archSol.Selection = (find != null) ? (ArchiveSelection)find.model : null;
                find = scenarioRelations.Find(x => x.ID == solver.LearningScenarioID);
                archSol.Scenario = (find != null) ? (ArchiveScenario)find.model : null;
                archive.LearnedSolvers.Add(archSol);
            }
            return archive;
        }
    }
}
