using System;
using System.Collections.Generic;

namespace T4TS.Example
{
    [TypeScriptInterface(Module = "Fooz", NamePrefix = "I")]
    public class Foobar
    {
        [TypeScriptMember(Name = "OverrideAll", Optional = true, Type = "any")]
        public string SomeString { get; set; }

        public Guid AGuid { get; set; }
        public Foobar Recursive { get; set; }
        public int? NullableInt { get; set; }
        public double? NullableDouble { get; set; }
        public Barfoo[] NestedObjectArr { get; set; }
        public List<Barfoo> NestedObjectList { get; set; }
        public string[][] TwoDimensions { get; set; }
        public Barfoo[][][] ThreeDimensions { get; set; }

        [TypeScriptMember(CamelCase = true)]
        public int CamelCasePlease { get; set; }

        [TypeScriptMember(Ignore = true)]
        public int IgnoreMe { get; set; }

        [TypeScriptMember(Ignore = false)]
        public int DoNotIgnoreMe { get; set; }
    }
}