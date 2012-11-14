using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class CodeGenerator
    {
        public Project Project { get; private set; }
        private static readonly string AttributeFullName = typeof(TypeScriptInterfaceAttribute).FullName;

        public CodeGenerator(Project project)
        {
            this.Project = project;
        }

        public TypeContext BuildContext()
        {
            var typeContext = new TypeContext();

            new ProjectTraverser(this.Project, (ns) =>
            {
                new NamespaceTraverser(ns, (codeClass) =>
                {
                    var values = GetInterfaceValues(codeClass);
                    var customType = new CustomType(values.Name, values.Module);

                    typeContext.AddCustomType(codeClass.FullName, customType);
                });
            });

            return typeContext;
        }

        public IEnumerable<TypeScriptModule> GetAllInterfaces()
        {
            var typeContext = BuildContext();
            var modules = new Dictionary<string, TypeScriptModule>();

            new ProjectTraverser(this.Project, (ns) =>
            {
                new NamespaceTraverser(ns, (codeClass) =>
                {
                    var values = GetInterfaceValues(codeClass);

                    TypeScriptModule module;
                    if (!modules.TryGetValue(values.Module, out module))
                    {
                        module = new TypeScriptModule { QualifiedName = values.Module };
                        modules.Add(values.Module, module);
                    }

                    var tsInterface = new TypeScriptInterface
                    {
                        FullName = codeClass.FullName,
                        Name = values.Name
                    };

                    TypescriptType indexedType;
                    if (TryGetIndexedType(codeClass, typeContext, out indexedType))
                        tsInterface.IndexedType = indexedType;

                    new ClassTraverser(codeClass, (property) =>
                    {
                        TypeScriptInterfaceMember member;
                        if (TryGetMember(property, typeContext, out member))
                            tsInterface.Members.Add(member);
                    });

                    module.Interfaces.Add(tsInterface);
                });
            });

            return modules.Values;
        }

        private bool TryGetIndexedType(CodeClass codeClass, TypeContext typeContext, out TypescriptType indexedType)
        {
            indexedType = null;
            if (codeClass.Bases == null || codeClass.Bases.Count == 0)
                return false;

            foreach (CodeElement baseClass in codeClass.Bases)
            {
                if (typeContext.IsGenericEnumerable(baseClass.FullName))
                {
                    string fullName = typeContext.UnwrapGenericType(baseClass.FullName);
                    indexedType = typeContext.GetTypeScriptType(fullName);
                    return true;
                }
            }

            return false;
        }

        private TypeScriptInterfaceAttributeValues GetInterfaceValues(CodeClass codeClass)
        {
            // TODO: implement
            return new TypeScriptInterfaceAttributeValues
            {
                Name = codeClass.Name,
                Module = "T4TS"
            };
        }

        private bool TryGetMember(CodeProperty property, TypeContext typeContext, out TypeScriptInterfaceMember member)
        {
            member = null;
            if (property.Access != vsCMAccess.vsCMAccessPublic)
                return false;

            var getter = property.Getter;
            if (getter == null)
                return false;
            
            var values = GetMemberValues(property, typeContext);
            member = new TypeScriptInterfaceMember
            {
                Name = values.Name ?? property.Name,
                FullName = property.FullName,
                Optional = values.Optional,
                Type = (string.IsNullOrWhiteSpace(values.Type))
                    ? typeContext.GetTypeScriptType(getter.Type)
                    : new CustomType(values.Type)
            };

            return true;
        }

        private MemberAttributeValues GetMemberValues(CodeProperty property, TypeContext typeContext)
        {
            // TODO: implement
            return new MemberAttributeValues();
        }
    }
}
