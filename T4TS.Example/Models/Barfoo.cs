using System;

namespace T4TS.Example
{
    [TypeScriptInterface(Module = "")]
    public class Barfoo
    {
        public int Number { get; set; }
        public Inherited Complex { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }
}