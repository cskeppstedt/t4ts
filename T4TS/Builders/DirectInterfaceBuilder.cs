using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace T4TS.Builders
{
    public partial class DirectInterfaceBuilder : CodeClassInterfaceBuilder
    {
        private DirectSettings settings;

        public DirectInterfaceBuilder(DirectSettings settings)
        {
            this.settings = settings;
        }

        public TypeScriptInterface Build(
            CodeClass codeClass,
            TypeContext typeContext)
        {
            TypeScriptInterface result = null;

            string moduleName = this.settings.GetModuleNameFromNamespace(codeClass.Namespace);
                
            bool interfaceCreated;
            result = typeContext.GetOrCreateInterface(
                moduleName,
                codeClass.FullName,
                out interfaceCreated);

            result.Name = codeClass.Name;

            TypeFullName fullName = TypeFullNameParser.Parse(codeClass.FullName);
            if (fullName.TypeArgumentFullNames != null)
            {
                foreach (TypeFullName typeArgumentName in fullName.TypeArgumentFullNames)
                {
                    result.GenericParameters.Add(typeArgumentName.FullName);
                }
            }

            bool createdBase;
            if (codeClass.Bases != null)
            {
                foreach (CodeElement baseElement in codeClass.Bases)
                {
                    if (baseElement.FullName != typeof(Object).FullName)
                    {
                        result.Bases.Add(
                            typeContext.GetOrCreateOutputType(
                                baseElement.FullName,
                                resolveOutputOnly: true,
                                created: out createdBase));
                    }
                }
                result.Parent = result.Bases.FirstOrDefault();
            }

            TypescriptType indexedType;
            if (TryGetIndexedType(codeClass, typeContext, out indexedType))
            {
                result.IndexedType = indexedType;
            }

            Traversal.TraversePropertiesInClass(codeClass, (property) =>
            {
                TypeScriptInterfaceMember member;
                if (TryGetMember(property, typeContext, out member))
                {
                    result.Members.Add(member);
                }
            });
            return result;
        }

        private bool TryGetAttribute(CodeElements attributes, string attributeFullName, out CodeAttribute attribute)
        {
            foreach (CodeAttribute attr in attributes)
            {
                if (attr.FullName == attributeFullName)
                {
                    attribute = attr;
                    return true;
                }
            }

            attribute = null;
            return false;
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

        private bool TryGetMember(CodeProperty property, TypeContext typeContext, out TypeScriptInterfaceMember member)
        {
            member = null;
            if (property.Access != vsCMAccess.vsCMAccessPublic)
                return false;

            var getter = property.Getter;
            if (getter == null)
                return false;

            string name = property.Name;
            if (name.StartsWith("@"))
            {
                name = name.Substring(1);
            }
            if (this.settings.CamelCase)
            {
                name = name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
            }

            bool typeCreated;
            member = new TypeScriptInterfaceMember
            {
                Name = name,
                Type = typeContext.GetOrCreateOutputType(
                    getter.Type.AsFullName,
                    resolveOutputOnly: false,
                    created: out typeCreated)
            };
            return true;
        }
    }
}
