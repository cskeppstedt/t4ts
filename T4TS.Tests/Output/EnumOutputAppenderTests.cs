using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Outputs;

namespace T4TS.Tests.Output
{
    [TestClass]
    public class EnumOutputAppenderTests
    {
        private const string NotGlobalExplicitExpectedOutput = @"/** Generated from Foo.ExplicitEnum */
export enum ExplicitEnum {
    First = 1,
    Second = 2,
    Fifth = 5
}
";

        private const string NotGlobalImplicitExpectedOutput = @"/** Generated from Foo.ImplicitEnum */
export enum ImplicitEnum {
    First,
    Second,
    Third
}
";
        private const string GlobalExplicitExpectedOutput = @"/** Generated from Foo.ExplicitEnum */
enum ExplicitEnum {
    First = 1,
    Second = 2,
    Fifth = 5
}
";
        private TypeContext typeContext = new TypeContext();

        private TypeScriptEnum explicitEnumType;
        private TypeScriptEnum implicitEnumType;

        public EnumOutputAppenderTests()
        {
            bool created;
            explicitEnumType = this.typeContext.GetOrCreateEnum(
                "Foo",
                TypeName.ParseDte("Foo.ExplicitEnum"),
                "ExplicitEnum",
                out created);
            explicitEnumType.Values = new List<TypeScriptEnumValue>()
            {
                new TypeScriptEnumValue()
                {
                    Name = "First",
                    Value = "1"
                },
                new TypeScriptEnumValue()
                {
                    Name = "Second",
                    Value = "2"
                },
                new TypeScriptEnumValue()
                {
                    Name = "Fifth",
                    Value = "5"
                }
            };

            implicitEnumType = this.typeContext.GetOrCreateEnum(
                "Foo",
                TypeName.ParseDte("Foo.ImplicitEnum"),
                "ImplicitEnum",
                out created);
            implicitEnumType.Values = new List<TypeScriptEnumValue>()
            {
                new TypeScriptEnumValue()
                {
                    Name = "First"
                },
                new TypeScriptEnumValue()
                {
                    Name = "Second"
                },
                new TypeScriptEnumValue()
                {
                    Name = "Third"
                }
            };
        }

        [TestMethod]
        public void EnumAppenderExplicitNotGlobal()
        {
            var sb = new StringBuilder();
            var appender = new EnumOutputAppender(
                new OutputSettings(),
                this.typeContext,
                inGlobalModule: false);

            appender.AppendOutput(
                sb,
                0,
                this.explicitEnumType);

            Assert.AreEqual(EnumOutputAppenderTests.NotGlobalExplicitExpectedOutput, sb.ToString());
        }

        [TestMethod]
        public void EnumAppenderExplicitGlobal()
        {
            var sb = new StringBuilder();
            var appender = new EnumOutputAppender(
                new OutputSettings(),
                this.typeContext,
                inGlobalModule: true);

            appender.AppendOutput(
                sb,
                0,
                this.explicitEnumType);

            Assert.AreEqual(EnumOutputAppenderTests.GlobalExplicitExpectedOutput, sb.ToString());
        }

        [TestMethod]
        public void EnumAppenderImplicitNotGlobal()
        {
            var sb = new StringBuilder();
            var appender = new EnumOutputAppender(
                new OutputSettings(),
                this.typeContext,
                inGlobalModule: false);

            appender.AppendOutput(
                sb,
                0,
                this.implicitEnumType);

            Assert.AreEqual(EnumOutputAppenderTests.NotGlobalImplicitExpectedOutput, sb.ToString());
        }
    }
}
