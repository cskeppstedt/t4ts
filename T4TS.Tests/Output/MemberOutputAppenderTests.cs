using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Outputs;

namespace T4TS.Tests
{
    [TestClass]
    public class MemberOutputAppenderTests
    {
        [TestMethod]
        public void TypescriptVersion083YieldsBool()
        {
            Version version = new Version(0, 8, 3);
            TypeContext typeContext = new TypeContext(new TypeContext.Settings()
            {
                CompatibilityVersion = version
            });

            var sb = new StringBuilder();
            var member = new TypeScriptMember
            {
                Name = "Foo",
                Type = typeContext.GetTypeReference(
                    TypeName.FromLiteral(typeof(bool).FullName),
                    contextTypeReference: null)
            };

            var appender = new MemberOutputAppender(
                new OutputSettings
                {
                    CompatibilityVersion = version
                },
                typeContext);

            appender.AppendOutput(
                sb,
                0,
                member);
            Assert.AreEqual("Foo: bool;", sb.ToString().Trim());
        }

        [TestMethod]
        public void TypescriptVersion090YieldsBoolean()
        {
            TypeContext typeContext = new TypeContext();

            var sb = new StringBuilder();
            var member = new TypeScriptMember
            {
                Name = "Foo",
                Type = typeContext.GetTypeReference(
                    TypeName.FromLiteral(typeof(bool).FullName),
                    contextTypeReference: null)
            };

            var appender = new MemberOutputAppender(
                new OutputSettings
                {
                    CompatibilityVersion = new Version(0, 9, 0)
                },
                new TypeContext());

            appender.AppendOutput(
                sb,
                0,
                member);
            Assert.AreEqual("Foo: boolean;", sb.ToString().Trim());
        }

        [TestMethod]
        public void DefaultTypescriptVersionYieldsBoolean()
        {
            TypeContext typeContext = new TypeContext();

            var sb = new StringBuilder();
            var member = new TypeScriptMember
            {
                Name = "Foo",
                Type = typeContext.GetTypeReference(
                    TypeName.FromLiteral(typeof(bool).FullName),
                    contextTypeReference: null)
            };

            var appender = new MemberOutputAppender(
                new OutputSettings
                {
                    CompatibilityVersion = null
                },
                new TypeContext());

            appender.AppendOutput(
                sb,
                0,
                member);
            Assert.AreEqual("Foo: boolean;", sb.ToString().Trim());
        }
    }
}
