using EnvDTE;
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
                                TypescriptType = ct.Name
                            };

                            break;
                        }
                    }
                }
            }
        }

        private TypeScriptInterface GetInterface(AttributeDecoratedInstance instance, TypeContext typeContext)
        {
            return new TypeScriptInterface
            {
                Name = instance.CodeType.Name,
                Members = GetMembers(instance, typeContext).ToList()
            };
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
                string elemType = codeType.ElementType.AsFullName;
                if (typeContext.ContainsKey(elemType))
                    return typeContext[elemType].CodeType.Name + "[]";
                
                return "any[]";
            }

            if (typeContext.ContainsKey(codeType.AsFullName))
                return typeContext[codeType.AsFullName].CodeType.Name;

            if (genericCollectionTypeStarts.Any(s => codeType.AsFullName.StartsWith(s)))
            {
                string fullName = codeType.AsFullName.Split('<', '>')[1];
                if (typeContext.ContainsKey(fullName))
                    return typeContext[fullName].CodeType.Name + "[]";

                return "any[]";
            }

            return "any";
        }
    }
}
