using System.Collections.Generic;

namespace T4TS
{
    public class TypeScriptInterface
    {
        public TypeScriptInterface()
        {
            Members = new List<TypeScriptInterfaceMember>();
            SubClasses = new List<TypeScriptInterface>();
            SubEnums = new List<TypeScriptEnum>();
        }

        public string Name { get; set; }
        public string FullName { get; set; }

        public List<TypeScriptInterfaceMember> Members { get; set; }
        public TypescriptType IndexedType { get; set; }
        public TypeScriptInterface Parent { get; set; }
        public TypeScriptModule Module { get; set; }
        public TypeScriptInterface Owner { get; set; }


        public List<TypeScriptInterface> SubClasses { get; set; }
        public List<TypeScriptEnum> SubEnums { get; set; }
    }
}