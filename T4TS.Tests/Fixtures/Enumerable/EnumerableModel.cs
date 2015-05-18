using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Example.Models;
using T4TS.Tests.Fixtures.Basic;

namespace T4TS.Tests.Fixtures.Enumerable
{
    [TypeScriptInterface]
    public partial class EnumerableModel
    {
        public int NormalProperty { get; set; }
        public int[] PrimitiveArray { get; set; }
        public List<int> PrimitiveList { get; set; }
        public BasicModel[] InterfaceArray { get; set; }
        public List<BasicModel> InterfaceList { get; set; }
        public int[][] DeepArray { get; set; }
        public List<List<int>> DeepList { get; set; }
        public IEnumerable<string> Generic { get; set; }
    }
}
