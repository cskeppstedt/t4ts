using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Outputs;

namespace T4TS.Outputs
{
    public interface TypeScriptOutputType
    {
        string Name { get; }
        string FullName { get; set; }

        TypeScriptModule Module { get; }
    }
}
