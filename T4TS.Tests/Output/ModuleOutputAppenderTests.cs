﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Outputs;

namespace T4TS.Tests
{
    [TestClass]
    public class ModuleOutputAppenderTests
    {
        [TestMethod]
        public void TypescriptVersion083YieldsModule()
        {
            Version version = new Version(0, 8, 3);

            var sb = new StringBuilder();
            var module = new TypeScriptModule
            {
                QualifiedName = "Foo"
            };
            
            var appender = new ModuleOutputAppender(
                sb,
                0,
                new Settings
                {
                    CompatibilityVersion = version
                },
                new TypeContext(new TypeContext.Settings()
                {
                    CompatibilityVersion = version
                }));

            appender.AppendOutput(module);
            Assert.IsTrue(sb.ToString().StartsWith("module "));
        }

        [TestMethod]
        public void TypescriptVersion090YieldsDeclareModule()
        {
            var sb = new StringBuilder();
            var module = new TypeScriptModule
            {
                QualifiedName = "Foo"
            };

            var appender = new ModuleOutputAppender(
                sb,
                0,
                new Settings
                {
                    CompatibilityVersion = new Version(0, 9, 0)
                },
                new TypeContext());

            appender.AppendOutput(module);
            Assert.IsTrue(sb.ToString().StartsWith("declare module "));
        }

        [TestMethod]
        public void DefaultTypescriptVersionYieldsDeclareModule()
        {
            var sb = new StringBuilder();
            var module = new TypeScriptModule
            {
                QualifiedName = "Foo"
            };

            var appender = new ModuleOutputAppender(
                sb,
                0,
                new Settings
                {
                    CompatibilityVersion = null
                },
                new TypeContext());

            appender.AppendOutput(module);
            Assert.IsTrue(sb.ToString().StartsWith("declare module "));
        }
    }
}
