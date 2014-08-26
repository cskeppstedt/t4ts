using EnvDTE;
using System;
using System.Linq;

namespace T4TS
{
    public class ProjectTraverser
    {
        public Action<CodeNamespace> WithNamespace { get; private set; }

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

        private void Traverse(ProjectItems items)
        {
            foreach (ProjectItem pi in items)
            {
                if (pi.FileCodeModel != null)
                {
                    if (CodeTraverser.Settings.ProjectNamesToProcess == null ||
                        CodeTraverser.Settings.ProjectNamesToProcess.Contains(pi.ContainingProject.Name))
                    {
                        var codeElements = pi.FileCodeModel.CodeElements;
                        foreach (var ns in codeElements.OfType<CodeNamespace>())
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
