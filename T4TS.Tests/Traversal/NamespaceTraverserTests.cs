using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T4TS.Tests.Utils;
using EnvDTE;
using System.Collections;

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
