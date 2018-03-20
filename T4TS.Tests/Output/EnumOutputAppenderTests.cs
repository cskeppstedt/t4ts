using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Output
{
    [TestClass]
    public class EnumOutputAppenderTests
    {
        private const string NotGlobalExplicitExpectedOutput = @"/** Generated from Foo.SampleEnum **/
export enum SampleEnum {
    First = 1,
    Second = 2,
    Fifth = 5
}
";

        private const string NotGlobalImplicitExpectedOutput = @"/** Generated from Foo.SampleEnum **/
export enum SampleEnum {
    First,
    Second,
    Third
}
";
        private const string GlobalExplicitExpectedOutput = @"/** Generated from Foo.SampleEnum **/
enum SampleEnum {
    First = 1,
    Second = 2,
    Fifth = 5
}
";

        private static TypeScriptEnum explicitEnumType = new TypeScriptEnum()
        {
            FullName = "Foo.SampleEnum",
            Name = "SampleEnum",
            Values = new List<TypeScriptEnumValue>()
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
                }
        };
        private static TypeScriptEnum implicitEnumType = new TypeScriptEnum()
        {
            FullName = "Foo.SampleEnum",
            Name = "SampleEnum",
            Values = new List<TypeScriptEnumValue>()
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
                }
        };

        [TestMethod]
        public void EnumAppenderExplicitNotGlobal()
        {
            var sb = new StringBuilder();
            var appender = new EnumOutputAppender(
                sb,
                0,
                new Settings(),
                inGlobalModule: false);

            appender.AppendOutput(EnumOutputAppenderTests.explicitEnumType);

            Assert.AreEqual(EnumOutputAppenderTests.NotGlobalExplicitExpectedOutput, sb.ToString());
        }

        [TestMethod]
        public void EnumAppenderExplicitGlobal()
        {
            var sb = new StringBuilder();
            var appender = new EnumOutputAppender(
                sb,
                0,
                new Settings(),
                inGlobalModule: true);

            appender.AppendOutput(EnumOutputAppenderTests.explicitEnumType);

            Assert.AreEqual(EnumOutputAppenderTests.GlobalExplicitExpectedOutput, sb.ToString());
        }

        [TestMethod]
        public void EnumAppenderImplicitNotGlobal()
        {
            var sb = new StringBuilder();
            var appender = new EnumOutputAppender(
                sb,
                0,
                new Settings(),
                inGlobalModule: false);

            appender.AppendOutput(EnumOutputAppenderTests.implicitEnumType);

            Assert.AreEqual(EnumOutputAppenderTests.NotGlobalImplicitExpectedOutput, sb.ToString());
        }
    }
}
