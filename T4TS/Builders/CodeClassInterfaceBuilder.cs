using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Builders
{
    public interface CodeClassInterfaceBuilder
    {
        TypeScriptInterface Build(
            CodeClass codeClass,
            TypeContext typeContext);
    }
}
