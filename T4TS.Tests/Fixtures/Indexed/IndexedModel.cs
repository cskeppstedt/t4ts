using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Tests.Fixtures.Basic;

namespace T4TS.Tests.Fixtures.Indexed
{
    [TypeScriptInterface]
    public class IndexedComplexModel : List<BasicModel>
    {
        public int SomeProp { get; set; }
    }

    [TypeScriptInterface]
    public class IndexedPrimitiveModel : List<string>
    {
        public int SomeProp { get; set; }
    }
}
