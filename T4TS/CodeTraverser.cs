using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using T4TS.Builders;

namespace T4TS
{
    public class CodeTraverser
    {
        private Solution solution;
        private CodeClassInterfaceBuilder builder;
        private TypeContext context;

        public CodeTraverser(
            Solution solution,
            TypeContext context,
            CodeClassInterfaceBuilder builder)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

            this.solution = solution;
            this.context = context;
            this.builder = builder;
        }

        public IEnumerable<TypeScriptModule> GetAllInterfaces()
        {
            var tsMap = new Dictionary<CodeClass, TypeScriptInterface>();

            Traversal.TraverseNamespacesInSolution(
                this.solution,
                (ns) =>
                {
                    Traversal.TraverseClassesInNamespace(ns, (codeClass) =>
                    {
                        TypeScriptInterface tsInterface = this.builder.Build(
                            codeClass,
                            this.context);
                        if (tsInterface != null)
                        {
                            tsMap.Add(codeClass, tsInterface);
                        }
                    });
                });

            var tsInterfaces = tsMap.Values.ToList();
            tsMap.Keys.ToList().ForEach(codeClass =>
            {
                CodeElements baseClasses = codeClass.Bases;
                if (baseClasses != null && baseClasses.Count > 0)
                {
                    CodeElement baseClass = baseClasses.Item(1);
                    if (baseClass != null)
                    {
                        ///since this is traversing project files, a class's base class can be defined in multiple files, if that is a partial class.
                        ///SingleOrDefault fails if it finds multiple files ==> That's why use FirstOrDefault
                        var parent = tsInterfaces.FirstOrDefault(intf => intf.FullName == baseClass.FullName);
                        if (parent != null)
                        {
                            tsMap[codeClass].Parent = parent;
                        }
                    }
                }
            });

            return this.context.GetModules()
                .OrderBy(m => m.QualifiedName)
                .ToList();
        }
    }
}
