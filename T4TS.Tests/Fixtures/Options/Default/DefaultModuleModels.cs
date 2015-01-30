using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Options.Default
{
    [TypeScriptInterface]
    public class DefaultModuleModel
    {
        public string SomeProp { get; set; }
    }

    [TypeScriptInterface(Module="Override")]
    public class DefaultModuleOverrideModel
    {
        public string SomeProp { get; set; }
    }
}
