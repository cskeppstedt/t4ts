using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using T4TS.Builders;

namespace T4TS
{
    public class CodeTraverser
    {
        public Solution Solution { get; private set; }
        public Settings Settings { get; private set; }

        private static readonly string InterfaceAttributeFullName = "T4TS.TypeScriptInterfaceAttribute";
        private static readonly string MemberAttributeFullName = "T4TS.TypeScriptMemberAttribute";

        private AttributeInterfaceBuilder builder;

        public CodeTraverser(Solution solution, Settings settings)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

            if (settings == null)
                throw new ArgumentNullException("settings");

            Solution = solution;
            this.Settings = settings;

            builder = new AttributeInterfaceBuilder(this.Settings);
        }

        public IEnumerable<TypeScriptModule> GetAllInterfaces()
        {
            var tsMap = new Dictionary<CodeClass, TypeScriptInterface>();
            TypeContext typeContext = new TypeContext(this.Settings.UseNativeDates);

            Traversal.TraverseNamespacesInSolution(this.Solution, (ns) =>
            {
                Traversal.TraverseClassesInNamespace(ns, (codeClass) =>
                {
                    TypeScriptInterface tsInterface = this.builder.Build(
                        codeClass,
                        typeContext);
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

            return typeContext.GetModules()
                .OrderBy(m => m.QualifiedName)
                .ToList();
        }
    }
}
