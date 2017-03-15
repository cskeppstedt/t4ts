using System;
using EnvDTE;

namespace T4TS
{
    public class SolutionTraverser
    {
        public Action<CodeNamespace> WithNamespace { get; private set; }

        public SolutionTraverser(Solution solution, Action<CodeNamespace> withNamespace)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

            if (withNamespace == null)
                throw new ArgumentNullException("withNamespace");

            WithNamespace = withNamespace;

            if (solution.Projects != null)
                Traverse(solution.Projects);
        }

        private void Traverse(Projects projects)
        {
            var item = projects.GetEnumerator();
			
            while (item.MoveNext())
            {
                var project = item.Current as Project;
                if (project == null)
                {
                    continue;
                }

                if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
                {
                    GetSolutionFolderProjects(project);
                }
                else
                {
                    new ProjectTraverser(project, WithNamespace);
                }
            }
        }

        private void GetSolutionFolderProjects(Project solutionFolder)
        {
            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
                {
                   GetSolutionFolderProjects(subProject);
                }
                else
                {
                    new ProjectTraverser(subProject, WithNamespace);
                }
            }
        }
    }
}