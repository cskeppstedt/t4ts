using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypescriptType
    {
        public virtual string Name { get { return "any"; } }

        public override string ToString()
        {
            return Name;
        }
    }
}
