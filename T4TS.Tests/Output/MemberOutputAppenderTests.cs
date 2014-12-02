using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace T4TS.Tests
{
    [TestClass]
    public class MemberOutputAppenderTests
    {
        private void AssertOutputs(string expected, string actual)
        {
            Assert.AreEqual(expected.Replace("\r\n", "\n").Trim(), actual.Replace("\r\n", "\n").Trim());
        }

        [TestMethod]
        public void MemberOutputAppenderRespectsCompatibilityVersion()
        {
            var sb = new StringBuilder();

            var member = new TypeScriptInterfaceMember
            {
                Name = "Foo",
                FullName = "Foo",
                Type = new BoolType()
            };

            var settings = new Settings();
            var appender = new MemberOutputAppender(sb, 0, settings);

            settings.CompatibilityVersion = new Version(0, 8, 3);
            appender.AppendOutput(member);
            Assert.IsTrue(sb.ToString().Contains("bool"));
            Assert.IsFalse(sb.ToString().Contains("boolean"));
            Console.WriteLine(sb.ToString());
            sb.Clear();

            settings.CompatibilityVersion = new Version(0, 9, 0);
            appender.AppendOutput(member);
            Assert.IsTrue(sb.ToString().Contains("boolean"));
            Console.WriteLine(sb.ToString());
            sb.Clear();

            settings.CompatibilityVersion = null;
            appender.AppendOutput(member);
            Assert.IsTrue(sb.ToString().Contains("boolean"));
            Console.WriteLine(sb.ToString());
        }

        [TestMethod]
        public void MemberOutputAppenderTestEnumType()
        {
            var sb = new StringBuilder();

            var member = new TypeScriptInterfaceMember
            {
                Name = "Foo",
                FullName = "Foo",
                Type = new EnumType("FooEnum")
            };

            var settings = new Settings();
            var appender = new MemberOutputAppender(sb, 0, settings);

            appender.AppendOutput(member);
            Console.WriteLine(sb.ToString());
            Assert.IsTrue(sb.ToString().Contains("FooEnum"));
        }


        [TestMethod]
        public void TestOutput()
        {
            var settings = new Settings { UseNativeDates = true };
            string res = OutputFormatter.GetOutput(GetDataToRender(settings), settings);
            Console.WriteLine(res);
            AssertOutputs(ExpectedResult, res);
        }

        [TestMethod]
        public void TestOutputSubClasses()
        {
            var settings = new Settings { UseNativeDates = true };
            string res = OutputFormatter.GetOutput(GetDataToRenderSubClasses(settings), settings);
            Console.WriteLine(res);
            AssertOutputs(ExpectedSubClassesResult, res);
        }

        [TestMethod]
        public void TestOutputDataContract()
        {
            var settings = new Settings { UseNativeDates = true };

            // Test behaviour by default - no process DataContract classes 
            var res = OutputFormatter.GetOutput(GetDataToRenderDataContract(settings), settings);
            Console.WriteLine(res);
            AssertOutputs(OutputHeader, res);

            // Test extended behaviour - process DataContract classes 
            settings.ProcessDataContracts = true;
            res = OutputFormatter.GetOutput(GetDataToRenderDataContract(settings), settings);
            Console.WriteLine(res);
            AssertOutputs(ExpectedDataContractResult, res);

            // Test extended behaviour - process DataContract classes with XmlIgnoreAttribute
            settings.MemberIgnoreAttributes = new[] { "XmlIgnoreAttribute" };
            res = OutputFormatter.GetOutput(GetDataToRenderDataContract(settings), settings);
            Console.WriteLine(res);
            AssertOutputs(ExpectedDataContractWithXmlIgnoredResult, res);

            // Test extended behaviour - process DataContract classes with XmlIgnoreAttribute and JsonIgnoreAttribute
            settings.MemberIgnoreAttributes = new[] { "XmlIgnoreAttribute", "JsonIgnoreAttribute" };
            res = OutputFormatter.GetOutput(GetDataToRenderDataContract(settings), settings);
            Console.WriteLine(res);
            AssertOutputs(ExpectedDataContractWithXmlIgnoredAndJsonIgnoredResult, res);
        }

        [TestMethod]
        public void TestOutputDataInheritance()
        {
            var settings = new Settings { UseNativeDates = true };
            var res = OutputFormatter.GetOutput(GetDataToRenderDataInheritance(settings), settings);
            AssertOutputs(ExpectedDataNoInheritanceResult, res);
            settings.ProcessParentClasses = true;
            res = OutputFormatter.GetOutput(GetDataToRenderDataInheritance(settings), settings);
            Console.WriteLine(res);
            AssertOutputs(ExpectedDataInheritanceResult, res);
        }


        #region - Test Data -

        private List<TypeScriptModule> GetDataToRender(Settings settings)
        {
            var generator = new CodeTraverser(
                new MockSolution(
                    typeof(TestClass),
                    typeof(TestEnum)
                    ).Object, settings);
            return generator.GetAllInterfaces().ToList();
        }

        private const string OutputHeader =
            @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/
";

        private const string ExpectedResult = OutputHeader + @"
declare module External1 {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestClass **/
    export interface TestClass {
        Id: number;
        Name: string;
        Date: Date;
        DatesList: Date[];
        DatesArray: Date[];
        RefObject: any;
        IntArray: number[];
        SelfArray: External1.TestClass[];
        IsVisible: boolean;
        IsOptional?: boolean;
        IntOptional?: number;
        Self: External1.TestClass;
        EnumProp: External2.TestEnum;
        EnumPropNull?: External2.TestEnum;
        EnumArray: External2.TestEnum[];
    }
}

declare module External2 {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestEnum **/
    enum TestEnum {
        TheItem1 = 1,
        Item2 = 2,
        Item21 = 3,
        Item22 = 4,
        Item23 = 5,
        Item3 = 5,
        Item4 = 6,
    }
}
";
        [TypeScriptEnum(Module = "External2")]
        public enum TestEnum
        {
            [TypeScriptMember(Name = "TheItem1")]
            Item1 = 1,
            Item2 = 2,
            Item21,
            Item22,
            Item23,
            Item3 = 5,
            Item4,
        }

        [TypeScriptInterface(Module = "External1")]

        public class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public DateTime Date { get; set; }

            public List<DateTime> DatesList { get; set; }
            public DateTime[] DatesArray { get; set; }

            public WeakReference RefObject { get; set; }

            public int[] IntArray { get; set; }

            public TestClass[] SelfArray { get; set; }

            public bool IsVisible { get; set; }
            public bool? IsOptional { get; set; }
            public int? IntOptional { get; set; }

            public TestClass Self { get; set; }
            public TestEnum EnumProp { get; set; }
            public TestEnum? EnumPropNull { get; set; }

            public TestEnum[] EnumArray { get; set; }
        }

        #endregion

        #region - Test Sub Classes -

        private List<TypeScriptModule> GetDataToRenderSubClasses(Settings settings)
        {
            var generator = new CodeTraverser(
                new MockSolution(
                    typeof(TestClass1)
                    ).Object, settings);
            return generator.GetAllInterfaces().ToList();
        }



        private const string ExpectedSubClassesResult = OutputHeader + @"
declare module External1 {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestClass1+SubEnum **/
    module TestClass1 {
        enum SubEnum {
            TheItem1 = 1,
            Item2 = 2,
            Item21 = 3,
            Item22 = 4,
            Item23 = 5,
            Item3 = 5,
            Item4 = 6,
        }
    }
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestClass1+SubClass **/
    module TestClass1 {
        export interface TestSubClass {
            Id: number;
            Name: string;
        }
    }
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestClass1 **/
    export interface TestClass1 {
        Id: number;
        Name: string;
        SubCls: External1.TestClass1.TestSubClass;
        SubEnm: External1.TestClass1.SubEnum;
    }
}
";

        [TypeScriptInterface(Module = "External1")]
        public class TestClass1
        {
            public enum SubEnum
            {
                TheItem1 = 1,
                Item2 = 2,
                Item21 = 3,
                Item22 = 4,
                Item23 = 5,
                Item3 = 5,
                Item4 = 6,
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public SubClass SubCls { get; set; }
            public SubEnum SubEnm { get; set; }

            [TypeScriptInterface(Name = "TestSubClass")]
            public class SubClass
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }

        #endregion

        #region - Test DataContract -

        private List<TypeScriptModule> GetDataToRenderDataContract(Settings settings)
        {
            var generator = new CodeTraverser(
                new MockSolution(
                    typeof(TestDataClass)
                    ).Object, settings);
            return generator.GetAllInterfaces().ToList();
        }

        private class JsonIgnoreAttribute : Attribute {}

        private const string ExpectedDataContractResult = OutputHeader + @"
declare module T4TS {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestDataClass **/
    export interface TestDataClass {
        Id: number;
        Name: string;
        XmlIgnored: string;
    }
}
";
        private const string ExpectedDataContractWithXmlIgnoredResult = OutputHeader + @"
declare module T4TS {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestDataClass **/
    export interface TestDataClass {
        Id: number;
        Name: string;
        JsonIgnored: string;
    }
}
";
        private const string ExpectedDataContractWithXmlIgnoredAndJsonIgnoredResult = OutputHeader + @"
declare module T4TS {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestDataClass **/
    export interface TestDataClass {
        Id: number;
        Name: string;
    }
}
";

        [DataContract]
        public class TestDataClass
        {
            public int Id { get; set; }
            public string Name { get; set; }

            [JsonIgnore]
            public string JsonIgnored { get; set; }

            [XmlIgnore]
            public string XmlIgnored { get; set; }
        }

        #endregion

        #region - Test Data Inheritance -

        private List<TypeScriptModule> GetDataToRenderDataInheritance(Settings settings)
        {
            var generator = new CodeTraverser(
                new MockSolution(
                    typeof(TestDataParentClass),
                    typeof(TestDataChildClass)
                    ).Object, settings);
            return generator.GetAllInterfaces().ToList();
        }

        private const string ExpectedDataInheritanceResult = OutputHeader + @"
declare module T4TS {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestDataParentClass **/
    export interface TestDataParentClass {
        Id: number;
    }
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestDataChildClass **/
    export interface TestDataChildClass extends T4TS.TestDataParentClass {
        Name: string;
    }
}
";
        private const string ExpectedDataNoInheritanceResult = OutputHeader + @"
declare module T4TS {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestDataChildClass **/
    export interface TestDataChildClass {
        Name: string;
    }
}
";
        public abstract class TestDataParentClass
        {
            public int Id { get; internal protected set; }
        }

        [TypeScriptInterface]
        public class TestDataChildClass : TestDataParentClass
        {
            public string Name { get; set; }
        }

        #endregion
    }
}