using System.Collections.Generic;

namespace T4TS
{
    public class TypeScriptEnum
    {
        public TypeScriptEnum()
        {
            Members = new List<TypeScriptEnumMember>();
        }

        public string Name { get; set; }
        public string FullName { get; set; }

        public List<TypeScriptEnumMember> Members { get; set; }
        public TypeScriptModule Module { get; set; }
        public TypeScriptInterface Owner { get; set; }
    }
}