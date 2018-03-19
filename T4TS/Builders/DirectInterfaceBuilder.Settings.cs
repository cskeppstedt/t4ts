using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Builders
{
    public partial class DirectInterfaceBuilder
    {
        public class Settings
        {
            public string ModuleRoot { get; set; }

            public string NamespaceRoot { get; set; }

            public bool CamelCase { get; set; }
        }
    }
}
