using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class NullableType : TypescriptType
    {
        public TypescriptType WrappedType { get; set; }

        public override string ToString()
        {
            return WrappedType.ToString();
        }
    }
}
