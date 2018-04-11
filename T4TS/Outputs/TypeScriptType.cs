using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace T4TS.Outputs
{
    public class TypeScriptType : TypeReference
    {
        public List<TypeScriptMember> Fields { get; set; }
        public List<TypeScriptMethod> Methods { get; set; }
        public TypeReference IndexedType { get; set; }
        public TypeReference Parent { get; set; }

        public IList<TypeReference> Bases { get; set; }
        
        public bool IsClass { get; internal set; }

        public TypeScriptType(
            TypeName sourceType,
            IEnumerable<TypeReference> typeArgumentReferences,
            TypeReference contextTypeReference)
                : base(
                    sourceType,
                    typeArgumentReferences,
                    contextTypeReference)
        {
            Fields = new List<TypeScriptMember>();
            Methods = new List<TypeScriptMethod>();
            Bases = new List<TypeReference>();
        }
    }
}
