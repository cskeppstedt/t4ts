using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests
{
    [TestClass]
    public class MemberOutputAppenderTests
    {
        [TestMethod]
        public void TypescriptVersion083YieldsBool()
        {
            var sb = new StringBuilder();
            var member = new TypeScriptInterfaceMember
            {
                Name = "Foo",
                Type = new BoolType()
            };

            var appender = new MemberOutputAppender(sb, 0, new Settings
            {
                CompatibilityVersion = new Version(0, 8, 3)
            });

            appender.AppendOutput(member);
            Assert.AreEqual("Foo: bool;", sb.ToString().Trim());
        }

        [TestMethod]
        public void TypescriptVersion090YieldsBoolean()
        {
            var sb = new StringBuilder();
            var member = new TypeScriptInterfaceMember
            {
                Name = "Foo",
                Type = new BoolType()
            };

            var appender = new MemberOutputAppender(sb, 0, new Settings
            {
                CompatibilityVersion = new Version(0, 9, 0)
            });

            appender.AppendOutput(member);
            Assert.AreEqual("Foo: boolean;", sb.ToString().Trim());
        }

        [TestMethod]
        public void DefaultTypescriptVersionYieldsBoolean()
        {
            var sb = new StringBuilder();
            var member = new TypeScriptInterfaceMember
            {
                Name = "Foo",
                Type = new BoolType()
            };

            var appender = new MemberOutputAppender(sb, 0, new Settings
            {
                CompatibilityVersion = null
            });

            appender.AppendOutput(member);
            Assert.AreEqual("Foo: boolean;", sb.ToString().Trim());
        }
    }
}
