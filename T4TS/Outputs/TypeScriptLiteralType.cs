using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeScriptLiteralType : TypeScriptType
    {
        public TypeScriptLiteralType(string name)
        {
            this.FullName = this.Name = name;
        }

        public string Name { get; private set; }

        public string FullName { get; private set; }

        public TypeScriptModule Module
        {
            get { return null; }
        }
    }
}
