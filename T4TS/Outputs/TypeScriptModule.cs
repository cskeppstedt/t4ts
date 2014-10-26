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
        public List<TypeScriptEnum> Enums { get; set; }

        /// <summary>
        /// Returns true if this is the global namespace (ie. no module name)
        /// </summary>
        public bool IsGlobal
        {
            get { return string.IsNullOrWhiteSpace(QualifiedName); }
        }

        public TypeScriptModule()
        {
            Interfaces = new List<TypeScriptInterface>();
            Enums = new List<TypeScriptEnum>();
        }
    }
}
