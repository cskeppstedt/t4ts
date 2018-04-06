using EnvDTE;
using EnvDTE80;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace T4TS.Tests.Utils
{
    public class DTETransformer
    {
        public static Solution BuildDteSolution(params Type[] fromClassTypes)
        {
            var moqDte = new Mock<DTE>();
            var moqSolution = new Mock<Solution>();
            
            var byAssembly = fromClassTypes
                .GroupBy(x => x.Assembly.FullName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.ToList()
                );

            var projects = new List<Project>();
            foreach (string assemblyName in byAssembly.Keys)
                projects.Add(BuildDteProject(byAssembly[assemblyName], assemblyName));
            
            var moqProjects = new Mock<Projects>();
            moqProjects.Setup(x => x.GetEnumerator()).Returns(() => projects.GetEnumerator());

            moqSolution.SetupGet(x => x.Projects).Returns(moqProjects.Object);
            moqDte.SetupGet(x => x.Solution).Returns(moqSolution.Object);
            
            return moqSolution.Object;
        }

        public static Project BuildDteProject(IEnumerable<Type> fromClassTypes, string projectName, ProjectItems subProjectItems = null)
        {
            var moqProject = new Mock<Project>();
            var moqProjectItems = new Mock<ProjectItems>();

            moqProject.SetupGet(x => x.ProjectItems).Returns(moqProjectItems.Object);

            var byNamespace = fromClassTypes
                .GroupBy(x => x.Namespace)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );

            var projectItems = new List<ProjectItem>();

            foreach (string namespaceName in byNamespace.Keys)
            {
                var projectItem = BuildDteProjectItem(byNamespace[namespaceName], projectName, subProjectItems);
                projectItems.Add(projectItem);
            }

            moqProjectItems.Setup(x => x.GetEnumerator()).Returns(() => projectItems.GetEnumerator());
            
            return moqProject.Object;
        }

        public static ProjectItem BuildDteProjectItem(
            IEnumerable<Type> fromTypes,
            string projectItemName,
            ProjectItems subProjectItems = null)
        {
            var namespaceName = fromTypes
                .Select(t => t.Namespace)
                .Distinct()
                .Single();

            var mockedTypes = fromTypes
                .Select(BuildDteObject)
                .ToList();

            var moqFileCodeModel = new Mock<FileCodeModel>();
            var moqProjectItem = new Mock<ProjectItem>();
            var moqProjCodeElements = new Mock<CodeElements>();

            var moqMembers = new Mock<CodeElements>();
            moqMembers.Setup(x => x.GetEnumerator()).Returns(() => mockedTypes.GetEnumerator());

            var moqCodeNamespace = new Mock<CodeNamespace>();
            moqCodeNamespace.SetupGet(x => x.Members).Returns(moqMembers.Object);
            moqCodeNamespace.SetupGet(x => x.Name).Returns(namespaceName);
            moqCodeNamespace.SetupGet(x => x.FullName).Returns(namespaceName);

            moqProjCodeElements.Setup(x => x.GetEnumerator()).Returns(() => new[] { moqCodeNamespace.Object }.GetEnumerator());
            moqFileCodeModel.SetupGet(x => x.CodeElements).Returns(moqProjCodeElements.Object);
            moqProjectItem.SetupProperty(x => x.Name, projectItemName);
            moqProjectItem.SetupGet(x => x.FileCodeModel).Returns(moqFileCodeModel.Object);
            moqProjectItem.SetupGet(x => x.ProjectItems).Returns(subProjectItems);

            return moqProjectItem.Object;
        }

        public static object BuildDteObject(Type fromType)
        {
            object result = null;
            if (fromType.IsClass)
            {
                result = DTETransformer.BuildDteClass(fromType);
            }
            else if (fromType.IsInterface)
            {
                result = DTETransformer.BuildDteInterface(fromType);
            }
            else if (fromType.IsEnum)
            {
                result = DTETransformer.BuildDteEnum(fromType);
            }
            return result;
        }

        public static CodeClass BuildDteClass(Type fromClass)
        {
            var moqMember = new Mock<CodeClass>();

            var classAttributes = BuildDteAttributes<TypeScriptInterfaceAttribute>(fromClass);
            moqMember.SetupGet(x => x.Attributes).Returns(classAttributes);

            var properties = new List<CodeProperty>(
                fromClass
                    .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                    .Select(BuildDteProperty)
            );
            
            var propertiesMoq = new Mock<CodeElements>();
            propertiesMoq.Setup(x => x.GetEnumerator()).Returns(() => properties.GetEnumerator());

            var bases = GetBaseTypes(fromClass)
                .Select(BuildDteBase)
                .ToList();

            var basesMoq = new Mock<CodeElements>();
            basesMoq.Setup(x => x.GetEnumerator()).Returns(() => bases.GetEnumerator());
            basesMoq.Setup(x => x.Count).Returns(() => bases.Count);
            basesMoq.Setup(x => x.Item(It.IsAny<int>()))
                .Returns((int i) => bases.ElementAtOrDefault(i - 1)); // Item() accessor is not zero-based

            var moqCodeNamespace = new Mock<CodeNamespace>();
            moqCodeNamespace.SetupGet(x => x.FullName).Returns(fromClass.Namespace);
            
            moqMember.SetupGet(x => x.Name).Returns(DTETransformer.GenerateNameFromType(fromClass));
            moqMember.SetupGet(x => x.FullName).Returns(DTETransformer.GenerateFullNameFromType(fromClass));
            moqMember.SetupGet(x => x.Bases).Returns(basesMoq.Object);
            moqMember.SetupGet(x => x.Members).Returns(propertiesMoq.Object);
            moqMember.SetupGet(x => x.Namespace).Returns(moqCodeNamespace.Object);

            return moqMember.Object;
        }

        public static CodeInterface BuildDteInterface(Type fromClass)
        {
            var moqMember = new Mock<CodeInterface>();

            var classAttributes = BuildDteAttributes<TypeScriptInterfaceAttribute>(fromClass);
            moqMember.SetupGet(x => x.Attributes).Returns(classAttributes);

            var properties = new List<CodeProperty>(
                fromClass
                    .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                    .Select(BuildDteProperty)
            );

            var propertiesMoq = new Mock<CodeElements>();
            propertiesMoq.Setup(x => x.GetEnumerator()).Returns(() => properties.GetEnumerator());

            var bases = GetBaseTypes(fromClass)
                .Select(BuildDteBase)
                .ToList();

            var basesMoq = new Mock<CodeElements>();
            basesMoq.Setup(x => x.GetEnumerator()).Returns(() => bases.GetEnumerator());
            basesMoq.Setup(x => x.Count).Returns(() => bases.Count);
            basesMoq.Setup(x => x.Item(It.IsAny<int>()))
                .Returns((int i) => bases.ElementAtOrDefault(i - 1)); // Item() accessor is not zero-based

            var moqCodeNamespace = new Mock<CodeNamespace>();
            moqCodeNamespace.SetupGet(x => x.FullName).Returns(fromClass.Namespace);

            moqMember.SetupGet(x => x.Name).Returns(DTETransformer.GenerateNameFromType(fromClass));
            moqMember.SetupGet(x => x.FullName).Returns(DTETransformer.GenerateFullNameFromType(fromClass));
            moqMember.SetupGet(x => x.Bases).Returns(basesMoq.Object);
            moqMember.SetupGet(x => x.Members).Returns(propertiesMoq.Object);
            moqMember.SetupGet(x => x.Namespace).Returns(moqCodeNamespace.Object);

            return moqMember.Object;
        }

        public static CodeEnum BuildDteEnum(Type fromType)
        {
            var moqMember = new Mock<CodeEnum>();

            var classAttributes = BuildDteAttributes<TypeScriptInterfaceAttribute>(fromType);
            moqMember.SetupGet(x => x.Attributes).Returns(classAttributes);

            var properties = new List<CodeProperty>(
                fromType
                    .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                    .Select(BuildDteProperty)
            );

            var propertiesMoq = new Mock<CodeElements>();
            propertiesMoq.Setup(x => x.GetEnumerator()).Returns(() => properties.GetEnumerator());

            var values = Enum.GetValues(fromType)
                .Cast<int>()
                .Select(
                    (enumValue) =>
                    {
                        var valueMoq = new Mock<CodeVariable>();
                        valueMoq.SetupGet(x => x.Name).Returns(Enum.GetName(fromType, enumValue));
                        valueMoq.SetupGet(x => x.InitExpression).Returns(enumValue);
                        return valueMoq.Object;
                    })
                .ToList();

            var valuesMoq = new Mock<CodeElements>();
            valuesMoq.Setup(x => x.GetEnumerator()).Returns(() => values.GetEnumerator());
            valuesMoq.Setup(x => x.Count).Returns(() => values.Count);
            //valuesMoq.Setup(x => x.Item(It.IsAny<int>()))
            //    .Returns((int i) => values.ElementAtOrDefault(i - 1)); // Item() accessor is not zero-based

            var moqCodeNamespace = new Mock<CodeNamespace>();
            moqCodeNamespace.SetupGet(x => x.FullName).Returns(fromType.Namespace);

            moqMember.SetupGet(x => x.Name).Returns(fromType.Name);
            moqMember.SetupGet(x => x.FullName).Returns(fromType.FullName);
            moqMember.SetupGet(x => x.Members).Returns(valuesMoq.Object);
            moqMember.SetupGet(x => x.Namespace).Returns(moqCodeNamespace.Object);

            return moqMember.Object;
        }

        private static CodeElement BuildDteBase(Type withType)
        {
            var moqBase = new Mock<CodeElement>();
            string baseTypeFullname = DTETransformer.GenerateFullNameFromType(withType);

            moqBase.SetupGet(x => x.FullName).Returns(baseTypeFullname);
            return moqBase.Object;
        }

        private static CodeElements BuildDteAttributes<T>(T fromAttribute) where T : Attribute
        {
            var attributes = new List<CodeAttribute>();
            var attributeType = typeof(T);
            
            if (fromAttribute != null)
            {
                var moqAttribute = new Mock<CodeAttribute>();
                var moqAttributeChildren = new Mock<CodeElements>();
                var moqAttributeChild = new Mock<CodeAttributeArgument>();

                moqAttribute.SetupGet(x => x.FullName).Returns(attributeType.FullName);

                var interfaceOptions = new List<CodeElement>();

                foreach (var prop in fromAttribute.GetType().GetProperties())
                {
                    var val = prop.GetValue(fromAttribute);
                    if (val == null)
                        continue;

                    var moqProp = new Mock<CodeAttributeArgument>();
                    moqProp.SetupGet(x => x.Name).Returns(prop.Name);
                    moqProp.SetupGet(x => x.Value).Returns(val.ToString());
                    interfaceOptions.Add(moqProp.As<CodeElement>().Object);
                }

                moqAttributeChildren.Setup(x => x.GetEnumerator()).Returns(() => interfaceOptions.GetEnumerator());
                moqAttribute.SetupGet(x => x.Children).Returns(moqAttributeChildren.Object);

                attributes.Add(moqAttribute.Object);
            }

            var moqAttributes = new Mock<CodeElements>();
            moqAttributes.Setup(x => x.GetEnumerator()).Returns(() => attributes.GetEnumerator());

            return moqAttributes.Object;
        }

        private static CodeElements BuildDteAttributes<T>(Type fromType) where T : Attribute
        {
            var targetAttribute = fromType.GetCustomAttributes(
                typeof(T),
                inherit: true
            ).FirstOrDefault() as T;

            return BuildDteAttributes<T>(targetAttribute);
        }

        private static CodeProperty BuildDteProperty(PropertyInfo fromProperty)
        {
            var property = new Mock<CodeProperty>();
            var getter = new Mock<CodeFunction>();
            var getterType = GetCodeTypeRef(fromProperty.PropertyType);

            getter.SetupProperty(x => x.Type, getterType);
            property.SetupProperty(x => x.Access, vsCMAccess.vsCMAccessPublic);
            property.SetupProperty(x => x.Getter, getter.Object);

            
            // Super ugly hack. We want the property name to start with a @,
            // because that's how the DTE version is rendered. However, it looks
            // like the @ is stripped when we acquire it through reflection.
            string name = fromProperty.Name;
            if (PropertyNameIsReserved(name))
                name = "@" + name;
            
            property.SetupProperty(x => x.Name, name);

            var memberAttribute = fromProperty.GetCustomAttributes<TypeScriptMemberAttribute>().FirstOrDefault();
            var attributes = BuildDteAttributes(memberAttribute);
            property.SetupGet(x => x.Attributes).Returns(attributes);

            return property.Object;
        }

        private static bool PropertyNameIsReserved(string name)
        {
            switch (name)
            {
                case "public": return true;
                case "readonly": return true;
                case "class": return true;
                default: return false;
            }
        }

        private static CodeTypeRef GetCodeTypeRef(Type fromType)
        {
            var getterType = new Mock<CodeTypeRef>();
            var typeRef = GetTypeRef(fromType);
            
            string typeFullname = GetTypeFullname(fromType.FullName ?? fromType.Name);

            getterType.SetupGet(x => x.TypeKind).Returns(typeRef);
            getterType.SetupGet(x => x.AsFullName).Returns(typeFullname);

            if (typeRef == vsCMTypeRef.vsCMTypeRefArray)
            {
                var elementType = GetCodeTypeRef(fromType.GetElementType());
                getterType.SetupGet(x => x.ElementType).Returns(elementType);
            }
            return getterType.Object;
        }

        static readonly string[] wrapTypes = new string[] {
            "System.Collections.Generic.List",
            "System.Collections.Generic.IEnumerable",
            "System.Collections.Generic.IList",
            "System.Collections.Generic.ICollection",
            "System.Collections.Generic.Dictionary",
            "System.Collections.Generic.IDictionary",
            "System.Nullable"
        };


        private static string GetTypeFullname(string typeFullname)
        {
            string wrapType = wrapTypes.FirstOrDefault(typeFullname.StartsWith);
            
            if (string.IsNullOrEmpty(wrapType))
            {
                return Regex.Match(typeFullname, "[^,]+").Value;
            }
            else if (wrapType == "System.Collections.Generic.Dictionary" || wrapType == "System.Collections.Generic.IDictionary")
            {
                var match = Regex.Match(typeFullname, @"\[\[([^\]]+)\],\[([^\]]+)\]\]");
                string keyType = match.Groups[1].Value;
                string valType = match.Groups[2].Value;

                return string.Format("{0}<{1},{2}>", wrapType, GetTypeFullname(keyType), GetTypeFullname(valType));
            }
            else
            {
                var match = Regex.Match(typeFullname, @"\[\[(.*)\]\]");
                string elementType = match.Groups[1].Value;

                return string.Format("{0}<{1}>", wrapType, GetTypeFullname(elementType));
            }
        }

        private static readonly Dictionary<string, vsCMTypeRef> TypeMap = new Dictionary<string, vsCMTypeRef>
        {
            { typeof(Int32).FullName, vsCMTypeRef.vsCMTypeRefInt },
            { typeof(Int64).FullName, vsCMTypeRef.vsCMTypeRefLong },
            { typeof(Char).FullName, vsCMTypeRef.vsCMTypeRefChar },
            { typeof(String).FullName, vsCMTypeRef.vsCMTypeRefString },
            { typeof(Boolean).FullName, vsCMTypeRef.vsCMTypeRefBool },
            { typeof(Byte).FullName, vsCMTypeRef.vsCMTypeRefByte },
            { typeof(Double).FullName, vsCMTypeRef.vsCMTypeRefDouble },
            { typeof(Int16).FullName, vsCMTypeRef.vsCMTypeRefShort },
            { typeof(Single).FullName, vsCMTypeRef.vsCMTypeRefFloat },
            { typeof(Decimal).FullName, vsCMTypeRef.vsCMTypeRefDecimal }
        };

        private static vsCMTypeRef GetTypeRef(Type fromType)
        {
            vsCMTypeRef typeRef;
            if (!String.IsNullOrEmpty(fromType.FullName)
                && TypeMap.TryGetValue(fromType.FullName, out typeRef))
            {
                return typeRef;
            }

            if (fromType.IsArray)
                return vsCMTypeRef.vsCMTypeRefArray;

            return vsCMTypeRef.vsCMTypeRefObject;
        }

        private static IEnumerable<Type> GetBaseTypes(Type type)
        {
            IEnumerable<Type> result;
            if (type.BaseType == null
                || type.BaseType == typeof(object))
            {
                result = type.GetInterfaces();
            }
            else
            {
                result = Enumerable
                    .Repeat(type.BaseType, 1)
                    .Concat(type.GetInterfaces())
                    .Concat(type.GetInterfaces().SelectMany<Type, Type>(GetBaseTypes))
                    .Concat(GetBaseTypes(type.BaseType));
            }
            return result;
        }

        private static string GenerateNameFromType(Type type)
        {
            TypeFullName nameWithoutArguments = TypeFullNameParser.Parse(type.Name);
            return nameWithoutArguments.FullName;
        }

        private static string GenerateFullNameFromType(Type type)
        {
            TypeFullName nameWithoutArguments = TypeFullNameParser.Parse(type.FullName);
            TypeFullName result = new TypeFullName(
                nameWithoutArguments.FullName,
                type.GetGenericArguments()
                    .Select((genericType) => new TypeFullName(genericType.FullName ?? genericType.Name))
                    .ToArray());
            return result.ToString();
        }
    }
}
