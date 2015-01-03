using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using T4TS.Example.Models;
using T4TS.Tests.Models;
using T4TS.Tests.Utils;

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

        [TestMethod]
        public void ShouldHandleReservedPropNames()
        {
            var solution = DTETransformer.BuildDteSolution(typeof(ReservedPropModel));
            var codeTraverser = new CodeTraverser(solution, new Settings());
            
            var modules = codeTraverser.GetAllInterfaces();
            var interfaces = modules.Single().Interfaces;
            var modelInterface = interfaces.Single();
            
            var classProp = modelInterface.Members.SingleOrDefault(m => m.Name == "class");
            var readonlyProp = modelInterface.Members.SingleOrDefault(m => m.Name == "readonly");
            var publicProp = modelInterface.Members.SingleOrDefault(m => m.Name == "public");

            Assert.AreEqual(3, modelInterface.Members.Count);

            Assert.IsNotNull(classProp);
            Assert.IsNotNull(readonlyProp);
            Assert.IsNotNull(publicProp);
            
            Assert.IsTrue(publicProp.Optional);
        }
    }
}
