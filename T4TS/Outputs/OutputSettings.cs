using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public class OutputSettings
    {
        /// <summary>
        /// The version of Typescript that is targeted
        /// </summary>
        public Version CompatibilityVersion { get; set; }

        public bool OpenBraceOnNextLine { get; set; }

        public bool OrderInterfacesByReference { get; set; }

        public bool IsDeclaration { get; set; }

        public OutputSettings()
        {
            this.CompatibilityVersion = new Version(0, 9, 1, 1);
            this.IsDeclaration = true;
        }
    }
}
