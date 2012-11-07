using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class AttributeDecoratedInstance
    {
        public CodeNamespace Namespace { get; set; }
        public CodeType CodeType { get; set; }
        public string TypescriptType { get; set; }
    }
}
