using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace T4TS.Outputs
{
    [System.Diagnostics.DebuggerDisplay("TypeScriptInterface {FullName}")]    
    public class TypeScriptInterface : TypeScriptType
    {
        public TypeScriptInterface(
            TypeName sourceType,
            IEnumerable<TypeReference> typeArgumentReferences,
            TypeReference contextTypeReference)
                : base(
                    sourceType,
                    typeArgumentReferences,
                    contextTypeReference)
        {
        }
    }
}
