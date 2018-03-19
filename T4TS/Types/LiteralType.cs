using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class LiteralType : TypescriptType
    {
        private string name;

        public override string Name
        {
            get { return this.name; }
        }

        public LiteralType(string name)
        {
            this.name = name;
        }
    }
}
