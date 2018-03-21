using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace T4TS
{
    [System.Diagnostics.DebuggerDisplay("TypeScriptInterface {FullName}")]    
    public class TypeScriptInterface : TypeScriptOutputType
    {
        public string Name { get; set; }
        public string FullName { get; set; }

        public List<TypeScriptInterfaceMember> Members { get; set; }
        public TypescriptType IndexedType { get; set; }
        public TypeScriptOutputType Parent { get; set; }
        public TypeScriptModule Module { get; set; }
        public List<string> GenericParameters { get; set; }

        public IList<TypeScriptOutputType> Bases { get; set; }

        public TypeScriptInterface()
        {
            Members = new List<TypeScriptInterfaceMember>();
            Bases = new List<TypeScriptOutputType>();
            GenericParameters = new List<string>();
        }
    }
}
