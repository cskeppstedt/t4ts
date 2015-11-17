using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using T4TS.Example.Models;
using T4TS.Tests.Mocks;
using T4TS.Tests.Traversal.Models;

namespace T4TS.Tests.Traversal
{
    [TestClass]
    public class ProjectTraversalTests
    {
        [TestMethod]
        public void ShouldVisitEachNamespace()
        {
            var proj = new MockProjects(subProjectItems: null, types: new Type[]
            {
                typeof(LocalModel),
                typeof(ModelFromDifferentProject)
            }).Single();

            var expectedNames = new List<string> { "T4TS.Tests.Traversal.Models", "T4TS.Example.Models" };
            var actualNames   = new List<string>();

            new ProjectTraverser(proj, (ns) => {
                actualNames.Add(ns.Name);
            });

            CollectionAssert.AreEqual(expectedNames, actualNames);
        }

        [TestMethod]
        public void ShouldVisitSubProjectItems()
        {
            var subProjItem = new MockProjectItems(subProjectItems: null, types: new Type[]
            {
                typeof(ModelFromDifferentProject)
            }).Single();

            var moqSubProjectItems = new Mock<ProjectItems>();
            moqSubProjectItems.Setup(x => x.GetEnumerator()).Returns(() => new[] { subProjItem }.GetEnumerator());

            var proj = new MockProjects(subProjectItems: moqSubProjectItems.Object, types: new Type[]
            {
                typeof(LocalModel)
            }).Single();

            var expectedNames = new List<string> { "T4TS.Tests.Traversal.Models", "T4TS.Example.Models" };
            var actualNames   = new List<string>();

            new ProjectTraverser(proj, (ns) => {
                actualNames.Add(ns.Name);
            });

            CollectionAssert.AreEqual(expectedNames, actualNames);
        }
    }
}