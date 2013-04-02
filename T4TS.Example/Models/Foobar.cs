using System.Collections.Generic;

namespace T4TS.Example.Models
{
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
        
        [TypeScriptMember(CamelCase=true)]
        public int CamelCasePlease { get; set; }
    }
}