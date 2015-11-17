using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;
using T4TS.Tests.Mocks;

namespace T4TS.Tests.Traversal
{
    [TestClass]
    public class NamespaceTraverserTests
    {
        class M
        {
            public int A { get; set; }
        }

        class N
        {
            public int A { get; set; }
        }

        [TestMethod]
        public void ShouldVisitEachCodeClass()
        {
            var project = new MockProjects(null, new Type[]
            {
                typeof(M),
                typeof(N)
            }).Single();

            int callCount = 0;
            var expectedNames = new string[] { "M", "N" };
            ProjectItem projectItem;

            if (TryGetSingle(project.ProjectItems.GetEnumerator(), out projectItem))
            {
                foreach (CodeNamespace ns in projectItem.FileCodeModel.CodeElements)
                    new NamespaceTraverser(ns, (c) => { Assert.AreEqual(expectedNames[callCount++], c.Name); });
            }

            Assert.AreEqual(2, callCount);
        }

        private bool TryGetSingle<T>(IEnumerator enumerator, out T item) where T: class
        {
            enumerator.MoveNext();
            item = (T)enumerator.Current;
            return !enumerator.MoveNext();
        }
    }
}
