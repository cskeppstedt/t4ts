using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace T4TS.Outputs
{
    [System.Diagnostics.DebuggerDisplay("TypeScriptInterface {FullName}")]    
    public class TypeScriptInterface : TypeReference
    {
        public List<TypeScriptInterfaceMember> Members { get; set; }
        public TypeReference IndexedType { get; set; }
        public TypeReference Parent { get; set; }

        public IList<TypeReference> Bases { get; set; }

        public TypeScriptInterface(
            TypeName sourceType,
            IEnumerable<TypeReference> typeArgumentReferences,
            TypeReference contextTypeReference)
                : base(
                    sourceType,
                    typeArgumentReferences,
                    contextTypeReference)
        {
            Members = new List<TypeScriptInterfaceMember>();
            Bases = new List<TypeReference>();
        }
    }
}
