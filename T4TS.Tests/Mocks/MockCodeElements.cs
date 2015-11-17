using System;
using EnvDTE;
using Moq;
using System.Linq;
using System.Collections.Generic;
using T4TS.Tests.Utils;

namespace T4TS.Tests.Mocks
{
    internal class MockCodeElements : CodeElements<CodeNamespace>
    {
        public MockCodeElements(params Type[] types)
        {
            NamespaceUtil.GroupedByNamespace(types).ToList().ForEach(kv => {
                var codeNamespace = new Mock<CodeNamespace>(MockBehavior.Strict);
                codeNamespace.Setup(x => x.Members).Returns(new MockCodeTypes(kv.Value));
                codeNamespace.Setup(x => x.Name).Returns(kv.Key);

                Add(codeNamespace.Object);
            });
        }
    }
}