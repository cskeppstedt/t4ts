using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.Collections;
using T4TS.Outputs;

namespace T4TS.Builders
{
    public class AttributeInterfaceBuilder : ICodeClassToInterfaceBuilder
    {
        private static readonly string InterfaceAttributeFullName = "T4TS.TypeScriptInterfaceAttribute";
        private static readonly string MemberAttributeFullName = "T4TS.TypeScriptMemberAttribute";

        private Settings settings;

        public AttributeInterfaceBuilder(Settings settings)
        {
            this.settings = settings;
        }

        public TypeScriptInterface Build(
            CodeClass codeClass,
            TypeContext typeContext)
        {
            TypeScriptInterface result = null;

            TypeScriptInterfaceAttributeValues attributeValues = this.GetAttributeValues(codeClass);
            if (attributeValues != null)
            {
                string moduleName = attributeValues.Module;
                if (String.IsNullOrEmpty(moduleName)
                    && codeClass.Namespace != null)
                {
                    moduleName = codeClass.Namespace.FullName;
                }
                
                bool interfaceCreated;
                result = typeContext.GetOrCreateInterface(
                    moduleName,
                    TypeName.ParseDte(codeClass.FullName),
                    GetInterfaceName(attributeValues),
                    out interfaceCreated);
                
                if (!String.IsNullOrEmpty(attributeValues.Extends))
                {
                    result.Parent = typeContext.GetLiteralReference(attributeValues.Extends);
                }
                else if (codeClass.Bases.Count > 0)
                {
                    // Getting the first item directly causes problems in unit tests.  Get it from an enumerator.
                    IEnumerator enumerator = codeClass.Bases.GetEnumerator();
                    enumerator.MoveNext();
                    TypeName parentTypeName = TypeName.ParseDte(
                        ((CodeElement)enumerator.Current).FullName);

                    if (BuilderHelper.IsValidBaseType(parentTypeName))
                    {
                        result.Parent = typeContext.GetTypeReference(
                            parentTypeName,
                            result);
                    }
                }

                result.IndexedType = this.GetIndexedType(
                    result,
                    codeClass,
                    typeContext);

                Traversal.TraverseProperties(
                    codeClass.Members,
                    (property) =>
                    {
                        TypeScriptMember member;
                        if (TryGetMember(
                            result,
                            property,
                            typeContext,
                            out member))
                        {
                            result.Fields.Add(member);
                        }
                    });
            }
            return result;
        }

        private TypeScriptInterfaceAttributeValues GetAttributeValues(
            CodeClass codeClass)
        {
            TypeScriptInterfaceAttributeValues result = null;

            CodeAttribute attribute;
            if (TryGetAttribute(
                    codeClass.Attributes,
                    InterfaceAttributeFullName,
                    out attribute))
            {
                result = this.GetInterfaceValues(
                    codeClass,
                    attribute);
            }
            return result;
        }

        private TypeScriptInterfaceAttributeValues GetInterfaceValues(CodeClass codeClass, CodeAttribute interfaceAttribute)
        {
            var values = GetAttributeValues(interfaceAttribute);

            return new TypeScriptInterfaceAttributeValues
            {
                Name = values.ContainsKey("Name") ? values["Name"] : codeClass.Name,
                Module = values.ContainsKey("Module") ? values["Module"] : this.settings.DefaultModule ?? "T4TS",
                NamePrefix = values.ContainsKey("NamePrefix") ? values["NamePrefix"] : this.settings.DefaultInterfaceNamePrefix ?? string.Empty,
                Extends = values.ContainsKey("Extends") ? values["Extends"] : string.Empty
            };
        }

        private string GetInterfaceName(TypeScriptInterfaceAttributeValues attributeValues)
        {
            if (!string.IsNullOrEmpty(attributeValues.NamePrefix))
                return attributeValues.NamePrefix + attributeValues.Name;

            return attributeValues.Name;
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

        private TypeReference GetIndexedType(
            TypeScriptInterface interfaceContext,
            CodeClass codeClass,
            TypeContext typeContext)
        {
            TypeReference result = null;
            if (codeClass.Bases != null)
            {
                TypeName baseName;
                foreach (CodeElement baseClass in codeClass.Bases)
                {
                    baseName = TypeName.ParseDte(baseClass.FullName);
                    if (baseName.UniversalName == typeof(IEnumerable<>).FullName)
                    {
                        result = typeContext.GetTypeReference(
                            baseName.TypeArguments.First(),
                            interfaceContext);
                    }
                }
            }
            return result;
        }

        private bool TryGetMember(TypeScriptInterface interfaceContext,
            CodeProperty property,
            TypeContext typeContext,
            out TypeScriptMember member)
        {
            member = null;
            if (property.Access != vsCMAccess.vsCMAccessPublic)
                return false;

            var getter = property.Getter;
            if (getter == null)
                return false;

            var values = GetMemberValues(property, typeContext);

            string name;
            if (values.Name != null)
            {
                name = values.Name;
            }
            else
            {
                name = property.Name;
                if (name.StartsWith("@"))
                    name = name.Substring(1);
            }

            TypeReference memberType;
            if (!string.IsNullOrWhiteSpace(values.Type))
            {
                memberType = typeContext.GetLiteralReference(values.Type);
            }
            else
            {
                memberType = typeContext.GetTypeReference(
                    TypeName.ParseDte(getter.Type.AsFullName),
                    interfaceContext);
            }

            member = new TypeScriptMember
            {
                Name = name,
                //FullName = property.FullName,
                Optional = values.Optional,
                Ignore = values.Ignore,
                Type = memberType
            };

            if (member.Ignore)
                return false;

            if (values.CamelCase && values.Name == null)
                member.Name = member.Name.Substring(0, 1).ToLowerInvariant() + member.Name.Substring(1);

            return true;
        }

        private TypeScriptMemberAttributeValues GetMemberValues(CodeProperty property, TypeContext typeContext)
        {
            bool? attributeOptional = null;
            bool? attributeCamelCase = null;
            bool attributeIgnore = false;
            string attributeName = null;
            string attributeType = null;

            CodeAttribute attribute;
            if (TryGetAttribute(property.Attributes, MemberAttributeFullName, out attribute))
            {
                var values = GetAttributeValues(attribute);
                bool parsedProperty;
                if (values.ContainsKey("Optional") && bool.TryParse(values["Optional"], out parsedProperty))
                    attributeOptional = parsedProperty;

                if (values.ContainsKey("CamelCase") && bool.TryParse(values["CamelCase"], out parsedProperty))
                    attributeCamelCase = parsedProperty;

                if (values.ContainsKey("Ignore") && bool.TryParse(values["Ignore"], out parsedProperty))
                    attributeIgnore = parsedProperty;

                values.TryGetValue("Name", out attributeName);
                values.TryGetValue("Type", out attributeType);
            }

            return new TypeScriptMemberAttributeValues
            {
                Optional = attributeOptional.HasValue ? attributeOptional.Value : this.settings.DefaultOptional,
                Name = attributeName,
                Type = attributeType,
                CamelCase = attributeCamelCase ?? this.settings.DefaultCamelCaseMemberNames,
                Ignore = attributeIgnore
            };
        }

        private Dictionary<string, string> GetAttributeValues(CodeAttribute codeAttribute)
        {
            var values = new Dictionary<string, string>();
            foreach (CodeElement child in codeAttribute.Children)
            {
                var property = (EnvDTE80.CodeAttributeArgument)child;
                if (property == null || property.Value == null)
                    continue;

                // remove quotes if the property is a string
                string val = property.Value ?? string.Empty;
                if (val.StartsWith("\"") && val.EndsWith("\""))
                    val = val.Substring(1, val.Length - 2);

                values.Add(property.Name, val);
            }

            return values;
        }
    }
}
