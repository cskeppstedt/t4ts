using System;
using EnvDTE;
using Moq;

namespace T4TS.Tests
{
    class MockSolution : Mock<Solution>
    {
        public MockSolution(params Type[] types) : base(MockBehavior.Strict)
        {
            Setup(x => x.Projects).Returns(new MockProjects(types));
        }
    }
}