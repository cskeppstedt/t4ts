using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T4TS.Example.Models
{
    [TypeScriptInterface]
    public class Barfoo
    {
        public int Number { get; set; }
        public Inherited Complex { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }
}
