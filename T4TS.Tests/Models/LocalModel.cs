using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Models
{
    [TypeScriptInterface]
    public class LocalModel
    {
        public int Id { get; set; }

        [TypeScriptMember(Optional = true)]
        public string Optional { get; set; }
    }
}
