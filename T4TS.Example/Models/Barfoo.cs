using System;
using System.Collections.Generic;

namespace T4TS.Example.Models
{
    [TypeScriptInterface(Module = "")]
    public class Barfoo
    {
        public int Number { get; set; }
        public Inherited Complex { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public Dictionary<string, object> aValue { get; set; }
    }
}
