using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Generator
{
    public class CodeGenerator
    {
        public Project Project { get; private set; }

        public CodeGenerator(Project project)
        {
            this.Project = project;
        }

        public IEnumerable<TypeScriptInterface> GetInterfaces()
        {
            return VisitProjectItems(Project.ProjectItems);
        }

        private IEnumerable<TypeScriptInterface> VisitProjectItems(ProjectItems items)
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
                            foreach (var tsInterface in FindClassesWithAttribute(codeElement as CodeNamespace))
                                yield return tsInterface;
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

        private IEnumerable<TypeScriptInterface> FindClassesWithAttribute(CodeNamespace ns)
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
                        if (attr.FullName == typeof(TypeScriptInterfaceAttribute).FullName)
                            yield return GetInterface(ct);
                    }
                }
            }
        }

        private TypeScriptInterface GetInterface(CodeType ct)
        {
            return new TypeScriptInterface
            {
                Name = ct.Name,
                Members = GetMembers(ct.Members).ToList()
            };
        }

        private IEnumerable<TypeScriptInterfaceMember> GetMembers(CodeElements members)
        {
            foreach (CodeElement codeElement in members)
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
                        Type = GetTypeScriptType(func.Type)
                    };
                }
            }
        }

        private string GetTypeScriptType(CodeTypeRef codeType)
        {
            switch (codeType.TypeKind)
            {
                case vsCMTypeRef.vsCMTypeRefChar:
                case vsCMTypeRef.vsCMTypeRefString:
                    return "string";

                case vsCMTypeRef.vsCMTypeRefBool:
                    return "bool";

                case vsCMTypeRef.vsCMTypeRefArray:
                    return "any[]";

                case vsCMTypeRef.vsCMTypeRefByte:
                case vsCMTypeRef.vsCMTypeRefDouble:
                case vsCMTypeRef.vsCMTypeRefInt:
                case vsCMTypeRef.vsCMTypeRefShort:
                case vsCMTypeRef.vsCMTypeRefFloat:
                case vsCMTypeRef.vsCMTypeRefLong:
                case vsCMTypeRef.vsCMTypeRefDecimal:
                    return "number";

                default: return "any";
            }
        }
    }
}
