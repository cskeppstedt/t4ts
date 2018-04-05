using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public class TypeReference
    {
        public TypeName SourceType { get; private set; }

        public IEnumerable<TypeReference> TypeArgumentReferences { get; private set; }

        public TypeReference ContextTypeReference { get; private set; }

        
        public TypeReference(
            TypeName sourceType,
            IEnumerable<TypeReference> typeArgumentReferences,
            TypeReference contextTypeReference)
        {
            this.SourceType = sourceType;
            this.TypeArgumentReferences = typeArgumentReferences;
            this.ContextTypeReference = contextTypeReference;
        }
    }
}
