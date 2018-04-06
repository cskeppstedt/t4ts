using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Example.Models;
using T4TS.Tests.Models;
using T4TS.Tests.Utils;

namespace T4TS.Tests
{
    [TestClass]
    public class TraversalTests
    {
        class M
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
        }

        class N
        {
            public int A { get; set; }
        }

        [TestMethod]
        public void ShouldVisitEachProperty()
        {
            var codeClass = DTETransformer.BuildDteClass(typeof(M));
            int callCount = 0;

            var expectedNames = new string[] { "A", "B", "C" };

            Traversal.TraverseProperties(
                codeClass.Members,
                (p) => { Assert.AreEqual(expectedNames[callCount++], p.Name); });
            Assert.AreEqual(3, callCount);
        }

        [TestMethod]
        public void ShouldVisitEachCodeClass()
        {
            var project = DTETransformer.BuildDteProject(new Type[]
            {
                typeof(M),
                typeof(N)
            }, projectName: "NamespaceTraverserTests");

            int callCount = 0;
            var expectedNames = new string[] { "M", "N" };
            ProjectItem projectItem;

            if (TryGetSingle(project.ProjectItems.GetEnumerator(), out projectItem))
            {
                foreach (CodeNamespace ns in projectItem.FileCodeModel.CodeElements)
                    Traversal.TraverseClassesInNamespace(ns, (c) => { Assert.AreEqual(expectedNames[callCount++], c.Name); });
            }

            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public void ShouldVisitEachNamespace()
        {
            var proj = DTETransformer.BuildDteProject(new Type[]
            {
                typeof(LocalModel),
                typeof(ModelFromDifferentProject)
            }, projectName: "proj");

            int callCount = 0;
            var expectedNames = new string[] { "T4TS.Tests.Models", "T4TS.Example.Models" };

            T4TS.Traversal.TraverseNamespacesInProject(
                proj,
                (ns) => { Assert.AreEqual(expectedNames[callCount++], ns.Name); });

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
            var expectedNames = new string[] { "T4TS.Tests.Models", "T4TS.Example.Models" };

            T4TS.Traversal.TraverseNamespacesInProject(proj, (ns) => { Assert.AreEqual(expectedNames[callCount++], ns.Name); });

            Assert.AreEqual(2, callCount);
        }

        private bool TryGetSingle<T>(IEnumerator enumerator, out T item) where T : class
        {
            enumerator.MoveNext();
            item = (T)enumerator.Current;
            return !enumerator.MoveNext();
        }
    }
}
