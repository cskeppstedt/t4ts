using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T4TS.Tests.Utils;

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
        public void ShouldVisitEachNamespace()
        {
            var project = DTETransformer.BuildDteProject(new Type[]
            {
                typeof(M),
                typeof(N)
            }, "NamespaceTraverserTests");
        }
    }
}
