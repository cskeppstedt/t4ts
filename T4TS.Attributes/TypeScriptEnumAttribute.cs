using System;

namespace T4TS
{
    /// <summary>
    /// Add this attribute to an enum to generate a corresponding TypeScript enum.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple=false, Inherited=true)]
    public class TypeScriptEnumAttribute: Attribute
    {
        /// <summary>
        /// Specifies which module the enum should be placed.
        /// The default module will be used if not specified.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// The name of the enum.
        /// If not specified, the name of the class will be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If specified, the enum name will be prefixed with this string.
        /// </summary>
        public string NamePrefix { get; set; }
    }
}