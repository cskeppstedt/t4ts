using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class Settings
    {
        /// <summary>
        /// The default module of the generated interface, if not specified by the TypeScriptInterfaceAttribute
        /// </summary>
        public string DefaultModule { get; set; }

        /// <summary>
        /// The default value for Optional, if not specified by the TypeScriptMemberAttribute
        /// </summary>
        public bool DefaultOptional { get; set; }

        /// <summary>
        /// The default value for the CamelCase flag for an interface member name, if not specified by the TypeScriptMemberAttribute
        /// </summary>
        public bool DefaultCamelCaseMemberNames { get; set; }
    }
}
