using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T4TS.Example.Models
{
    [TypeScriptInterface]
    public class Barfoo
    {
        public int No { get; set; }
        public double Dbl { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }
}
