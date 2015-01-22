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
    public class InheritanceModel
    {
        public BasicModel Basic { get; set; }
        public ModelFromDifferentProject External { get; set; }
    }
}
