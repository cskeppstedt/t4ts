using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Fixtures.BasicInterface
{
    public interface IBasic
    {
        int MyProperty { get; set; }
        DateTime SomeDateTime { get; set; }
    }
}
