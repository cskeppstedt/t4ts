using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using T4TS.Example.Models;
using T4TS.Tests.Mocks;
using T4TS.Tests.Traversal.Models;

namespace T4TS.Tests.Traversal
{
    [TestClass]
    public class CodeTraverserTests
    {
        [TestMethod]
        public void ShouldBuildInterfacesFromMarkedClassesOnly()
        {
            var solution = new MockSolution(
                typeof(LocalModel),                 // has the TypeScriptInterface attribute
                typeof(ModelFromDifferentProject),  // has the TypeScriptInterface attribute
                typeof(string)                      // has no TypeScriptInterface attribute
            ).Object;

            var codeTraverser = new CodeTraverser(solution, new Settings());
            Assert.AreEqual(2, codeTraverser.GetAllInterfaces().Count());
        }

        [TestMethod]
        public void ShouldWorkIfSolutionContainsPartialClasses() {
          //this may not make much sense, but this is my best guess at mimicking partial classes...
          //Actually traceing out all the TypeScriptInterfaces in the T4TS.Example solution contains these:
          //  ...
          //T4TS.Example.Models.PartialModelByEF 
          //T4TS.Example.Models.PartialModelByEF 
          //T4TS.Example.Models.InheritsPartialModelByEF 
          //T4TS.Example.Models.Partial 
          //T4TS.Example.Models.Partial 
          //  ...

          var solution = new MockSolution(
              typeof(T4TS.Tests.Fixtures.Partial.PartialModel),
              typeof(T4TS.Tests.Fixtures.Partial.PartialModel),
              typeof(T4TS.Tests.Fixtures.Partial.InheritsFromPartialModel)
          ).Object;

          var codeTraverser = new CodeTraverser(solution, new Settings());
          var allModules = codeTraverser.GetAllInterfaces();
          Assert.AreEqual(1, allModules.Count());
          Assert.AreEqual(3, allModules.First().Interfaces.Count());
        }

        [TestMethod]
        public void ShouldHandleReservedPropNames()
        {
            var solution = new MockSolution(typeof(ReservedPropModel)).Object;
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
