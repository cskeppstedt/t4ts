using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace T4TS.Tests
{
    [TestClass]
    public partial class MemberOutputAppenderTests
    {
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
            var settings = new Settings { UseNativeDates = "Date" };
            string res = OutputFormatter.GetOutput(GetDataToRender(settings), settings);
            Console.WriteLine(res);
            Assert.AreEqual(ExpectedResult, res);
        }

        [TestMethod]
        public void TestOutputSubClasses()
        {
            var settings = new Settings { UseNativeDates = "Date" };
            string res = OutputFormatter.GetOutput(GetDataToRenderSubClasses(settings), settings);
            Console.WriteLine(res);
            Assert.AreEqual(ExpectedSubClassesResult, res);
        }

        [TestMethod]
        public void TestOutputDataContract()
        {
            var settings = new Settings { UseNativeDates = "Date" };
            var res = OutputFormatter.GetOutput(GetDataToRenderDataContract(settings), settings);
            // Test behaviour by default - no process DataContract classes 
            Assert.AreEqual(OutputHeader, res);
            // Test extended behaviour - process DataContract classes 
            settings.ProcessDataContracts = true;
            res = OutputFormatter.GetOutput(GetDataToRenderDataContract(settings), settings);
            Console.WriteLine(res);
            Assert.AreEqual(ExpectedDataContractResult, res);
        }

        [TestMethod]
        public void TestInheritedGeneric()
        {
            var settings = new Settings { UseNativeDates = "Date", ProcessDataContracts = true };
            string res = OutputFormatter.GetOutput(GetInheritedGenericDataToRender(settings), settings);
            Console.WriteLine(res);
            Assert.AreEqual(ExpectedInheritedGenericResult, res);
        }
    }
}