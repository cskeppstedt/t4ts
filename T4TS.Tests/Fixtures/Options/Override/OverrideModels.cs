using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Options.Override
{
    [TypeScriptInterface(NamePrefix = "Foo")]
    public class InterfaceNamePrefixModel
    {
        public string SomeProp { get; set; }
    }

    [TypeScriptInterface(Name = "Bar")]
    public class InterfaceNameOverrideModel
    {
        public string SomeOtherProp { get; set; }
    }

    [TypeScriptInterface(Module = "SomeModule")]
    public class ModuleNameOverrideModel
    {
        public string SomeThirdProp { get; set; }
    }

    [TypeScriptInterface]
    public class MemberNameOverrideModel
    {
        [TypeScriptMember(Name = "OverriddenName")]
        public string OriginalName { get; set; }
    }

    [TypeScriptInterface]
    public class MemberOptionalModel
    {
        [TypeScriptMember(Optional = true)]
        public string Member { get; set; }
    }

    [TypeScriptInterface]
    public class MemberCamelCaseModel
    {
        [TypeScriptMember(CamelCase = true)]
        public string MemberName { get; set; }
    }

    [TypeScriptInterface]
    public class MemberTypeModel
    {
        [TypeScriptMember(Type = "number")]
        public string NotANumber { get; set; }
    }

    [TypeScriptInterface]
    public class MemberIgnoreModel
    {
        [TypeScriptMember(Ignore = true)]
        public string Ignored { get; set; }

        [TypeScriptMember(Ignore = false)]
        public string NotIgnored { get; set; }
    }
}
