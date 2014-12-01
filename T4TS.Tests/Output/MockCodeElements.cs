using System;
using EnvDTE;
using Moq;

namespace T4TS.Tests
{
    internal class MockCodeElements : CodeElemens<CodeNamespace>
    {
        public MockCodeElements(params Type[] types)
        {
            var codeNamespace = new Mock<CodeNamespace>(MockBehavior.Strict);
            codeNamespace.Setup(x => x.Members).Returns(new MockCodeTypes(types));
            Add(codeNamespace.Object);
        }
    }
}