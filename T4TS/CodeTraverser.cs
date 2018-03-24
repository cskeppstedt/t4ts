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
            IDictionary<string, IList<CodeNamespace>> namespacesByName
                = new Dictionary<string, IList<CodeNamespace>>();

            Traversal.TraverseNamespacesInSolution(
                this.solution,
                (codeNamespace) =>
                {
                    this.TraverseNamespace(
                        codeNamespace,
                        this.Settings);

                    if (this.Settings.ResolveReferences)
                    {
                        IList<CodeNamespace> namespaceList;
                        if (!namespacesByName.TryGetValue(
                            codeNamespace.FullName,
                            out namespaceList))
                        {
                            namespaceList = new List<CodeNamespace>();
                            namespacesByName.Add(
                                codeNamespace.FullName,
                                namespaceList);
                        }
                        namespaceList.Add(codeNamespace);
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
            IDictionary<string, IList<CodeNamespace>> namespacesByName,
            ICollection<string> fullNamesToIgnore)
        {
            int result = 0;

            if (fullNamesToIgnore == null)
            {
                fullNamesToIgnore = new HashSet<string>();
            }
            IDictionary<IList<CodeNamespace>, ICollection<string>> typeNamesByNamespaces =
                new Dictionary<IList<CodeNamespace>, ICollection<string>>();

            TypeName sourceType;
            TypeScriptOutputType outputType;
            IList<CodeNamespace> namespaceList;
            ICollection<string> typeNames;
            IList<TypeScriptDelayResolveType> delayTypes = this.context.GetDelayResolveTypes().ToList();
            foreach (TypeScriptDelayResolveType delayType in delayTypes)
            {
                sourceType = delayType.SourceType;
                if (!fullNamesToIgnore.Contains(sourceType.UniversalName))
                {
                    outputType = this.context.GetOutput(sourceType);
                    if (outputType == null)
                    {
                        outputType = this.context.GetSystemOutputType(sourceType);
                    }
                    if (outputType == null)
                    {
                        if (namespacesByName.TryGetValue(
                            sourceType.Namespace,
                            out namespaceList))
                        {
                            if (!typeNamesByNamespaces.TryGetValue(
                                namespaceList,
                                out typeNames))
                            {
                                typeNames = new HashSet<string>();
                                typeNamesByNamespaces.Add(
                                    namespaceList,
                                    typeNames);
                            }

                            typeNames.Add(sourceType.UnqualifiedTypeName);
                            result++;
                        }
                        else
                        {
                            throw new Exception(String.Format(
                                "Can't resolve type {0} because the namespace is unknown",
                                sourceType.QualifiedName));
                        }
                    }
                    fullNamesToIgnore.Add(sourceType.UniversalName);
                }
            }

            TraverserSettings settings;
            foreach (KeyValuePair<IList<CodeNamespace>, ICollection<string>> namespaceTypeNamesPair
                in typeNamesByNamespaces)
            {
                settings = new TraverserSettings()
                {
                    InterfaceBuilder = this.Settings.InterfaceBuilder,
                    EnumBuilder = this.Settings.EnumBuilder,
                    ClassFilter = (codeClass) => namespaceTypeNamesPair.Value.Contains(codeClass.Name),
                    EnumFilter = (codeEnum) => namespaceTypeNamesPair.Value.Contains(codeEnum.Name)
                };

                foreach (CodeNamespace codeNamespace in namespaceTypeNamesPair.Key)
                {
                    this.TraverseNamespace(
                        codeNamespace,
                        settings);
                }
            }

            if (typeNamesByNamespaces.Count > 0)
            {
                this.ResolveReferenceTypes(
                    namespacesByName,
                    fullNamesToIgnore);
            }
        }
    }
}
