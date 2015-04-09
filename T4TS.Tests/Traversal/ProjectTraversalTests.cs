using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T4TS.Tests.Utils;
using T4TS.Tests.Traversal.Models;
using T4TS.Example.Models;
using EnvDTE;
using Moq;

namespace T4TS.Tests.Traversal
{
    [TestClass]
    public class ProjectTraversalTests
    {
        [TestMethod]
        public void ShouldVisitEachNamespace()
        {
            var proj = DTETransformer.BuildDteProject(new Type[]
            {
                typeof(LocalModel),
                typeof(ModelFromDifferentProject)
            }, projectName: "proj");

            int callCount = 0;
            var expectedNames = new string[] { "T4TS.Tests.Traversal.Models", "T4TS.Example.Models" };

            new ProjectTraverser(proj, (ns) => { Assert.AreEqual(expectedNames[callCount++], ns.Name); });

            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public void ShouldVisitSubProjectItems()
        {
            var subProjItem = DTETransformer.BuildDteProjectItem(new Type[]
            {
                typeof(ModelFromDifferentProject)
            }, projectItemName: "subProj");

            var moqSubProjectItems = new Mock<ProjectItems>();
            moqSubProjectItems.Setup(x => x.GetEnumerator()).Returns(() => new[] { subProjItem }.GetEnumerator());

            var proj = DTETransformer.BuildDteProject(new Type[]
            {
                typeof(LocalModel)
            }, projectName: "proj", subProjectItems: moqSubProjectItems.Object);

            int callCount = 0;
            var expectedNames = new string[] { "T4TS.Tests.Traversal.Models", "T4TS.Example.Models" };
            
            new ProjectTraverser(proj, (ns) => { Assert.AreEqual(expectedNames[callCount++], ns.Name); });

            Assert.AreEqual(2, callCount);
        }
    }
}