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
        
        public TypeReference(
            TypeName sourceType,
            TypeReferenceFactory factory)
        {
            this.SourceType = sourceType;
            if (sourceType.TypeArguments != null)
            {
                this.TypeArgumentReferences = sourceType.TypeArguments
                    .Select((argumentType) =>
                        factory.GetTypeReference(argumentType))
                    .ToList();
            }
            else
            {
                this.TypeArgumentReferences = new TypeReference[0];
            }
        }
    }
}
