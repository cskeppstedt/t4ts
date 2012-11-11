﻿using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Generator
{
    public class CodeGenerator
    {
        public Project Project { get; private set; }
        private static readonly string AttributeFullName = typeof(TypeScriptInterfaceAttribute).FullName;
        private static readonly string DefaultModuleName = "Api";

        private static readonly string[] genericCollectionTypeStarts = new string[] {
            "System.Collections.Generic.List<",
            "System.Collections.Generic.IList<",
            "System.Collections.Generic.ICollection<"
        };

        public CodeGenerator(Project project)
        {
            this.Project = project;
        }

        public IEnumerable<TypeScriptInterface> GetInterfaces()
        {
            // A context that holds all interfaces that will be generated.
            // Keyed on the FullName of the CodeType.
            var typeContext = new TypeContext(VisitProjectItems(Project.ProjectItems)
                .ToDictionary(i => i.CodeType.FullName, i => i));

            foreach (var instance in typeContext.Values)
                yield return GetInterface(instance, typeContext);
        }

        private IEnumerable<AttributeDecoratedInstance> VisitProjectItems(ProjectItems items)
        {
            foreach (ProjectItem pi in items)
            {
                if (pi.FileCodeModel != null)
                {
                    var codeElements = pi.FileCodeModel.CodeElements;
                    foreach (CodeElement codeElement in codeElements)
                    {
                        if (codeElement is CodeNamespace)
                        {
                            foreach (var instance in FindClassesWithAttribute(codeElement as CodeNamespace))
                                yield return instance;
                        }
                    }
                }

                if (pi.ProjectItems != null && pi.ProjectItems.Count > 0)
                {
                    foreach (var tsInterface in VisitProjectItems(pi.ProjectItems))
                        yield return tsInterface;
                }
            }
        }

        private IEnumerable<AttributeDecoratedInstance> FindClassesWithAttribute(CodeNamespace ns)
        {
            foreach (CodeElement codeElement in ns.Members)
            {
                if (codeElement is CodeType)
                {
                    var ct = codeElement as CodeType;
                    if (ct.Attributes == null)
                        continue;

                    foreach (CodeAttribute attr in ct.Attributes)
                    {
                        if (attr.FullName == AttributeFullName)
                        {
                            yield return new AttributeDecoratedInstance
                            {
                                CodeType = ct,
                                Namespace = ns,
                                TypescriptType = ct.Name,
                                Module = GetAttributeModuleName(attr)
                            };

                            break;
                        }
                    }
                }
            }
        }

        private string GetAttributeModuleName(CodeAttribute codeAttribute)
        {
            foreach (CodeElement child in codeAttribute.Children)
            {
                EnvDTE80.CodeAttributeArgument property = (EnvDTE80.CodeAttributeArgument)child;
                if (property.Name == "Module")
                    return property.Value.Replace("\"","");
            }

            return DefaultModuleName;
        }

        private TypeScriptInterface GetInterface(AttributeDecoratedInstance instance, TypeContext typeContext)
        {
            var tsInterface = new TypeScriptInterface
            {
                FullName = instance.CodeType.FullName,
                Name = instance.CodeType.Name,
                Module = instance.Module,
                Members = GetMembers(instance, typeContext).ToList()
            };

            if (instance.CodeType.Bases.Count > 0)
            {
                foreach (CodeElement elem in instance.CodeType.Bases)
                {
                    if (genericCollectionTypeStarts.Any(elem.FullName.StartsWith))
                    {
                        string fullName = UnwrapGenericType(elem.FullName);
                        if (typeContext.ContainsKey(fullName))
                        {
                            tsInterface.IndexerType = typeContext[fullName].CodeType.Name;
                            return tsInterface;
                        }
                    }
                }
            }

            return tsInterface;
        }

        private IEnumerable<TypeScriptInterfaceMember> GetMembers(AttributeDecoratedInstance instance, TypeContext typeContext)
        {
            foreach (CodeElement codeElement in instance.CodeType.Members)
            {
                if (!(codeElement is CodeProperty))
                    continue;

                var codeProperty = (CodeProperty)codeElement;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic)
                    continue;

                var func = codeProperty.Getter;
                if (func != null)
                {
                    yield return new TypeScriptInterfaceMember
                    {
                        Name = codeProperty.Name,
                        Type = GetTypeScriptType(instance, func.Type, codeProperty, typeContext)
                    };
                }
            }
        }

        private string GetTypeScriptType(AttributeDecoratedInstance instance, CodeTypeRef codeType, CodeProperty codeProperty, TypeContext typeContext)
        {
            switch (codeType.TypeKind)
            {
                case vsCMTypeRef.vsCMTypeRefChar:
                case vsCMTypeRef.vsCMTypeRefString:
                    return "string";

                case vsCMTypeRef.vsCMTypeRefBool:
                    return "bool";

                case vsCMTypeRef.vsCMTypeRefByte:
                case vsCMTypeRef.vsCMTypeRefDouble:
                case vsCMTypeRef.vsCMTypeRefInt:
                case vsCMTypeRef.vsCMTypeRefShort:
                case vsCMTypeRef.vsCMTypeRefFloat:
                case vsCMTypeRef.vsCMTypeRefLong:
                case vsCMTypeRef.vsCMTypeRefDecimal:
                    return "number";

                default:
                    return TryResolveType(instance, codeType, codeProperty, typeContext);
            }
        }

        private string TryResolveType(AttributeDecoratedInstance instance, CodeTypeRef codeType, CodeProperty codeProperty, TypeContext typeContext)
        {
            if (codeType.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
            {
                string typeFullName = codeType.ElementType.AsFullName;
                return TryResolveEnumerableType(typeFullName, typeContext);
            }

            if (genericCollectionTypeStarts.Any(s => codeType.AsFullName.StartsWith(s)))
            {
                string fullName = UnwrapGenericType(codeType);
                return TryResolveEnumerableType(fullName, typeContext);
            }

            return TryResolveUnknownType(codeType.AsFullName, typeContext);
        }

        private string TryResolveEnumerableType(string typeFullName, TypeContext typeContext)
        {
            return TryResolveUnknownType(typeFullName, typeContext) + "[]";
        }

        private string TryResolveUnknownType(string typeFullName, TypeContext typeContext)
        {
            AttributeDecoratedInstance instance;
            if (typeContext.TryGetValue(typeFullName, out instance))
                return instance.CodeType.Name;

            switch (typeFullName)
            {
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Decimal":
                case "System.Byte":
                case "System.SByte":
                case "System.Single":
                    return "number";

                case "System.String":
                case "System.DateTime":
                    return "string";
                
                case "System.Object":
                    return "Object";

                default:
                    return "any";
            }
        }

        private string UnwrapGenericType(CodeTypeRef codeType)
        {
            return UnwrapGenericType(codeType.AsFullName);
        }

        private string UnwrapGenericType(string typeFullName)
        {
            return typeFullName.Split('<', '>')[1];
        }
    }
}