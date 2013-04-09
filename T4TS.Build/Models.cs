using System;
using System.Collections.Generic;

namespace T4TS.Build
{
    [TypeScriptInterface(Module = "")]
    public class Barfoo
    {
        public int Number { get; set; }
        public Inherited Complex { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }

    [TypeScriptInterface(Module = "Fooz", NamePrefix = "I")]
    public class Foobar
    {
        [TypeScriptMember(Name = "OverrideAll", Optional = true, Type = "bool")]
        public string SomeString { get; set; }
        public Foobar Recursive { get; set; }
        public int? NullableInt { get; set; }
        public double? NullableDouble { get; set; }
        public Barfoo[] NestedObjectArr { get; set; }
        public List<Barfoo> NestedObjectList { get; set; }
        public string[][] TwoDimensions { get; set; }
        public Barfoo[][][] ThreeDimensions { get; set; }

        [TypeScriptMember(CamelCase = true)]
        public int CamelCasePlease { get; set; }
    }

    [TypeScriptInterface(Name = "OverridenName")]
    public class Inherited : List<Barfoo>
    {
        [TypeScriptMember(Optional = true, Name = "OtherName")]
        public string StringProperty { get; set; }
        public int[] Integers { get; set; }
        public List<double> Doubles { get; set; }
        public List<List<int>> TwoDimList { get; set; }
    }

    #region Inheritance Test

    public class TestClass
    {
        public string Property { get; set; }
    }

    [TypeScriptInterface]
    public class InheritanceTest1 : Barfoo
    {
        public string SomeString { get; set; }
        public Foobar Recursive { get; set; }
    }

    [TypeScriptInterface]
    public class InheritanceTest2 : InheritanceTest1
    {
        public string SomeString2 { get; set; }
        public Foobar Recursive2 { get; set; }
    }

    [TypeScriptInterface]
    public class InheritanceTest3 : Inherited
    {
        public string SomeString3 { get; set; }
        public Foobar Recursive3 { get; set; }
    }

    [TypeScriptInterface]
    public class InheritanceTest4 : TestClass
    {
        public string SomeString4 { get; set; }
        public Foobar Recursive4 { get; set; }
    }

    #endregion
}
