using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests
{
    [TestClass]
    public class TypeNameTests
    {
        [TestMethod]
        public void DteNonGenericType()
        {
            string rawName = "foo";
            TypeName name = TypeName.ParseDte(rawName);

            this.AssertAllEqual(
                name,
                rawName);

            Assert.AreEqual(
                0,
                name.TypeArguments.Count);
        }

        [TestMethod]
        public void DteSingleParameterGenericType()
        {
            string rawName = "foo<bar>";
            TypeName name = TypeName.ParseDte(rawName);
            Assert.AreEqual(
                rawName,
                name.RawName);
            Assert.AreEqual(
                rawName,
                name.QualifiedName);
            Assert.AreEqual(
                "foo`1",
                name.UniversalName);
            Assert.AreEqual(
                "foo",
                name.UnqualifiedName);
            Assert.AreEqual(
                1,
                name.TypeArguments.Count);

            this.AssertAllEqual(
                name.TypeArguments[0],
                "bar");
        }

        [TestMethod]
        public void DteMultipleParameterGenericType()
        {
            string rawName = "foo<bar,bark,bart>";
            TypeName name = TypeName.ParseDte(rawName);
            Assert.AreEqual(
                rawName,
                name.RawName);
            Assert.AreEqual(
                rawName,
                name.QualifiedName);
            Assert.AreEqual(
                "foo`3",
                name.UniversalName);
            Assert.AreEqual(
                "foo",
                name.UnqualifiedName);
            Assert.AreEqual(
                3,
                name.TypeArguments.Count);

            this.AssertAllEqual(
                name.TypeArguments[0],
                "bar");
            this.AssertAllEqual(
                name.TypeArguments[1],
                "bark");
            this.AssertAllEqual(
                name.TypeArguments[2],
                "bart");
        }

        [TestMethod]
        public void DteMultipleParameterReplaceGenericType()
        {
            string rawName = "foo<bar,bark,bart>";
            TypeName name = TypeName.ParseDte(rawName);
            TypeName renamed = name.ReplaceUnqualifiedName("somethingNew");
            Assert.AreEqual(
                "somethingNew<bar,bark,bart>",
                renamed.RawName);
            Assert.AreEqual(
                "somethingNew<bar,bark,bart>",
                renamed.QualifiedName);
            Assert.AreEqual(
                "somethingNew`3",
                renamed.UniversalName);
            Assert.AreEqual(
                "somethingNew",
                renamed.UnqualifiedName);
            Assert.AreEqual(
                3,
                renamed.TypeArguments.Count);

            this.AssertAllEqual(
                renamed.TypeArguments[0],
                "bar");
            this.AssertAllEqual(
                renamed.TypeArguments[1],
                "bark");
            this.AssertAllEqual(
                renamed.TypeArguments[2],
                "bart");
        }

        [TestMethod]
        public void DteMultipleParameterCompoundGenericType()
        {
            string rawName = "foo<bar<food,foot<bard>>,bark,bart<foosball>>";
            TypeName name = TypeName.ParseDte(rawName);
            Assert.AreEqual(
                rawName,
                name.RawName);
            Assert.AreEqual(
                rawName,
                name.QualifiedName);
            Assert.AreEqual(
                "foo`3",
                name.UniversalName);
            Assert.AreEqual(
                "foo",
                name.UnqualifiedName);
            Assert.AreEqual(
                3,
                name.TypeArguments.Count);


            TypeName parameter0 = name.TypeArguments[0];
            Assert.AreEqual(
                "bar<food,foot<bard>>",
                parameter0.RawName);
            Assert.AreEqual(
                "bar<food,foot<bard>>",
                parameter0.QualifiedName);
            Assert.AreEqual(
                "bar`2",
                parameter0.UniversalName);
            Assert.AreEqual(
                "bar",
                parameter0.UnqualifiedName);
            Assert.AreEqual(
                2,
                parameter0.TypeArguments.Count);

            this.AssertAllEqual(
                parameter0.TypeArguments[0],
                "food");


            TypeName parameter0_1 = parameter0.TypeArguments[1];
            Assert.AreEqual(
                "foot<bard>",
                parameter0_1.RawName);
            Assert.AreEqual(
                "foot<bard>",
                parameter0_1.QualifiedName);
            Assert.AreEqual(
                "foot`1",
                parameter0_1.UniversalName);
            Assert.AreEqual(
                "foot",
                parameter0_1.UnqualifiedName);
            Assert.AreEqual(
                1,
                parameter0_1.TypeArguments.Count);

            this.AssertAllEqual(
                parameter0_1.TypeArguments[0],
                "bard");


            this.AssertAllEqual(
                name.TypeArguments[1],
                "bark");

            TypeName parameter2 = name.TypeArguments[2];
            Assert.AreEqual(
                "bart<foosball>",
                parameter2.RawName);
            Assert.AreEqual(
                "bart<foosball>",
                parameter2.QualifiedName);
            Assert.AreEqual(
                "bart`1",
                parameter2.UniversalName);
            Assert.AreEqual(
                "bart",
                parameter2.UnqualifiedName);
            Assert.AreEqual(
                1,
                parameter2.TypeArguments.Count);

            this.AssertAllEqual(
                parameter2.TypeArguments[0],
                "foosball");
        }

        private void AssertAllEqual(
            TypeName name,
            string expected)
        {
            Assert.AreEqual(
                expected,
                name.RawName);
            Assert.AreEqual(
                expected,
                name.QualifiedName);
            Assert.AreEqual(
                expected,
                name.UniversalName);
            Assert.AreEqual(
                expected,
                name.UnqualifiedName);
        }
    }
}
