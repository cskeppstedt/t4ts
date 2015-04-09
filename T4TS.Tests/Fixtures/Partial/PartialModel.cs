using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Example.Models;
using T4TS.Tests.Fixtures.Basic;

namespace T4TS.Tests.Fixtures.Partial
{
    [TypeScriptInterface]
    public partial class PartialModel
    {
        public BasicModel OnPartialModel { get; set; }
    }

    public partial class PartialModel : ModelFromDifferentProject
    {
        public BasicModel OnOtherPartialModel { get; set; }
    }
}
