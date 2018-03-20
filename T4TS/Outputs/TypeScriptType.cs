using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public interface TypeScriptType
    {
        string Name { get; }
        string FullName { get; }

        TypeScriptModule Module { get; }
    }
}
