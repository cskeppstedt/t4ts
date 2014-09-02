using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T4TS
{
    public class TypeScriptInterfaceMember
    {
        public string Name { get; set; }
        public TypescriptType Type { get; set; }
        public bool Optional { get; set; }
        //public string FullName { get; set; }
        public bool Ignore { get; set; }
    }
}
