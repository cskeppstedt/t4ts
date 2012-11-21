using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    [TypeScriptInterface(Module = "Fooz")]
    public class Foobar
    {
        [TypeScriptMember(Name = "OverrideAll", Optional = true, Type = "bool")]
        public string SomeString { get; set; }
        public Foobar Recursive { get; set; }
        public Barfoo[] NestedObjectArr { get; set; }
        public List<Barfoo> NestedObjectList { get; set; }
        public string[][] TwoDimensions { get; set; }
        public Barfoo[][][] ThreeDimensions { get; set; }
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
}
