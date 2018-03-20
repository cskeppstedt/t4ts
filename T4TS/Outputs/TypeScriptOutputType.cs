using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public interface TypeScriptOutputType
    {
        string Name { get; }
        string FullName { get; set; }

        TypeScriptModule Module { get; }
    }
}
