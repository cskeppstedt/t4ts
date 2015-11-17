using System;
using EnvDTE;
using Moq;

namespace T4TS.Tests.Mocks
{
    internal class MockProjects : BaseList<Project>, Projects
    {
        public Properties Properties
        {
            get { throw new NotImplementedException(); }
        }

        public new DTE Parent
        {
            get { throw new NotImplementedException(); }
        }

        public MockProjects(ProjectItems subProjectItems, params Type[] types)
        {
            var project = new Mock<Project>(MockBehavior.Strict);
            project.Setup(x => x.ProjectItems).Returns(new MockProjectItems(subProjectItems, types));
            Add(project.Object);
        }
    }
}