using System;
using System.Collections.Generic;

namespace T4TS.Example.Models
{
    /// <summary>
    /// Barfoo has some comments!
    /// <example>var bar = new Barfoo();</example>
    /// </summary>
    [TypeScriptInterface(Module = "")]
    public class Barfoo
    {
        /// <summary>
        /// Well, this is a number
        /// And has multiple lines of comment
        /// "Nicely" formated
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Okay, this has a single line of comment
        /// </summary>
        public Inherited Complex { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public Dictionary<string, object> aValue { get; set; }
    }
}
