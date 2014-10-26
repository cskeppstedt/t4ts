using System;

namespace T4TS
{
    /// <summary>
    /// Add this attribute to a property to customize the generated interface member
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=true)]
    public class TypeScriptMemberAttribute: Attribute
    {
        /// <summary>
        /// The member name in the interface.
        /// If not specified, the property name will be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specify if the member should be optional, ie. "name?: type".
        /// If not specified, the default value will be used (see settings).
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Specify which type the interface member will have.
        /// If not specified, a suitable type will be determined.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// If set to true, the first character of the member name will be lower cased.
        /// If not specified, the default value will be used (see settings).
        /// </summary>
        public bool CamelCase { get; set; }

        /// <summary>
        /// If set to true, this property will be ignored.
        /// </summary>
        public bool Ignore { get; set; }
    }
}
