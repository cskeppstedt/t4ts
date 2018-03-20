using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeScriptEnum : TypeScriptOutputType
    {
        public IList<TypeScriptEnumValue> Values { get; set; }

        public string Name { get; set; }
        public TypeScriptModule Module { get; set; }

        public string FullName
        {
            get; set;
        }

        public TypeScriptEnum()
        {
            this.Values = new List<TypeScriptEnumValue>();
        }
    }
}
