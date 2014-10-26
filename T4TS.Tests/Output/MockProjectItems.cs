using System;
using EnvDTE;
using Moq;

namespace T4TS.Tests
{
    class MockProjectItems : BaseList<ProjectItem>, ProjectItems
    {
        #region - Not Implemented Members -

        public ProjectItem AddFromFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromTemplate(string fileName, string name)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFolder(string name, string kind = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}")
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromFileCopy(string filePath)
        {
            throw new NotImplementedException();
        }

        public Project ContainingProject { get { throw new NotImplementedException(); } }

        #endregion

        public MockProjectItems(params Type[] types)
        {
            var fileCodeModel = new Mock<FileCodeModel>(MockBehavior.Strict);
            fileCodeModel.Setup(x => x.CodeElements).Returns(new MockCodeElements(types));

            var projectItem = new Mock<ProjectItem>(MockBehavior.Strict);
            projectItem.Setup(x => x.FileCodeModel).Returns(fileCodeModel.Object);
            projectItem.Setup(x => x.ProjectItems).Returns((ProjectItems)null);
            projectItem.Setup(x => x.SubProject).Returns((Project)null);
            Add(projectItem.Object);
        }
    }
}