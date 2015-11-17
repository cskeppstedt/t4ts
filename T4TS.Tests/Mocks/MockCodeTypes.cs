using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Moq;
using System.Text.RegularExpressions;

namespace T4TS.Tests.Mocks
{
    internal class MockCodeTypes : CodeElements<CodeElement>
    {
        public MockCodeTypes(params Type[] types)
        {
            foreach (Type type in types)
            {
                if (type.IsEnum)
                    Add((CodeElement) new MockCodeEnum(type).Object);
                else
                    Add((CodeElement) new MockCodeClass(type).Object);
            }
        }
    }

    internal class MockCodeClass : Mock<CodeClass>
    {
        public MockCodeClass(Type fromClass) : base(MockBehavior.Strict)
        {
            var el = As<CodeElement>();
            el.Setup(x => x.Name).Returns(fromClass.Name.Split('`')[0]);
            el.Setup(x => x.FullName).Returns(TypeMap.GetTypeFullname(fromClass.FullName));
            Setup(x => x.Attributes).Returns(new MockAttributes(fromClass.GetCustomAttributes(false).OfType<Attribute>()));
            Setup(x => x.Name).Returns(fromClass.Name.Split('`')[0]);
            Setup(x => x.FullName).Returns(fromClass.FullName);
            Setup(x => x.Members).Returns(new MockCodeProperties(fromClass));
            Setup(x => x.Access).Returns(vsCMAccess.vsCMAccessPublic);

            var baseTypes = GetBaseTypes(fromClass)
                .Select(baseType => (CodeElement) new MockCodeClass(baseType).Object)
                .ToArray();

            Setup(x => x.Bases).Returns(new CodeElements<CodeElement>(baseTypes));
        }

        private static IEnumerable<Type> GetBaseTypes(Type type)
        {
            if (type.BaseType == null)
                return type.GetInterfaces();

            return Enumerable
                .Repeat(type.BaseType, 1)
                .Concat(type.GetInterfaces())
                .Concat(type.GetInterfaces().SelectMany<Type, Type>(GetBaseTypes))
                .Concat(GetBaseTypes(type.BaseType));
        }
    }

    internal class MockCodeEnum : Mock<CodeEnum>
    {
        public MockCodeEnum(Type type) : base(MockBehavior.Strict)
        {
            As<CodeElement>();
            Setup(x => x.Attributes).Returns(new MockAttributes(type.GetCustomAttributes(false).OfType<Attribute>()));
            Setup(x => x.Name).Returns(type.Name);
            Setup(x => x.FullName).Returns(type.FullName);
            Setup(x => x.Bases).Returns(new CodeElements<CodeElement>());
            Setup(x => x.Members).Returns(new MockCodeVariables(type));
            Setup(x => x.Access).Returns(vsCMAccess.vsCMAccessPublic);
        }
    }

    internal class MockAttributes : CodeElements<CodeAttribute>
    {
        public MockAttributes(IEnumerable<Attribute> attributes)
        {
            foreach (Attribute attr in attributes)
            {
                Add(new MockCodeAttribute(attr).Object);
            }
        }
    }

    internal class MockCodeAttribute : Mock<CodeAttribute>
    {
        public MockCodeAttribute(Attribute attr) : base(MockBehavior.Strict)
        {
            Setup(x => x.Name).Returns(attr.GetType().Name);
            Setup(x => x.FullName).Returns(attr.GetType().FullName);
            Setup(x => x.Children).Returns(new MockAttributeProperties(attr));
        }
    }

    internal class MockAttributeProperties : CodeElements<CodeAttributeArgument>
    {
        public MockAttributeProperties(Attribute attr)
        {
            foreach (PropertyInfo pi in attr.GetType().GetProperties())
            {
                object value = pi.GetValue(attr);
                if (value != null)
                    Add(new MockCodeAttributeArgument(pi.Name, value).Object);
            }
        }
    }

    internal class MockCodeAttributeArgument : Mock<CodeAttributeArgument>
    {
        public MockCodeAttributeArgument(string name, object value) : base(MockBehavior.Strict)
        {
            As<CodeElement>();
            Setup(x => x.Name).Returns(name);
            Setup(x => x.Value).Returns(value.ToString());
        }
    }


    internal class MockCodeProperties : CodeElements<CodeElement>
    {
        public MockCodeProperties(Type type)
        {
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (pi.DeclaringType == type)
                Add((CodeElement) new MockCodeProperty(pi).Object);
            }

            foreach (Type subType in type.GetNestedTypes())
            {
                if (subType.IsEnum)
                    Add((CodeElement) new MockCodeEnum(subType).Object);
                else if (subType.IsClass)
                    Add((CodeElement) new MockCodeClass(subType).Object);
            }
        }
    }

    internal class MockCodeProperty : Mock<CodeProperty>
    {
        public MockCodeProperty(PropertyInfo pi) : base(MockBehavior.Strict)
        {
            As<CodeElement>();
            Setup(x => x.FullName).Returns(pi.Name);
            Setup(x => x.Name).Returns(pi.Name);
            Setup(x => x.Attributes).Returns(new MockAttributes(pi.GetCustomAttributes()));
            Setup(x => x.Access).Returns(pi.GetAccessors(false).Length == 0 ? vsCMAccess.vsCMAccessPrivate : vsCMAccess.vsCMAccessPublic);
            if (pi.CanRead)
                Setup(x => x.Getter).Returns(new PropertyGetterMock(pi).Object);
            else
                Setup(x => x.Getter).Returns((CodeFunction) null);
        }
    }

    internal class PropertyGetterMock : Mock<CodeFunction>
    {
        public PropertyGetterMock(PropertyInfo pi) : base(MockBehavior.Strict)
        {
            Setup(x => x.Name).Returns(pi.Name);
            Setup(x => x.Type).Returns(new CodeTypeRefMock(pi.PropertyType).Object);
        }
    }

    internal class FieldGetterMock : Mock<CodeFunction>
    {
        public FieldGetterMock(FieldInfo fi) : base(MockBehavior.Strict)
        {
            Setup(x => x.Name).Returns(fi.Name);
            Setup(x => x.Type).Returns(new CodeTypeRefMock(fi.FieldType).Object);
        }
    }

    internal class CodeTypeRefMock : Mock<CodeTypeRef>
    {
        public CodeTypeRefMock(Type propertyType) : base(MockBehavior.Strict)
        {
            var typeRef = TypeMap.GetTypeRef(propertyType);

            string typeFullname = TypeMap.GetTypeFullname(propertyType.FullName);

            SetupGet(x => x.TypeKind).Returns(typeRef);
            SetupGet(x => x.AsFullName).Returns(typeFullname);

            if (typeRef == vsCMTypeRef.vsCMTypeRefArray)
            {
                var elementType = new CodeTypeRefMock(propertyType.GetElementType()).Object;
                SetupGet(x => x.ElementType).Returns(elementType);
            }
        }
    }
    
    internal class MockCodeVariables : CodeElements<CodeVariable>
    {
        public MockCodeVariables(Type type)
        {
            foreach (string name in Enum.GetNames(type))
            {
                FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public);
                Add(new MockCodeVariable(fi).Object);
            }
        }
    }

    internal class MockCodeVariable : Mock<CodeVariable>
    {
        public MockCodeVariable(FieldInfo fi) : base(MockBehavior.Strict)
        {
            Setup(x => x.FullName).Returns(fi.Name);
            Setup(x => x.Name).Returns(fi.Name);
            Setup(x => x.Attributes).Returns(new MockAttributes(fi.GetCustomAttributes()));
            Setup(x => x.Access).Returns(vsCMAccess.vsCMAccessPublic);
            Setup(x => x.InitExpression).Returns(((int) fi.GetValue(null)).ToString());
        }
    }

    internal class MockCodeNamespace : Mock<CodeNamespace>
    {
        public MockCodeNamespace(IEnumerable<Type> fromClassTypes)
        {
            var namespaceName = fromClassTypes
                .Select(t => t.Namespace)
                .Distinct()
                .Single();

            var classes = fromClassTypes
                .Select(t => new MockCodeClass(t).Object)
                .ToList();

            var moqMembers = new Mock<CodeElements>();
            moqMembers.Setup(x => x.GetEnumerator()).Returns(() => classes.GetEnumerator());
            
            //codeNamespace.Setup(x => x.Members).Returns(new MockCodeTypes(types));
            Setup(x => x.Members).Returns(moqMembers.Object);
            Setup(x => x.Name).Returns(namespaceName);
        }
    }

    internal static class TypeMap
    {
        static readonly Dictionary<string, vsCMTypeRef> TypeRefMap = new Dictionary<string, vsCMTypeRef>
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

        static readonly string[] WrapTypes = new string[] {
            "System.Collections.Generic.List",
            "System.Collections.Generic.IEnumerable",
            "System.Collections.Generic.IList",
            "System.Collections.Generic.ICollection",
            "System.Collections.Generic.Dictionary",
            "System.Collections.Generic.IDictionary",
            "System.Nullable"
        };

        internal static vsCMTypeRef GetTypeRef(Type fromType)
        {
            vsCMTypeRef typeRef;
            if (TypeRefMap.TryGetValue(fromType.FullName, out typeRef))
                return typeRef;

            if (fromType.IsArray)
                return vsCMTypeRef.vsCMTypeRefArray;

            return vsCMTypeRef.vsCMTypeRefObject;
        }

        internal static string GetTypeFullname(string typeFullname)
        {
            string wrapType = WrapTypes.FirstOrDefault(typeFullname.StartsWith);

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
    }
}