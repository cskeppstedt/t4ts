using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Enumeration
{
    public enum SampleEmum
    {
        First = 1,
        Second = 2,
        Fifth = 5
    }

    public class EnumerationModel
    {
        public SampleEmum NormalProperty { get; set; }
        public SampleEmum[] PrimitiveArray { get; set; }
        public List<SampleEmum> PrimitiveList { get; set; }
    }
}
