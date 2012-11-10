using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class ArrayType: TypescriptType
    {
        public TypescriptType ElementType { get; set; }

        public override string ToString()
        {
            return ElementType.ToString() + "[]";
        }
    }
}
