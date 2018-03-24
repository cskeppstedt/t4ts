using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.Collections;

namespace T4TS.Builders
{
    public class AttributeInterfaceBuilder : CodeClassInterfaceBuilder
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
                    out interfaceCreated);

                result.Name = GetInterfaceName(attributeValues);
                if (!String.IsNullOrEmpty(attributeValues.Extends))
                {
                    result.Parent = new TypeScriptLiteralType()
                    {
                        FullName = attributeValues.Extends
                    };
                }
                else if (codeClass.Bases.Count > 0)
                {
                    // Getting the first item directly causes problems in unit tests.  Get it from an enumerator.
                    IEnumerator enumerator = codeClass.Bases.GetEnumerator();
                    enumerator.MoveNext();
                    string parentTypeName = ((CodeElement)enumerator.Current).FullName;
                    if (!typeContext.IsGenericEnumerable(parentTypeName))
                    {
                        result.Parent = typeContext.GetResolvableTypeFromDteName(parentTypeName);
                    }
                }

                TypeScriptOutputType indexedType;
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

        private bool TryGetIndexedType(
            CodeClass codeClass,
            TypeContext typeContext,
            out TypeScriptOutputType indexedType)
        {
            indexedType = null;
            if (codeClass.Bases == null || codeClass.Bases.Count == 0)
                return false;

            foreach (CodeElement baseClass in codeClass.Bases)
            {
                if (typeContext.IsGenericEnumerable(baseClass.FullName))
                {
                    string fullName = typeContext.UnwrapGenericType(baseClass.FullName);
                    indexedType = typeContext.GetResolvableTypeFromDteName(fullName);
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

            TypeScriptOutputType type;
            if (!string.IsNullOrWhiteSpace(values.Type))
            {
                type = new TypeScriptLiteralType()
                {
                    FullName = values.Type
                };
            }
            else
            {
                type = typeContext.GetResolvableTypeFromDteName(getter.Type.AsFullName);
            }

            member = new TypeScriptInterfaceMember
            {
                Name = name,
                //FullName = property.FullName,
                Optional = values.Optional,
                Ignore = values.Ignore,
                Type = type
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
