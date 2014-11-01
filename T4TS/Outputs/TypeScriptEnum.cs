using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace T4TS
{
    public class TypeScriptEnum
    {
        public string Name { get; set; }
        public string FullName { get; set; }

        public List<TypeScriptEnumMember> Members { get; set; }
        public TypeScriptModule Module { get; set; }
        public TypeScriptInterface Owner { get; set; }

        public TypeScriptEnum()
        {
            Members = new List<TypeScriptEnumMember>();
        }
    }
}
