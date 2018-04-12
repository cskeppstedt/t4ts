using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public class TypeScriptConstructor : TypeScriptMethod
    {
        public IList<string> BaseArguments { get; set; }

        public TypeScriptConstructor()
        {
            this.BaseArguments = new List<string>();

            this.Name = "constructor";
        }
    }
}
