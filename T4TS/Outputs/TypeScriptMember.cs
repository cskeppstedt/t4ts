using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T4TS.Outputs
{
    public class TypeScriptMember
    {
        public string Name { get; set; }
        public TypeReference Type { get; set; }
        public bool Optional { get; set; }
        //public string FullName { get; set; }
        public bool Ignore { get; set; }
    }
}
