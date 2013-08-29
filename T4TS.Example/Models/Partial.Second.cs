using System;

namespace T4TS.Example.Models
{
    [TypeScriptInterface]
    partial class Partial
    {
        public string FromSecondClass { get; set; }
        public bool? AlsoSecondClass { get; set; }
    }
}
