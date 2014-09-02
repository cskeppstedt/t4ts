using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnvDTE;
using Moq;
using EnvDTE80;
using System.Reflection;
using T4TS.Tests.Utils;
using T4TS.Tests.Models;
using T4TS.Example.Models;

namespace T4TS.Tests.Traversal
{
    [TestClass]
    public class CodeTraverserTests
    {
        [TestMethod]
        public void ShouldBuildInterfacesFromMarkedClassesOnly()
        {
            var solution = DTETransformer.BuildDteSolution(
                typeof(LocalModel),                 // has the TypeScriptInterface attribute
                typeof(ModelFromDifferentProject),  // has the TypeScriptInterface attribute
                typeof(string)                      // has no TypeScriptInterface attribute
            );

            var codeTraverser = new CodeTraverser(solution, new Settings());
            Assert.AreEqual(2, codeTraverser.GetAllInterfaces().Count());
        }
    }
}
