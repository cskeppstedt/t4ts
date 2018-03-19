using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace T4TS.Builders
{
    public class StandardInterfaceBuilder : CodeClassInterfaceBuilder
    {
        public Settings Settings { get; private set; }
        
        private static readonly string MemberAttributeFullName = "T4TS.TypeScriptMemberAttribute";

        public StandardInterfaceBuilder(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.Settings = settings;
        }

        public TypeScriptInterface Build(
            CodeClass codeClass,
            TypescriptComponents components)
        {
            TypeScriptInterface result = null;

            InterfaceType interfaceType;
            //if (typeContext.TryGetInterfaceType(codeClass.FullName, out interfaceType)
            //    || !this.Settings.RequireInterfaceAttribute)
            //{
            //    TypeScriptInterfaceAttributeValues values = null;
            //    if (interfaceType != null)
            //    {
            //        values = interfaceType.AttributeValues;
            //    }

            //    string moduleName = null;
            //    if (values != null)
            //    {
            //        moduleName = values.Module;
            //    }

            //    if (String.IsNullOrEmpty(moduleName)
            //        && codeClass.Namespace != null)
            //    {
            //        moduleName = codeClass.Namespace.FullName;
            //    }

            //    TypeScriptModule module = components.GetModule(moduleName);
            //    if (module == null)
            //    {
            //        module = new TypeScriptModule { QualifiedName = moduleName };
            //        components.AddModule(module);
            //    }

            //    result = this.BuildInterface(
            //        codeClass,
            //        values,
            //        typeContext);
            //}
            return result;
        }
        
        private string GetInterfaceName(TypeScriptInterfaceAttributeValues attributeValues)
        {
            if (!string.IsNullOrEmpty(attributeValues.NamePrefix))
                return attributeValues.NamePrefix + attributeValues.Name;

            return attributeValues.Name;
        }

        private TypeScriptInterface BuildInterface(
            CodeClass codeClass,
            TypeScriptInterfaceAttributeValues attributeValues,
            TypeContext typeContext)
        {
            int result = 0;

            string extendsName = null;
            if (attributeValues != null)
            {
                extendsName = attributeValues.Extends;
            }

            if (String.IsNullOrEmpty(extendsName)
                && codeClass.Bases != null)
            {
                foreach (CodeClass baseClass in codeClass.Bases)
                {
                    //result += this.BuildInterface(
                    //    codeClass,
                    //    )
                }
            }
            
            var tsInterface = new TypeScriptInterface
            {
                FullName = codeClass.FullName,
                Name = (attributeValues != null)
                    ? GetInterfaceName(attributeValues)
                    : codeClass.Name,
                Extends = extendsName
            };

            TypescriptType indexedType;
            if (TryGetIndexedType(codeClass, typeContext, out indexedType))
                tsInterface.IndexedType = indexedType;

            Traversal.TraversePropertiesInClass(codeClass, (property) =>
            {
                TypeScriptInterfaceMember member;
                if (TryGetMember(property, typeContext, out member))
                    tsInterface.Members.Add(member);
            });


            module.Interfaces.Add(tsInterface);

            result++;
            return result;
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
                Optional = attributeOptional.HasValue ? attributeOptional.Value : Settings.DefaultOptional,
                Name = attributeName,
                Type = attributeType,
                CamelCase = attributeCamelCase ?? Settings.DefaultCamelCaseMemberNames,
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
