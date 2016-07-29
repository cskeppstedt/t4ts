﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using T4TS.Tests.Mocks;

namespace T4TS.Tests.Traversal
{
    [TestClass]
    public class ClassTraversalTests
    {
        class M
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
        }

        [TestMethod]
        public void ShouldVisitEachProperty()
        {
            var codeClass = new MockCodeClass(typeof(M)).Object;
            int callCount = 0;
            
            var expectedNames = new string[] {"A","B","C"};
            
            new ClassTraverser(codeClass, (p) => { Assert.AreEqual(expectedNames[callCount++], p.Name); });
            Assert.AreEqual(3, callCount);
        }
    }
}
