using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using T4TS.Builders;

namespace T4TS
{
    public partial class CodeTraverser
    {
        private Solution solution;
        private TypeContext context;

        public TraverserSettings Settings { get; set; }

        public CodeTraverser(
            Solution solution,
            TypeContext context)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

            if (context == null)
                throw new ArgumentNullException("context");
            
            this.solution = solution;
            this.context = context;

            this.Settings = new TraverserSettings();
        }

        public IEnumerable<TypeScriptModule> GetAllInterfaces()
        {
            IDictionary<string, CodeNamespace> namespacesByName = new Dictionary<string, CodeNamespace>();

            Traversal.TraverseNamespacesInSolution(
                this.solution,
                (codeNamespace) =>
                {
                    this.TraverseNamespace(
                        codeNamespace,
                        this.Settings);

                    if (this.Settings.ResolveReferences)
                    {
                        namespacesByName.Add(
                            codeNamespace.FullName,
                            codeNamespace);
                    }
                });

            if (this.Settings.ResolveReferences)
            {
                this.ResolveReferenceTypes(
                    namespacesByName,
                    fullNamesToIgnore: null);
            }

            return this.context.GetModules()
                .OrderBy(m => m.QualifiedName)
                .ToList();
        }

        private void TraverseNamespace(
            CodeNamespace codeNamespace,
            TraverserSettings settings)
        {
            if (settings.NamespaceFilter == null
                || settings.NamespaceFilter(codeNamespace))
            {
                if (settings.InterfaceBuilder != null)
                {
                    Traversal.TraverseClassesInNamespace(
                        codeNamespace,
                        (codeClass) =>
                        {
                            if (settings.ClassFilter == null
                                || settings.ClassFilter(codeClass))
                            {
                                settings.InterfaceBuilder.Build(
                                    codeClass,
                                    this.context);
                            }
                        });
                }

                if (settings.EnumBuilder != null)
                {
                    Traversal.TraverseEnumsInNamespace(
                        codeNamespace,
                        (codeEnum) =>
                        {
                            if (settings.EnumFilter == null
                                || settings.EnumFilter(codeEnum))
                            {
                                settings.EnumBuilder.Build(
                                    codeEnum,
                                    this.context);
                            }
                        });
                }
            }
        }

        private void ResolveReferenceTypes(
            IDictionary<string, CodeNamespace> namespacesByName,
            ICollection<string> fullNamesToIgnore)
        {
            int result = 0;

            if (fullNamesToIgnore == null)
            {
                fullNamesToIgnore = new HashSet<string>();
            }
            IDictionary<CodeNamespace, ICollection<string>> typeNamesByNamespace =
                new Dictionary<CodeNamespace, ICollection<string>>();

            string fullName;
            TypeScriptOutputType outputType;
            string namespaceName;
            CodeNamespace codeNamespace;
            ICollection<string> typeNames;
            foreach (TypeScriptDelayResolveType delayType in this.context.GetDelayLoadTypes())
            {
                fullName = delayType.FullName;
                if (!fullNamesToIgnore.Contains(fullName))
                {
                    outputType = this.context.GetOutput(fullName);
                    if (outputType == null)
                    {
                        int classNameStartIndex = fullName.LastIndexOf('.');
                        namespaceName = fullName.Substring(
                            0,
                            classNameStartIndex);

                        if (namespacesByName.TryGetValue(
                            namespaceName,
                            out codeNamespace))
                        {
                            if (!typeNamesByNamespace.TryGetValue(
                                codeNamespace,
                                out typeNames))
                            {
                                typeNames = new HashSet<string>();
                                typeNamesByNamespace.Add(
                                    codeNamespace,
                                    typeNames);
                            }

                            typeNames.Add(
                                fullName.Substring(classNameStartIndex + 1));
                            result++;
                        }
                    }
                    fullNamesToIgnore.Add(fullName);
                }
            }

            TraverserSettings settings;
            foreach (KeyValuePair<CodeNamespace, ICollection<string>> namespaceTypeNamesPair
                in typeNamesByNamespace)
            {
                settings = new TraverserSettings()
                {
                    InterfaceBuilder = this.Settings.InterfaceBuilder,
                    EnumBuilder = this.Settings.EnumBuilder,
                    ClassFilter = (codeClass) => namespaceTypeNamesPair.Value.Contains(codeClass.Name),
                    EnumFilter = (codeEnum) => namespaceTypeNamesPair.Value.Contains(codeEnum.Name)
                };

                this.TraverseNamespace(
                    namespaceTypeNamesPair.Key,
                    settings);
            }

            if (typeNamesByNamespace.Count > 0)
            {
                this.ResolveReferenceTypes(
                    namespacesByName,
                    fullNamesToIgnore);
            }
        }
    }
}
