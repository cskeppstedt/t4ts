using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Generator
{
    public class TypeScriptInterface
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Module { get; set; }
        public List<TypeScriptInterfaceMember> Members { get; set; }
        public string IndexerType { get; set; }
    }
}
