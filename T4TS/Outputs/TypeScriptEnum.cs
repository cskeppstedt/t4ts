using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeScriptEnum : TypeScriptType
    {
        IList<TypeScriptEnumValue> Values { get; set; }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public TypeScriptModule Module { get; set; }

        public string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TypeScriptEnum()
        {
            this.Values = new List<TypeScriptEnumValue>();
        }
    }
}
