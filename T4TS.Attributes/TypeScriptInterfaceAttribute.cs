using System;

namespace T4TS
{
    /// <summary>
    ///     Add this attribute to a class to generate a corresponding TypeScript interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TypeScriptInterfaceAttribute : Attribute
    {
        /// <summary>
        ///     Specifies which module the interface should be placed.
        ///     The default module will be used if not specified.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        ///     The name of the interface.
        ///     If not specified, the name of the class will be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     If specified, the interface name will be prefixed with this string.
        /// </summary>
        public string NamePrefix { get; set; }
    }
}