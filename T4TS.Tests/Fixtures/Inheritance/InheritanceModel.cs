using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Example.Models;
using T4TS.Tests.Fixtures.Basic;

namespace T4TS.Tests.Fixtures.Inheritance
{
    [TypeScriptInterface]
    public class InheritanceModel : OtherInheritanceModel
    {
        public BasicModel OnInheritanceModel { get; set; }
    }

    [TypeScriptInterface]
    public class OtherInheritanceModel : ModelFromDifferentProject
    {
        public BasicModel OnOtherInheritanceModel { get; set; }
    }
}
