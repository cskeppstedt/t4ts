using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public interface TypeReferenceFactory
    {
        TypeReference GetTypeReference(TypeName sourceType);
    }
}
