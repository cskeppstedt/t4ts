using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Options.Default
{
    [TypeScriptInterface]
    public class DefaultInterfaceNamePrefixModel
    {
        public string SomeProp { get; set; }
    }

    [TypeScriptInterface(NamePrefix="PrefixOverride")]
    public class DefaultInterfaceNamePrefixOverrideModel
    {
        [TypeScriptMember(Name="OverrideName")]
        public string SomeProp { get; set; }
    }
}
