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
        public void MemberOutputAppenderRespectsCompatibilityVersion()
        {
            var sb = new StringBuilder();
            
            var member = new TypeScriptInterfaceMember
            {
                Name = "Foo",
                //FullName = "Foo",
                Type = new BoolType()
            };

            var settings = new Settings();
            var appender = new MemberOutputAppender(sb, 0, settings);

            settings.CompatibilityVersion = new Version(0, 8, 3);
            appender.AppendOutput(member);
            Assert.IsTrue(sb.ToString().Contains("bool"));
            Assert.IsFalse(sb.ToString().Contains("boolean"));
            sb.Clear();

            settings.CompatibilityVersion = new Version(0, 9, 0);
            appender.AppendOutput(member);
            Assert.IsTrue(sb.ToString().Contains("boolean"));
            sb.Clear();

            settings.CompatibilityVersion = null;
            appender.AppendOutput(member);
            Assert.IsTrue(sb.ToString().Contains("boolean"));
        }
    }
}
