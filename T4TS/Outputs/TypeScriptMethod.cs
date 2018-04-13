using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public class TypeScriptMethod : TypeScriptMember
    {
        public IList<TypeScriptMember> Arguments { get; set; }

        public IList<TypeReference> TypeArguments { get; set; }

        public OutputAppender<TypeScriptMethod> Appender { get; set; }

        public bool IsStatic { get; set; }

        public TypeScriptMethod()
        {
            this.Arguments = new List<TypeScriptMember>();
            this.TypeArguments = new List<TypeReference>();
        }
    }
}
