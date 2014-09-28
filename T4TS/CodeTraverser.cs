using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace T4TS
{
    public class CodeTraverser
    {
        public Solution Solution { get; private set; }
        public Settings Settings { get; private set; }

        private static readonly string InterfaceAttributeFullName = "T4TS.TypeScriptInterfaceAttribute";
        private static readonly string MemberAttributeFullName = "T4TS.TypeScriptMemberAttribute";

        public CodeTraverser(Solution solution, Settings settings)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

            if (settings == null)
                throw new ArgumentNullException("settings");

            Solution = solution;
            this.Settings = settings;
        }

        public TypeContext BuildContext()
        {
            var typeContext = new TypeContext(this.Settings);
            var partialClasses = new Dictionary<string, CodeClass>();

            new SolutionTraverser(this.Solution, (ns) =>
            {
                new NamespaceTraverser(ns, (codeClass) =>
                {
                    CodeAttribute attribute;
                    if (!TryGetAttribute(codeClass.Attributes, InterfaceAttributeFullName, out attribute))
                        return;

                    var values = GetInterfaceValues(codeClass, attribute);
                    var interfaceType = new InterfaceType(values);

                    if (!typeContext.ContainsInterfaceType(codeClass.FullName))
                        typeContext.AddInterfaceType(codeClass.FullName, interfaceType);
                });
            });

            return typeContext;
        }

        public IEnumerable<TypeScriptModule> GetAllInterfaces()
        {
            var typeContext = BuildContext();
            var byModuleName = new Dictionary<string, TypeScriptModule>();
            var tsMap = new Dictionary<CodeClass, TypeScriptInterface>();

            new SolutionTraverser(this.Solution, (ns) =>
            {
                new NamespaceTraverser(ns, (codeClass) =>
                {
                    InterfaceType interfaceType;
                    if (!typeContext.TryGetInterfaceType(codeClass.FullName, out interfaceType))
                        return;

                    var values = interfaceType.AttributeValues;
                    
                    TypeScriptModule module;
                    if (!byModuleName.TryGetValue(values.Module, out module))
                    {
                        module = new TypeScriptModule { QualifiedName = values.Module };
                        byModuleName.Add(values.Module, module);
                    }

                    var tsInterface = BuildInterface(codeClass, values, typeContext);
                    tsMap.Add(codeClass, tsInterface);
                    tsInterface.Module = module;
                    module.Interfaces.Add(tsInterface);
                });
            });

            var tsInterfaces = tsMap.Values.ToList();
            tsMap.Keys.ToList().ForEach(codeClass =>
            {
                CodeElements baseClasses = codeClass.Bases;
                if (baseClasses != null && baseClasses.Count > 0)
                {
                    CodeElement baseClass = baseClasses.Item(1);
                    if (baseClass != null)
                    {
                        var parent = tsInterfaces.SingleOrDefault(intf => intf.FullName == baseClass.FullName);
                        if (parent != null)
                        {
                            tsMap[codeClass].Parent = parent;
                        }
                    }
                }
            });

            return byModuleName.Values
                .OrderBy(m => m.QualifiedName)
                .ToList();
        }
        
        private string GetInterfaceName(TypeScriptInterfaceAttributeValues attributeValues)
        {
            if (!string.IsNullOrEmpty(attributeValues.NamePrefix))
                return attributeValues.NamePrefix + attributeValues.Name;

            return attributeValues.Name;
        }

        private TypeScriptInterface BuildInterface(CodeClass codeClass, TypeScriptInterfaceAttributeValues attributeValues, TypeContext typeContext)
        {
            var tsInterface = new TypeScriptInterface
            {
                FullName = codeClass.FullName,
                Name = GetInterfaceName(attributeValues)
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

            return tsInterface;
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

        private TypeScriptInterfaceAttributeValues GetInterfaceValues(CodeClass codeClass, CodeAttribute interfaceAttribute)
        {
            var values = GetAttributeValues(interfaceAttribute);

            return new TypeScriptInterfaceAttributeValues
            {
                Name = values.ContainsKey("Name") ? values["Name"] : codeClass.Name,
                Module = values.ContainsKey("Module") ? values["Module"] : Settings.DefaultModule ?? "T4TS",
                NamePrefix = values.ContainsKey("NamePrefix") ? values["NamePrefix"] : Settings.DefaultInterfaceNamePrefix ?? string.Empty
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
                //FullName = property.FullName,
                Optional = values.Optional,
                Ignore = values.Ignore,
                Type = (string.IsNullOrWhiteSpace(values.Type))
                    ? typeContext.GetTypeScriptType(getter.Type)
                    : new InterfaceType(values.Type)
            };

            if (member.Ignore)
            {
                return false;
            }

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
