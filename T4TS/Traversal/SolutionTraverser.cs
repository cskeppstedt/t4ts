using System;
using EnvDTE;

namespace T4TS
{
    public class SolutionTraverser
    {
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

        public Action<CodeNamespace> WithNamespace { get; private set; }

        private void Traverse(Projects projects)
        {
            foreach (Project project in projects)
            {
                new ProjectTraverser(project, WithNamespace);
            }
        }
    }
}