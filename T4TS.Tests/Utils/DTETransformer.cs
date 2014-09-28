using EnvDTE;
using EnvDTE80;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public static Project BuildDteProject(IEnumerable<Type> fromClassTypes, string assemblyName)
        {
            var moqProject = new Mock<Project>();
            var moqProjectItem = new Mock<ProjectItem>();
            var moqProjectItems = new Mock<ProjectItems>();

            var moqProjCodeElements = new Mock<CodeElements>();
            var moqCodeNamespace = new Mock<CodeNamespace>();
            var moqMembers = new Mock<CodeElements>();
            var moqFileCodeModel = new Mock<FileCodeModel>();

            var namespaces = new List<CodeNamespace>
            { 
                moqCodeNamespace.Object
            };

            moqFileCodeModel.SetupGet(x => x.CodeElements).Returns(moqProjCodeElements.Object);
            moqProjCodeElements.Setup(x => x.GetEnumerator()).Returns(() => namespaces.GetEnumerator());

            moqProject.SetupGet(x => x.ProjectItems).Returns(moqProjectItems.Object);

            moqProjectItem.SetupProperty(x => x.Name, assemblyName);
            moqProjectItem.SetupGet(x => x.FileCodeModel).Returns(moqFileCodeModel.Object);
            moqProjectItem.SetupGet(x => x.ProjectItems).Returns((ProjectItems)null);
            moqProjectItems.Setup(x => x.GetEnumerator()).Returns(() => new[] { moqProjectItem.Object }.GetEnumerator());

            moqCodeNamespace.SetupGet(x => x.Members).Returns(moqMembers.Object);

            var classes = fromClassTypes
                .Select(BuildDteClass)
                .ToList();

            moqMembers.Setup(x => x.GetEnumerator()).Returns(() => classes.GetEnumerator());

            return moqProject.Object;
        }

        public static CodeClass BuildDteClass(Type fromClass)
        {
            var moqMember = new Mock<CodeClass>();

            var classAttributes = BuildDteAttributes<TypeScriptInterfaceAttribute>(fromClass);
            moqMember.SetupGet(x => x.Attributes).Returns(classAttributes);

            var properties = new List<CodeProperty>(
                fromClass.GetProperties().Select(BuildDteProperty)
            );
            
            var propertiesMoq = new Mock<CodeElements>();
            propertiesMoq.Setup(x => x.GetEnumerator()).Returns(() => properties.GetEnumerator());

            moqMember.SetupGet(x => x.Name).Returns(fromClass.Name);
            moqMember.SetupGet(x => x.FullName).Returns(fromClass.FullName);
            moqMember.SetupGet(x => x.Bases).Returns((CodeElements)null);
            moqMember.SetupGet(x => x.Members).Returns(propertiesMoq.Object);

            return moqMember.Object;
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
            property.SetupProperty(x => x.Name, fromProperty.Name);

            var memberAttribute = fromProperty.GetCustomAttributes<TypeScriptMemberAttribute>().FirstOrDefault();
            var attributes = BuildDteAttributes(memberAttribute);
            property.SetupGet(x => x.Attributes).Returns(attributes);

            return property.Object;
        }


        private static CodeTypeRef GetCodeTypeRef(Type fromType)
        {
            var getterType = new Mock<CodeTypeRef>();
            getterType.SetupGet(x => x.TypeKind).Returns(GetTypeRef(fromType));

            return getterType.Object;
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
            if (!TypeMap.TryGetValue(fromType.FullName, out typeRef))
                throw new ApplicationException("No type map found for " + fromType.FullName);

            return typeRef;
        }
    }
}
