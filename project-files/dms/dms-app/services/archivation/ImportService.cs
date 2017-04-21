using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dms.models.archive;
using dms.models;

namespace dms.services.archivation
{
    class ImportService
    {
        private static ImportService sharedManager;

        public static ImportService SharedManager
        {
            get
            {
                if (sharedManager == null)
                {
                    sharedManager = new ImportService();
                }
                return sharedManager;
            }
        }

        public void importArchive(Archive archive)
        {
            importTasksFromArchive(archive);
            importScenarios(archive);
            importLearnSolvers(archive);
        }

        private void importLearnSolvers(Archive archive)
        {
            List<Entity> solvers = models.LearnedSolver.all(typeof(models.LearnedSolver));
            List<ArchiveLearnedSolver> archSolvers = archive.LearnedSolvers;
            foreach (ArchiveLearnedSolver archSolver in archSolvers)
            {
                var solver = solvers.FirstOrDefault(x => archSolver.equalsEntity(x));
                if (solver == null)
                {
                    importLearnSolver(archSolver);
                }
                else
                {
                    importQualities(archSolver.Qualities, solver.ID);
                }
            }
        }

        private void importScenarios(Archive archive)
        {
            List<Entity> scenarios = models.LearningScenario.all(typeof(models.LearningScenario));
            List<ArchiveScenario> archScens = archive.Scenarios;
            foreach (ArchiveScenario archScenario in archScens)
            {
                var scen = scenarios.FirstOrDefault(x => archScenario.equalsEntity(x));
                if (scen == null)
                {
                    importScenario(archScenario);
                }
                else
                {
                    archScenario.ID = scen.ID;
                }
            }
        }

        private void importTasksFromArchive(Archive archive)
        {
            List<Entity> tasks = models.Task.all(typeof(models.Task));
            List<ArchiveTask> archTasks = archive.Tasks;
            foreach (ArchiveTask archTask in archTasks)
            {
                var task = tasks.FirstOrDefault(x => archTask.equalsEntity(x));
                if (task != null)
                {
                    List<TaskTemplate> templates = TaskTemplate.templatesOfTaskId(task.ID);
                    List<ArchiveTemplate> archTemps = archTask.Templates;
                    foreach (ArchiveTemplate archTemp in archTemps)
                    {
                        var template = templates.FirstOrDefault(x => archTemp.equalsEntity(x));
                        if (template != null)
                        {
                            List<Selection> selections = Selection.selectionsOfTaskTemplateId(template.ID);
                            List<ArchiveSelection> archSels = archTemp.Selections;
                            foreach (ArchiveSelection archSel in archSels)
                            {
                                var selection = selections.FirstOrDefault(x => archSel.equalsEntity(x));
                                if (selection == null)
                                {
                                    importSelection(archSel, template.ID);
                                }
                                else
                                {
                                    archSel.ID = selection.ID;
                                }
                            }
                        }
                        else
                        {
                            importTemplate(archTemp, task.ID);
                        }
                    }

                    List<TaskSolver> solvers = TaskSolver.solversOfTaskId(task.ID);
                    List<ArchiveTaskSolver> archSolvs = archTask.Solvers;
                    foreach (ArchiveTaskSolver archSolver in archSolvs)
                    {
                        var solver = solvers.FirstOrDefault(x => archSolver.equalsEntity(x));
                        if (solver == null)
                        {
                            importTaskSolver(archSolver, task.ID);
                        }
                        else
                        {
                            archSolver.ID = solver.ID;
                        }
                    }
                }
                else
                {
                    importTask(archTask);
                }
            }
        }

        private void importTask(ArchiveTask archTask)
        {
            models.Task task = new models.Task(archTask);
            task.save();
            List<Entity> temps = new List<Entity>();
            foreach (ArchiveTemplate archTemp in archTask.Templates)
            {
                TaskTemplate temp = new TaskTemplate(archTemp);
                temp.TaskID = task.ID;
                temps.Add(temp);
            }
            DatabaseManager.SharedManager.insertMultipleEntities(temps);
            foreach (ArchiveTemplate archTemp in archTask.Templates)
            {
                archTemp.ID = temps[archTask.Templates.IndexOf(archTemp)].ID;
                importSelectionsAndParameters(archTemp);
            }
            List<Entity> solvers = new List<Entity>();
            foreach (ArchiveTaskSolver archSolv in archTask.Solvers)
            {
                TaskSolver solv = new TaskSolver(archSolv);
                solv.TaskID = task.ID;
                solvers.Add(solv);
            }
            DatabaseManager.SharedManager.insertMultipleEntities(solvers);
            foreach (ArchiveTaskSolver archSolv in archTask.Solvers)
            {
                archSolv.ID = solvers[archTask.Solvers.IndexOf(archSolv)].ID;
            }
        }

        private void importTemplate(ArchiveTemplate archTemp, int taskId)
        {
            TaskTemplate template = new TaskTemplate(archTemp);
            template.TaskID = taskId;
            template.save();
            archTemp.ID = template.ID;
            importSelectionsAndParameters(archTemp);
        }

        private void importSelectionsAndParameters(ArchiveTemplate archTemp)
        {
            List<Entity> sels = new List<Entity>();
            foreach (ArchiveSelection archSel in archTemp.Selections)
            {
                Selection sel = new Selection(archSel);
                sel.TaskTemplateID = archTemp.ID;
                sels.Add(sel);
            }
            DatabaseManager.SharedManager.insertMultipleEntities(sels);
            foreach (ArchiveSelection archSel in archTemp.Selections)
            {
                archSel.ID = sels[archTemp.Selections.IndexOf(archSel)].ID;
            }
            List<Entity> pars = new List<Entity>();
            foreach (ArchiveParameter archPar in archTemp.Parameters)
            {
                Parameter par = new Parameter(archPar);
                par.TaskTemplateID = archTemp.ID;
                pars.Add(par);
            }
            DatabaseManager.SharedManager.insertMultipleEntities(pars);
            foreach (ArchiveParameter archPar in archTemp.Parameters)
            {
                archPar.ID = pars[archTemp.Parameters.IndexOf(archPar)].ID;
            }
        }

        private void importSelection(ArchiveSelection archSelection, int templateId)
        {
            Selection selection = new Selection(archSelection);
            selection.TaskTemplateID = templateId;
            selection.save();
            archSelection.ID = selection.ID;
        }

        private void importScenario(ArchiveScenario archScenario)
        {
            LearningScenario scen = new LearningScenario(archScenario);
            scen.save();
            archScenario.ID = scen.ID;
        }

        private void importLearnSolver(ArchiveLearnedSolver archSolver)
        {
            LearnedSolver solver = new LearnedSolver(archSolver);
            solver.save();
            archSolver.ID = solver.ID;
            importQualities(archSolver.Qualities, archSolver.ID);
        }

        private void importTaskSolver(ArchiveTaskSolver archSolver, int taskId)
        {
            TaskSolver solver = new TaskSolver(archSolver);
            solver.TaskID = taskId;
            solver.save();
            archSolver.ID = solver.ID;
        }

        private void importQualities(List<ArchiveLearningQuality> archQualities, int learnedSolverId)
        {
            List<Entity> qualities = new List<Entity>();
            foreach (ArchiveLearningQuality qual in archQualities)
            {
                LearningQuality q = new LearningQuality(qual);
                q.LearnedSolverID = learnedSolverId;
                qualities.Add(q);
            }
            DatabaseManager.SharedManager.insertMultipleEntities(qualities);
        }
    }
}
