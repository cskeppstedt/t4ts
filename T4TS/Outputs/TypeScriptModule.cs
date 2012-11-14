using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeScriptModule
    {
        public string QualifiedName { get; set; }
        public List<TypeScriptInterface> Interfaces { get; set; }

        public TypeScriptModule()
        {
            Interfaces = new List<TypeScriptInterface>();
        }
    }
}
