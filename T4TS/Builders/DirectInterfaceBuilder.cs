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
    public partial class DirectInterfaceBuilder
        : ICodeClassToInterfaceBuilder,
        ICodeInterfaceToInterfaceBuilder
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

            this.PopulateBases(
                codeClass.Bases,
                result,
                typeContext);

            this.PopulateMembers(
                codeClass.Members,
                result,
                typeContext);

            return result;
        }

        public TypeScriptInterface Build(
            CodeInterface codeInterface,
            TypeContext typeContext)
        {
            TypeScriptInterface result = null;

            string moduleName = this.settings.GetModuleNameFromNamespace(codeInterface.Namespace);

            bool interfaceCreated;
            result = typeContext.GetOrCreateInterface(
                moduleName,
                TypeName.ParseDte(codeInterface.FullName),
                codeInterface.Name,
                out interfaceCreated);

            this.PopulateBases(
                codeInterface.Bases,
                result,
                typeContext);

            this.PopulateMembers(
                codeInterface.Members,
                result,
                typeContext);

            return result;
        }

        private void PopulateBases(
            CodeElements bases,
            TypeScriptInterface interfaceOutput,
            TypeContext typeContext)
        {
            if (bases != null)
            {
                foreach (CodeElement baseElement in bases)
                {
                    if (baseElement.FullName != typeof(Object).FullName)
                    {
                        interfaceOutput.Bases.Add(
                            typeContext.GetTypeReference(
                                TypeName.ParseDte(baseElement.FullName),
                                interfaceOutput));
                    }
                }

                TypeReference parentType = interfaceOutput.Bases.FirstOrDefault();
                if (parentType != null
                    && BuilderHelper.IsValidBaseType(parentType.SourceType))
                {
                    interfaceOutput.Parent = parentType;
                }
            }
        }

        private void PopulateMembers(
            CodeElements members,
            TypeScriptInterface interfaceOutput,
            TypeContext typeContext)
        {
            Traversal.TraverseProperties(
                members,
                (property) =>
                {
                    TypeScriptInterfaceMember member;
                    if (TryGetMember(
                        interfaceOutput,
                        property,
                        typeContext,
                        out member))
                    {
                        interfaceOutput.Members.Add(member);
                    }
                });
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

        private bool TryGetMember(
            TypeScriptInterface interfaceContext,
            CodeProperty property,
            TypeContext typeContext,
            out TypeScriptInterfaceMember member)
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
                    TypeName.ParseDte(getter.Type.AsFullName),
                    interfaceContext)
            };
            return true;
        }
    }
}
