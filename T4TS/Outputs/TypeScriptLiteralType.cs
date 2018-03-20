using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeScriptLiteralType : TypeScriptOutputType
    {
        public string Name
        {
            get { return this.FullName; }
        }

        public string FullName { get; set; }

        public TypeScriptModule Module
        {
            get { return null; }
        }
    }
}
