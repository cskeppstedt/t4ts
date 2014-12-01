using System;
using System.Linq;
using EnvDTE;

namespace T4TS
{
    public class ProjectTraverser
    {
        public ProjectTraverser(Project project, Action<CodeNamespace> withNamespace)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (withNamespace == null)
                throw new ArgumentNullException("withNamespace");

            WithNamespace = withNamespace;

            if (project.ProjectItems != null)
                Traverse(project.ProjectItems);
        }

        public Action<CodeNamespace> WithNamespace { get; private set; }

        private void Traverse(ProjectItems items)
        {
            foreach (ProjectItem pi in items)
            {
                if (pi.FileCodeModel != null)
                {
                    if (CodeTraverser.Settings.ProjectNamesToProcess == null ||
                        CodeTraverser.Settings.ProjectNamesToProcess.Contains(pi.ContainingProject.Name))
                    {
                        CodeElements codeElements = pi.FileCodeModel.CodeElements;
                        foreach (CodeNamespace ns in codeElements.OfType<CodeNamespace>())
                            WithNamespace(ns);
                    }
                }

                if (pi.ProjectItems != null)
                    Traverse(pi.ProjectItems);

                    /* LionSoft: Process projects in solution folders */
                else if (pi.SubProject != null && pi.SubProject.ProjectItems != null)
                {
                    Traverse(pi.SubProject.ProjectItems);
                }
                /* --- */
            }
        }
    }
}