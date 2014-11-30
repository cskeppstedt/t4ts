using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Moq;

namespace T4TS.Tests
{
    internal class MockCodeTypes : CodeElemens<CodeElement>
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
        public MockCodeClass(Type type) : base(MockBehavior.Strict)
        {
            As<CodeElement>();
            Setup(x => x.Attributes).Returns(new MockAttributes(type.GetCustomAttributes(false).OfType<Attribute>()));
            Setup(x => x.Name).Returns(type.Name);
            Setup(x => x.FullName).Returns(type.FullName);
            Setup(x => x.Bases).Returns(new CodeElemens<CodeElement>());
            Setup(x => x.Members).Returns(new MockCodeProperties(type));
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
            Setup(x => x.Bases).Returns(new CodeElemens<CodeElement>());
            Setup(x => x.Members).Returns(new MockCodeVariables(type));
        }
    }

    internal class MockAttributes : CodeElemens<CodeAttribute>
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

    internal class MockAttributeProperties : CodeElemens<CodeAttributeArgument>
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


    internal class MockCodeProperties : CodeElemens<CodeElement>
    {
        public MockCodeProperties(Type type)
        {
            foreach (PropertyInfo pi in type.GetProperties())
            {
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
            Setup(x => x.Access).Returns(vsCMAccess.vsCMAccessPublic);
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
            string fullName = propertyType.FullName;
            if (fullName.Contains('`'))
            {
                fullName = fullName.Split('`')[0] + '<' + propertyType.GetGenericArguments().Single().FullName + '>';
            }

            Setup(x => x.AsFullName).Returns(fullName);

            if (propertyType.IsArray)
            {
                Setup(x => x.TypeKind).Returns(vsCMTypeRef.vsCMTypeRefArray);
                Setup(x => x.ElementType).Returns(new CodeTypeRefMock(propertyType.GetElementType()).Object);
            }
            else if (propertyType == typeof (string))
            {
                Setup(x => x.TypeKind).Returns(vsCMTypeRef.vsCMTypeRefString);
            }
            else if (propertyType == typeof (char))
            {
                Setup(x => x.TypeKind).Returns(vsCMTypeRef.vsCMTypeRefChar);
            }
            else if (propertyType == typeof (bool))
            {
                Setup(x => x.TypeKind).Returns(vsCMTypeRef.vsCMTypeRefBool);
            }
            else if (propertyType == typeof (int))
            {
                Setup(x => x.TypeKind).Returns(vsCMTypeRef.vsCMTypeRefInt);
            }
            else
            {
                Setup(x => x.TypeKind).Returns(vsCMTypeRef.vsCMTypeRefObject);
            }
        }
    }


    internal class MockCodeVariables : CodeElemens<CodeVariable>
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
}