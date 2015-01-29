using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Nullable
{
    [TypeScriptInterface]
    public class NullableModel
    {
        public int? NullableInt { get; set; }
        public double? NullableDouble { get; set; }
    }
}
