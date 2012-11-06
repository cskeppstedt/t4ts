using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Generator
{
    public class TypeScriptInterface
    {
        public string Name { get; set; }
        public List<TypeScriptInterfaceMember> Members { get; set; }
    }
}
