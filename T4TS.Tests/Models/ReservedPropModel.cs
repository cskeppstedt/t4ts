using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Models
{
    [TypeScriptInterface]
    public class ReservedPropModel
    {
        public string @class { get; set; }

        public string @readonly { get; set; }

        [TypeScriptMember(Optional = true)]
        public bool @public { get; set; }
    }
}
