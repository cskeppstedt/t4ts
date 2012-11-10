using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace T4TS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class TypeScriptInterfaceAttribute: Attribute
    {
        public TypeScriptInterfaceAttribute()
        {
        }
    }
}