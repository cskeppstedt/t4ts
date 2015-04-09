using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Options.Default
{
    [TypeScriptInterface]
    public class DefaultOptionalModel
    {
        public string SomeProp { get; set; }
    }

    [TypeScriptInterface]
    public class DefaultOptionalOverrideModel
    {
        [TypeScriptMember(Optional = false)]
        public string SomeProp { get; set; }
    }
}
