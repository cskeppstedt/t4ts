using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Example.Models
{
    [TypeScriptInterface]
    public class Foobar
    {
        public int IntegerProperty { get; set; }
        public string SomeString { get; set; }
        public Barfoo NestedObject { get; set; }
    }
}
