using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using T4TS.Builders;
using T4TS.Outputs;

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
                    attemptedSourceNames: null);
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
                if (settings.ClassToInterfaceBuilder != null)
                {
                    Traversal.TraverseClassesInNamespace(
                        codeNamespace,
                        (codeClass) =>
                        {
                            if (settings.ClassFilter == null
                                || settings.ClassFilter(codeClass))
                            {
                                TypeScriptInterface builtType = 
                                    settings.ClassToInterfaceBuilder.Build(
                                        codeClass,
                                        this.context);

                                if (builtType != null
                                    && settings.TypeDecorators != null
                                    && settings.TypeDecorators.Any())
                                {
                                    foreach (Action<TypeScriptType> typeDecorator in settings.TypeDecorators)
                                    {
                                        typeDecorator(builtType);
                                    }
                                }
                            }
                        });
                }

                if (settings.InterfaceToInterfaceBuilder != null)
                {
                    Traversal.TraverseInterfacesInNamespace(
                        codeNamespace,
                        (codeInterface) =>
                        {
                            if (settings.InterfaceFilter == null
                                || settings.InterfaceFilter(codeInterface))
                            {
                                TypeScriptInterface builtType = 
                                    settings.InterfaceToInterfaceBuilder.Build(
                                        codeInterface,
                                        this.context);

                                if (builtType != null
                                    && settings.TypeDecorators != null
                                    && settings.TypeDecorators.Any())
                                {
                                    foreach(Action<TypeScriptType> typeDecorator in settings.TypeDecorators)
                                    {
                                        typeDecorator(builtType);
                                    }
                                }
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
            ICollection<string> attemptedSourceNames)
        {
            int result = 0;

            if (attemptedSourceNames == null)
            {
                attemptedSourceNames = new HashSet<string>();
            }
            IDictionary<IList<CodeNamespace>, ICollection<string>> typeNamesByNamespaces =
                new Dictionary<IList<CodeNamespace>, ICollection<string>>();

            TypeName outputName;
            IList<CodeNamespace> namespaceList;
            ICollection<string> typeNames;
            IList<TypeReference> typeReferences = this.context.GetTypeReferences().ToList();
            foreach (TypeReference typeReference in typeReferences)
            {
                if (!attemptedSourceNames.Contains(typeReference.SourceType.UniversalName))
                {
                    outputName = this.context.ResolveOutputTypeName(
                        typeReference,
                        allowNull: true);
                    if (outputName == null)
                    {
                        if (namespacesByName.TryGetValue(
                            typeReference.SourceType.Namespace,
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

                            typeNames.Add(typeReference.SourceType.UniversalName);
                            result++;
                        }
                        else if (typeReference.ContextTypeReference == null
                            && this.Settings.FailOnUnresolvedReferences)
                        {
                            throw new Exception(String.Format(
                                "Can't resolve type {0} because the namespace is unknown",
                                typeReference.SourceType.QualifiedName));
                        }
                    }
                    attemptedSourceNames.Add(typeReference.SourceType.UniversalName);
                }
            }

            TraverserSettings settings;
            foreach (KeyValuePair<IList<CodeNamespace>, ICollection<string>> namespaceTypeNamesPair
                in typeNamesByNamespaces)
            {
                settings = new TraverserSettings()
                {
                    ClassToInterfaceBuilder = this.Settings.ClassToInterfaceBuilder,
                    EnumBuilder = this.Settings.EnumBuilder,
                    ClassFilter = (codeClass) =>
                    {
                        TypeName currentName = TypeName.ParseDte(codeClass.FullName);
                        return namespaceTypeNamesPair.Value.Contains(currentName.UniversalName);
                    },
                    EnumFilter = (codeEnum) =>
                    {
                        TypeName currentName = TypeName.ParseDte(codeEnum.FullName);
                        return namespaceTypeNamesPair.Value.Contains(currentName.UniversalName);
                    },
                    TypeDecorators = this.Settings.TypeDecorators
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
                    attemptedSourceNames);
            }
        }
    }
}
