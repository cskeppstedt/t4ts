
namespace T4TS
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    
    /// <summary>
    /// Add this attribute to a class to generate a corresponding TypeScript interface.
    /// </summary>
    [GeneratedCode("T4TS", "1.0")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class TypeScriptInterfaceAttribute: Attribute
    {
        /// <summary>
        /// Specifies which module the interface should be placed.
        /// The default module will be used if not specified.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// The name of the interface.
        /// If not specified, the name of the class will be used.
        /// </summary>
        public string Name { get; set; }
    }
    
    /// <summary>
    /// Add this attribute to a property to customize the generated interface member
    /// </summary>
    [GeneratedCode("T4TS", "1.0")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class TypeScriptMemberAttribute: Attribute
    {
        /// <summary>
        /// The member name in the interface.
        /// If not specified, the property name will be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specify if the member should be optional, ie. "name?: type".
        /// If not specified, the default value will be used.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Specify which type the interface member will have.
        /// If not specified, a suitable type will be determined.
        /// </summary>
        public string Type { get; set; }
    }
}
