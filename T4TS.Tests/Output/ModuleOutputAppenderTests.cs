using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests
{
    [TestClass]
    public class ModuleOutputAppenderTests
    {
        [TestMethod]
        public void RespectsCompatibilityVersion()
        {
            var sb = new StringBuilder();
            var module = new TypeScriptModule
            {
                QualifiedName = "Foo"
            };
            
            var settings = new Settings();
            var appender = new ModuleOutputAppender(sb, 0, settings);

            settings.CompatibilityVersion = new Version(0, 8, 3);
            appender.AppendOutput(module);
            Assert.IsTrue(sb.ToString().StartsWith("module"));
            sb.Clear();

            settings.CompatibilityVersion = new Version(0, 9, 0);
            appender.AppendOutput(module);
            Assert.IsTrue(sb.ToString().StartsWith("declare module"));
            sb.Clear();

            settings.CompatibilityVersion = null;
            appender.AppendOutput(module);
            Assert.IsTrue(sb.ToString().StartsWith("declare module"));
        }
    }
}
