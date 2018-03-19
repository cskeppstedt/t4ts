using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace T4TS
{
    [System.Diagnostics.DebuggerDisplay("TypeScriptInterface {FullName}")]    
    public class TypeScriptInterface
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Extends { get; set; }

        public List<TypeScriptInterfaceMember> Members { get; set; }
        public TypescriptType IndexedType { get; set; }
        public TypeScriptInterface Parent { get; set; }
        public TypeScriptModule Module { get; set; }

        public IList<TypeScriptInterface> Bases { get; set; }

        public TypeScriptInterface()
        {
            Members = new List<TypeScriptInterfaceMember>();
            Bases = new List<TypeScriptInterface>();
        }
    }
}
