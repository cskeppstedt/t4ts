using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Example.Models
{
    [TypeScriptInterface]
    public class Inherited: List<Barfoo>
    {
        public string StringProperty { get; set; }
    }
}
