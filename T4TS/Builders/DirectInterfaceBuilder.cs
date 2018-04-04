using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using T4TS.Outputs;
using T4TS.Builders;

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
                TypeName.ParseDte(codeClass.FullName),
                codeClass.Name,
                out interfaceCreated);
            
            if (codeClass.Bases != null)
            {
                foreach (CodeElement baseElement in codeClass.Bases)
                {
                    if (baseElement.FullName != typeof(Object).FullName)
                    {
                        result.Bases.Add(
                            typeContext.GetTypeReference(
                                TypeName.ParseDte(baseElement.FullName)));
                    }
                }

                TypeReference parentType = result.Bases.FirstOrDefault();
                if (parentType != null
                    && BuilderHelper.IsValidBaseType(parentType.SourceType))
                {
                    result.Parent = parentType;
                }
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
            
            member = new TypeScriptInterfaceMember
            {
                Name = name,
                Type = typeContext.GetTypeReference(
                    TypeName.ParseDte(getter.Type.AsFullName))
            };
            return true;
        }
    }
}
