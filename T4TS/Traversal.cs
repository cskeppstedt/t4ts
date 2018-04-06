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
                    {
                        TraverseClass(
                            (CodeClass)elem,
                            withCodeClass);
                    }
                }
            }
        }

        public static void TraverseInterfacesInNamespace(
            CodeNamespace ns,
            Action<CodeInterface> withInterface)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");

            if (withInterface == null)
                throw new ArgumentNullException("withInterface");

            if (ns.Members != null)
            {
                foreach (object elem in ns.Members)
                {
                    if (elem is CodeInterface)
                    {
                        withInterface((CodeInterface)elem);
                    }
                }
            }
        }

        public static void TraverseEnumsInNamespace(CodeNamespace ns, Action<CodeEnum> withCodeEnum)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");

            if (withCodeEnum == null)
                throw new ArgumentNullException("withCodeClass");

            if (ns.Members != null)
            {
                foreach (object elem in ns.Members)
                {
                    if (elem is CodeEnum)
                        withCodeEnum((CodeEnum)elem);
                }
            }
        }

        public static void TraverseProperties(CodeElements elements, Action<CodeProperty> withProperty)
        {
            if (withProperty == null)
                throw new ArgumentNullException("withProperty");
            
            if (elements != null)
            {
                foreach (var property in elements)
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
                        {
                            Traversal.TraverseNamespace(
                                (CodeNamespace)elem,
                                withNamespace);
                        }
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

        private static void TraverseNamespace(
            CodeNamespace codeNamespace,
            Action<CodeNamespace> withNamespace)
        {
            withNamespace(codeNamespace);

            if (codeNamespace.Members != null)
            {
                foreach (object elem in codeNamespace.Members)
                {
                    if (elem is CodeNamespace)
                    {
                        Traversal.TraverseNamespace(
                            (CodeNamespace)elem,
                            withNamespace);
                    }
                }
            }
        }

        private static void TraverseClass(
            CodeClass codeClass,
            Action<CodeClass> withClass)
        {
            withClass(codeClass);

            if (codeClass.Children != null)
            {
                foreach (object child in codeClass.Children)
                {
                    if (child is CodeClass)
                    {
                        TraverseClass(
                            (CodeClass)child,
                            withClass);
                    }
                }
            }
        }
    }
}
