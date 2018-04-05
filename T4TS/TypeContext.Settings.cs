using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public partial class TypeContext
    {
        public class Settings
        {
            public bool UseNativeDates { get; set; }

            public Version CompatibilityVersion { get; set; }

            public Settings()
            {
                this.CompatibilityVersion = new Version(0, 9, 1, 1);
            }
        }
    }
}
