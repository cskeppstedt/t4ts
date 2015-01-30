using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Options.Names
{
    [TypeScriptInterface(NamePrefix="Foo")]
    public class InterfaceNamePrefixModel
    {
        public string SomeProp { get; set; }
    }

    [TypeScriptInterface(Name = "Bar")]
    public class InterfaceNameOverrideModel
    {
        public string SomeOtherProp { get; set; }
    }

    [TypeScriptInterface(Name = "Bar", Module="SomeModule")]
    public class ModuleNameOverrideModel
    {
        public string SomeThirdProp { get; set; }
    }
}
