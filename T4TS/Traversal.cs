using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public static class Traversal
    {
        public static void TraverseNamespacesInSolution(
            Solution solution,
            Action<CodeNamespace> withNamespace)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

            if (withNamespace == null)
                throw new ArgumentNullException("withNamespace");

            if (solution.Projects != null)
            {
                foreach (Project project in solution.Projects)
                {
                    Traversal.TraverseNamespacesInProject(project, withNamespace);
                }
            }
        }

        public static void TraverseNamespacesInProject(Project project, Action<CodeNamespace> withNamespace)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (withNamespace == null)
                throw new ArgumentNullException("withNamespace");
            
            if (project.ProjectItems != null)
            {
                Traversal.TraverseNamespacesInProjectItems(
                    project.ProjectItems,
                    withNamespace);
            }
        }

        public static void TraverseClassesInNamespace(CodeNamespace ns, Action<CodeClass> withCodeClass)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");

            if (withCodeClass == null)
                throw new ArgumentNullException("withCodeClass");
            
            if (ns.Members != null)
            {
                foreach (object elem in ns.Members)
                {
                    if (elem is CodeClass)
                        withCodeClass((CodeClass)elem);
                }
            }
        }

        public static void TraversePropertiesInClass(CodeClass codeClass, Action<CodeProperty> withProperty)
        {
            if (codeClass == null)
                throw new ArgumentNullException("codeClass");

            if (withProperty == null)
                throw new ArgumentNullException("withProperty");
            
            if (codeClass.Members != null)
            {
                foreach (var property in codeClass.Members)
                {
                    if (property is CodeProperty)
                        withProperty((CodeProperty)property);
                }
            }
        }

        private static void TraverseNamespacesInProjectItems
            (ProjectItems items,
            Action<CodeNamespace> withNamespace)
        {
            foreach (ProjectItem pi in items)
            {
                if (pi.FileCodeModel != null)
                {
                    var codeElements = pi.FileCodeModel.CodeElements;

                    foreach (object elem in codeElements)
                    {
                        if (elem is CodeNamespace)
                            withNamespace((CodeNamespace)elem);
                    }
                }

                if (pi.ProjectItems != null)
                {
                    Traversal.TraverseNamespacesInProjectItems(
                        pi.ProjectItems,
                        withNamespace);
                }
                else if (pi.SubProject != null && pi.SubProject.ProjectItems != null)
                {
                    Traversal.TraverseNamespacesInProjectItems(
                        pi.SubProject.ProjectItems,
                        withNamespace);
                }
            }
        }
    }
}
