using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace T4TS
{
    public class TypeScriptInterfaceAttribute: Attribute
    {
        public string Module { get; set; }

        public TypeScriptInterfaceAttribute()
            : this("Api")
        {
        }

        public TypeScriptInterfaceAttribute(string module) 
        {
            this.Module = module;
        }
    }
}