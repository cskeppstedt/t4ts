using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Generics
{
    public class GenericModel<TItem>
    {
        public TItem Item { get; set; }
        public int SomethingElse { get; set; }
    }

    public class InheritGenericModel : GenericModel<string>
    {
        public string SubTypeProperty { get; set; }
    }
}
