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
		foreach (Project project in projects)
		{
			TraverseProject(project);
		}
	}

	private void TraverseProject(Project project)
	{
		if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
		{
			foreach (ProjectItem projectItem in project.ProjectItems)
			{
				var childProject = projectItem.SubProject;
				if (childProject == null)
					continue;

				TraverseProject(childProject);
			}
		}
		else
			new ProjectTraverser(project, WithNamespace);
	}
    }
}
