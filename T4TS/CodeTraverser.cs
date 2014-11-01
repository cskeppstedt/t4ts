using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace T4TS
{
    public class CodeTraverser
    {
        public Solution Solution { get; private set; }
        public static Settings Settings { get; private set; }

        private const string InterfaceAttributeFullName = "T4TS.TypeScriptInterfaceAttribute";
        private const string MemberAttributeFullName = "T4TS.TypeScriptMemberAttribute";
        private const string EnumAttributeFullName = "T4TS.TypeScriptEnumAttribute";

        public CodeTraverser(Solution solution, Settings settings)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

            if (settings == null)
                throw new ArgumentNullException("settings");

            Solution = solution;
            Settings = settings;
        }


        private void BuildCodeClass(TypeContext typeContext, CodeClass codeClass, CodeClass owner = null)
        {
            if (codeClass == null) return;
            CodeAttribute attribute;
            InterfaceType interfaceType = null;
            if (owner != null)
            {
                var tsType = typeContext.GetTypeScriptType(owner.FullName);
                if (tsType != null)
                    interfaceType = new InterfaceType(codeClass.Name);
            }
            if (TryGetAttribute(codeClass.Attributes, InterfaceAttributeFullName, out attribute))
            {
                var values = GetInterfaceValues(codeClass, attribute);
                interfaceType = new InterfaceType(values);
            }
            if (interfaceType != null)
            {
                if (!typeContext.ContainsInterfaceType(codeClass.FullName))
                    typeContext.AddInterfaceType(codeClass.FullName, interfaceType);
                foreach (var subCodeClass in codeClass.Members)
                {
                    BuildCodeClass(typeContext, subCodeClass as CodeClass, codeClass);
                    BuildCodeEnum(typeContext, subCodeClass as CodeEnum, codeClass);
                }
            }
        }
        private void BuildCodeEnum(TypeContext typeContext, CodeEnum codeEnum, CodeClass owner = null)
        {
            if (codeEnum == null) return;
            CodeAttribute attribute;
            EnumType enumType = null;
            if (owner != null)
            {
                var tsType = typeContext.GetTypeScriptType(owner.FullName);
                if (tsType != null)
                    enumType = new EnumType(codeEnum.Name);
            }
            if (TryGetAttribute(codeEnum.Attributes, EnumAttributeFullName, out attribute))
            {
                var values = GetEnumValues(codeEnum, attribute);
                enumType = new EnumType(values);
            }
            if (enumType != null)
            {
                if (!typeContext.ContainsEnumType(codeEnum.FullName))
                    typeContext.AddEnumType(codeEnum.FullName, enumType);
            }
        }

        public TypeContext BuildContext()
        {
            var typeContext = new TypeContext(Settings);

            new SolutionTraverser(Solution, ns => 
            {
                new NamespaceTraverser(ns,
                    codeClass => BuildCodeClass(typeContext, codeClass),
                    codeEnum => BuildCodeEnum(typeContext, codeEnum)
                );
            });

            return typeContext;
        }

        private void ProcessCodeClass(TypeContext typeContext, IDictionary<CodeClass, TypeScriptInterface> tsMap, 
            IDictionary<string, TypeScriptModule> byModuleName, CodeClass codeClass)
        {
            InterfaceType interfaceType;
            if (typeContext.TryGetInterfaceType(codeClass.FullName, out interfaceType))
            {
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
            }
        }

        private void ProcessCodeEnum(TypeContext typeContext, IDictionary<CodeEnum, TypeScriptEnum> tsEnumMap,
            IDictionary<string, TypeScriptModule> byModuleName, CodeEnum codeEnum)
        {
            EnumType enumType;
            if (typeContext.TryGetEnumType(codeEnum.FullName, out enumType))
            {
                var values = enumType.AttributeValues;

                TypeScriptModule module;
                if (!byModuleName.TryGetValue(values.Module, out module))
                {
                    module = new TypeScriptModule { QualifiedName = values.Module };
                    byModuleName.Add(values.Module, module);
                }

                var tsEnum = BuildEnum(codeEnum, values, typeContext);
                tsEnumMap.Add(codeEnum, tsEnum);
                tsEnum.Module = module;
                module.Enums.Add(tsEnum);
            }
        }

        public IEnumerable<TypeScriptModule> GetAllInterfaces()
        {
            var typeContext = BuildContext();
            var byModuleName = new Dictionary<string, TypeScriptModule>();
            var tsMap = new Dictionary<CodeClass, TypeScriptInterface>();
            var tsEnumMap = new Dictionary<CodeEnum, TypeScriptEnum>();

            new SolutionTraverser(Solution, ns =>
            {
                new NamespaceTraverser(ns, 
                    codeClass => ProcessCodeClass(typeContext, tsMap, byModuleName, codeClass),
                    codeEnum => ProcessCodeEnum(typeContext, tsEnumMap, byModuleName, codeEnum)
                );
            });

            var tsInterfaces = tsMap.Values.ToList();
            tsMap.Keys.ToList().ForEach(codeClass =>
            {
                var baseClasses = codeClass.Bases;
                if (baseClasses.Count > 0)
                {
                    var baseClass = baseClasses.Item(1);
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
        private string GetEnumName(TypeScriptEnumAttributeValues attributeValues)
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

            // Add sub-classes to the interface
            foreach (var codeSubClass in codeClass.Members.OfType<CodeClass>())
            {
                var subAttributeValues = new TypeScriptInterfaceAttributeValues { Name = codeSubClass.Name };
                InterfaceType interfaceType;
                if (typeContext.TryGetInterfaceType(codeSubClass.FullName, out interfaceType))
                {
                    subAttributeValues = interfaceType.AttributeValues;
                    subAttributeValues.Module = attributeValues.Module + "." + tsInterface.Name;
                }
                    
                var subInterface = BuildInterface(codeSubClass, subAttributeValues, typeContext);
                subInterface.Owner = tsInterface;
                tsInterface.SubClasses.Add(subInterface);
            }

            // Add sub-enums to the interface
            foreach (var codeSubEnum in codeClass.Members.OfType<CodeEnum>())
            {
                var subAttributeValues = new TypeScriptEnumAttributeValues { Name = codeSubEnum.Name };
                EnumType enumType;
                if (typeContext.TryGetEnumType(codeSubEnum.FullName, out enumType))
                {
                    subAttributeValues = enumType.AttributeValues;
                    subAttributeValues.Module = attributeValues.Module + "." + tsInterface.Name;
                }
                    
                var subEnum = BuildEnum(codeSubEnum, subAttributeValues, typeContext);
                subEnum.Owner = tsInterface;
                tsInterface.SubEnums.Add(subEnum);
            }

            TypescriptType indexedType;
            if (TryGetIndexedType(codeClass, typeContext, out indexedType))
                tsInterface.IndexedType = indexedType;

            new ClassTraverser(codeClass, property =>
            {
                TypeScriptInterfaceMember member;
                if (TryGetMember(property, typeContext, out member))
                    tsInterface.Members.Add(member);
            });


            return tsInterface;
        }
        private TypeScriptEnum BuildEnum(CodeEnum codeEnum, TypeScriptEnumAttributeValues attributeValues, TypeContext typeContext)
        {
            var tsEnum = new TypeScriptEnum
            {
                FullName = codeEnum.FullName,
                Name = GetEnumName(attributeValues)
            };

            new EnumTraverser(codeEnum, (variable, index) =>
            {
                TypeScriptEnumMember member;
                if (TryGetEnumMember(variable, typeContext, index, out member))
                    tsEnum.Members.Add(member);
            });

            return tsEnum;
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
                if (TypeContext.IsGenericEnumerable(baseClass.FullName))
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
        private TypeScriptEnumAttributeValues GetEnumValues(CodeEnum codeEnum, CodeAttribute interfaceAttribute)
        {
            var values = GetAttributeValues(interfaceAttribute);

            return new TypeScriptEnumAttributeValues
            {
                Name = values.ContainsKey("Name") ? values["Name"] : codeEnum.Name,
                Module = values.ContainsKey("Module") ? values["Module"] : Settings.DefaultModule ?? "T4TS",
                NamePrefix = values.ContainsKey("NamePrefix") ? values["NamePrefix"] : Settings.DefaultEnumNamePrefix ?? string.Empty
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

        private bool TryGetEnumMember(CodeVariable variable, TypeContext typeContext, int index, out TypeScriptEnumMember member)
        {
            member = null;
            if (variable.Access != vsCMAccess.vsCMAccessPublic)
                return false;

            var values = GetMemberValues(variable, typeContext);
            member = new TypeScriptEnumMember
            {
                Name = values.Name ?? variable.Name,
                FullName = variable.FullName,
                Ignore = values.Ignore,
                Value = variable.InitExpression == null ? index : Int32.Parse(variable.InitExpression.ToString()),
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
                if (values.ContainsKey("Optional"))
                    attributeOptional = values["Optional"] == "true";

                if (values.ContainsKey("CamelCase"))
                    attributeCamelCase = values["CamelCase"] == "true";

                if (values.ContainsKey("Ignore"))
                    attributeIgnore = values["Ignore"] == "true";

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

        private TypeScriptMemberAttributeValues GetMemberValues(CodeVariable variable, TypeContext typeContext)
        {
            bool? attributeOptional = null;
            bool? attributeCamelCase = null;
            bool attributeIgnore = false;
            string attributeName = null;
            string attributeType = null;

            CodeAttribute attribute;
            if (TryGetAttribute(variable.Attributes, MemberAttributeFullName, out attribute))
            {
                var values = GetAttributeValues(attribute);
                if (values.ContainsKey("Optional"))
                    attributeOptional = values["Optional"] == "true";

                if (values.ContainsKey("CamelCase"))
                    attributeCamelCase = values["CamelCase"] == "true";

                if (values.ContainsKey("Ignore"))
                    attributeIgnore = values["Ignore"] == "true";

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
