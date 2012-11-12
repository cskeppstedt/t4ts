using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeScriptInterface
    {
        public string Name { get; set; }
        public string FullName { get; set; }

        public List<TypeScriptInterfaceMember> Members { get; set; }
        public TypescriptType IndexedType { get; set; }

        public TypeScriptInterface()
        {
            Members = new List<TypeScriptInterfaceMember>();
        }
    }
}
