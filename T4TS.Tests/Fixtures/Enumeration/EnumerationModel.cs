using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.Enumeration
{
    public class EnumerationModel
    {
        public ExplicitValueEnum ExplicitProperty { get; set; }
        public ExplicitValueEnum[] ExplicitArray { get; set; }
        public List<ExplicitValueEnum> ExplicitList { get; set; }

        public ImplicitValueEnum ImplicitProperty { get; set; }
        public ImplicitValueEnum[] ImplicitArray { get; set; }
        public List<ImplicitValueEnum> ImplicitList { get; set; }
    }
}
